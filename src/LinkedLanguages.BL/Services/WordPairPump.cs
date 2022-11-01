using AnyAscii;
using LinkedLanguages.BL.CLLD;
using LinkedLanguages.BL.Query;
using LinkedLanguages.BL.SPARQL.Query;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedLanguages.BL.Services
{
    public class WordPairPump
    {
        private readonly WordPairsSparqlQuery pairsQuery;
        private readonly WordSeeAlsoLinkSparqlQuery wordSeeAlsoLinkSparqlQuery;
        private readonly UnusedUserWordPairsQuery unusedUserWordPairs;
        private readonly TransliteratedWordParisQuery transliteratedWordParisQuery;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<WordPairPump> logger;

        public WordPairPump(WordPairsSparqlQuery pairsQuery,
                            WordSeeAlsoLinkSparqlQuery wordSeeAlsoLinkSparqlQuery,
                            UnusedUserWordPairsQuery unusedUserWordPairs,
                            TransliteratedWordParisQuery transliteratedWordParisQuery,
                            ApplicationDbContext dbContext,
                            ILogger<WordPairPump> logger)
        {
            this.pairsQuery = pairsQuery;
            this.wordSeeAlsoLinkSparqlQuery = wordSeeAlsoLinkSparqlQuery;
            this.unusedUserWordPairs = unusedUserWordPairs;
            this.transliteratedWordParisQuery = transliteratedWordParisQuery;
            this.dbContext = dbContext;
            this.logger = logger;
        }

        /// <summary>
        /// New words pump will be performed only when neccessary
        /// </summary>
        /// <param name="knownLangCode"></param>
        /// <param name="unknownLangCode"></param>
        public async Task Pump(string knownLangCode, string unknownLangCode)
        {
            Guid knownLangugageId = dbContext.Languages
                .AsNoTracking()
                .Where(l => l.Code == knownLangCode)
                .Select(l => l.Id)
                .Single();

            Guid unknownLanguageId = dbContext.Languages
                .AsNoTracking()
                .Where(l => l.Code == unknownLangCode)
                .Select(l => l.Id)
                .Single();

            IQueryable<WordPair> remainingWordsForCurrentUser = unusedUserWordPairs.GetQueryable(knownLangCode, unknownLangCode);

            if (!remainingWordsForCurrentUser.Any())
            {
                string key = $"{knownLangCode}-{unknownLangCode}";
                LanguagePageNumber offset = await dbContext.LanguageOffsets.FirstOrDefaultAsync(a => a.Key == key);
                int pageNumber = 0;

                if (offset != null)
                {
                    pageNumber = offset.PageNumer++;
                }
                else
                {
                    await dbContext.LanguageOffsets.AddAsync(new LanguagePageNumber() { Key = key, PageNumer = pageNumber });
                }
                await dbContext.SaveChangesAsync();

                List<WordPair> results = pairsQuery.Execute(new WordPairParameterDto(knownLangCode, knownLangugageId, unknownLangCode, unknownLanguageId, pageNumber));

                var databaseWords = await transliteratedWordParisQuery.GetQueryable(unknownLanguageId, knownLangugageId).ToArrayAsync();

                results = results.FilterSameWords()
                       .FilterSuffixesAndPrefixes()
                       .FilterWordPairsWithHighCLLD(knownLangugageId, unknownLanguageId)
                       .FilterTransliteratedWordsFromDatabase(databaseWords)
                       .FilterTransliteratedDuplicates()
                       .ToList();

                if (results.Any())
                {
                    //Add new words to database, pump is finished
                    await dbContext.WordPairs.AddRangeAsync(results);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    //Pump more words, all results were filtered out
                    await Pump(knownLangCode, unknownLangCode);
                }
            }

        }
    }

    public static class WordPairResultsPipeline
    {
        public static IEnumerable<WordPair> FilterSameWords(this IEnumerable<WordPair> results)
        {
            return results.Where(wp => string.Compare(wp.UnknownWord, wp.KnownWord, ignoreCase: true) != 0);
        }

        public static IEnumerable<WordPair> FilterSuffixesAndPrefixes(this IEnumerable<WordPair> results)
        {
            return results.Where(
                    wp => wp.UnknownWord.First() != '-' || wp.UnknownWord.Last() != '-' ||
                    wp.KnownWord.First() != '-' || wp.KnownWord.Last() != '-');
        }

        public static IEnumerable<WordPair> FilterWordPairsWithHighCLLD(this IEnumerable<WordPair> results, Guid knownLangugageId, Guid unknownLanguageId)
        {
            results.ToList().ForEach((wp) =>
            {
                wp.KnownWordTransliterated = wp.KnownWord.Transliterate();
                wp.UnknownWordTransliterated = wp.UnknownWord.Transliterate();
                CalculateCLLD(knownLangugageId, unknownLanguageId, wp);
            });

            return results.Where(wp => wp.Distance < 3);
        }

        public static IEnumerable<WordPair> FilterTransliteratedWordsFromDatabase(this IEnumerable<WordPair> results, TransliteratedWordsDto[] databaseWords)
        {
            var returnedResults = new List<WordPair>();

            foreach (var wp in results)
            {
                if (databaseWords.Any(db => db.KnownWordTransliterated == wp.KnownWordTransliterated && db.UnknownWordTransliterated == wp.UnknownWordTransliterated))
                {
                    continue;
                }

                returnedResults.Add(wp);
            }

            return returnedResults;
        }

        public static IEnumerable<WordPair> FilterTransliteratedDuplicates(this IEnumerable<WordPair> results)
        {
            return results.Where(a => string.Compare(a.UnknownWordTransliterated, a.KnownWordTransliterated, ignoreCase: true) != 0)
                .DistinctBy(a => new { a.UnknownWordTransliterated, a.KnownWordTransliterated });
        }

        /// <summary>
        /// Static predefined dictionary of characters is used only for  German, French and English languages
        /// </summary>
        /// <param name="knownLangugageId"></param>
        /// <param name="unknownLanguageId"></param>
        /// <param name="wp"></param>
        private static void CalculateCLLD(Guid knownLangugageId, Guid unknownLanguageId, WordPair wp)
        {
            Dictionary<string, string> characterMapping = new Dictionary<string, string>();

            if (unknownLanguageId == LanguageSeed.FrenchLanguageId && knownLangugageId == LanguageSeed.EnglishLanguageId)
            {
                characterMapping = FrenchCharacterMapper.FrenchToEnglishMapping;
            }
            else if (unknownLanguageId == LanguageSeed.EnglishLanguageId && knownLangugageId == LanguageSeed.FrenchLanguageId)
            {
                characterMapping = FrenchCharacterMapper.EnglishToFrenchMapping;
            }
            if (unknownLanguageId == LanguageSeed.GermanLanguageId && knownLangugageId == LanguageSeed.EnglishLanguageId)
            {
                characterMapping = GermanCharacterMapper.GermanToEnglishMapping;
            }
            else if (unknownLanguageId == LanguageSeed.EnglishLanguageId && knownLangugageId == LanguageSeed.GermanLanguageId)
            {
                characterMapping = GermanCharacterMapper.EnglishToGermanMapping;
            }

            wp.Distance = CrossLingualLevenshteinDistanceCalculator.Calc(wp.KnownWordTransliterated, wp.UnknownWordTransliterated, characterMapping);
        }
    }
}
