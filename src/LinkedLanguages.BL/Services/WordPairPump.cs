using AnyAscii;
using LinkedLanguages.BL.CLLD;
using LinkedLanguages.BL.Query;
using LinkedLanguages.BL.SPARQL.Query;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedLanguages.BL.Services
{
    public class WordPairPump
    {
        private readonly WordPairsSparqlQuery pairsQuery;
        private readonly UnusedUserWordPairsQuery unusedUserWordPairs;
        private readonly ApplicationDbContext dbContext;

        public WordPairPump(WordPairsSparqlQuery pairsQuery,
                            UnusedUserWordPairsQuery unusedUserWordPairs,
                            ApplicationDbContext dbContext)
        {
            this.pairsQuery = pairsQuery;
            this.unusedUserWordPairs = unusedUserWordPairs;
            this.dbContext = dbContext;
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

            Guid unknownLangugageId = dbContext.Languages
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

                List<WordPair> results = pairsQuery.Execute(new WordPairParameterDto(knownLangCode, knownLangugageId, unknownLangCode, unknownLangugageId, pageNumber));

                results = await PostProcessResults(results, knownLangugageId, unknownLangugageId);

                if (results.Any())
                {
                    //Add new words to database
                    await dbContext.WordPairs.AddRangeAsync(results);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    //Pump more words
                    await Pump(knownLangCode, unknownLangCode);
                }
            }

        }

        private async Task<List<WordPair>> PostProcessResults(List<WordPair> results, Guid knownLangugageId, Guid unknownLanguageId)
        {
            List<WordPair> returnedResults = new List<WordPair>();

            WordPair[] languageWords = await dbContext.WordPairs.AsNoTracking()
                .Where(a => a.UnknownLanguageId == unknownLanguageId && a.KnownLanguageId == knownLangugageId)
                .ToArrayAsync();

            foreach (WordPair wp in results)
            {
                //Same words are ignored, as they are easy to recognize
                if (string.Compare(wp.UnknownWord, wp.KnownWord, ignoreCase: true) == 0)
                {
                    continue;
                }

                //Suffixes and prefixes are ignored
                if (wp.UnknownWord.First() == '-' || wp.UnknownWord.Last() == '-' ||
                    wp.KnownWord.First() == '-' || wp.KnownWord.Last() == '-')
                {
                    continue;
                }

                wp.KnownWordTransliterated = wp.KnownWord.Transliterate();
                wp.UnknownWordTransliterated = wp.UnknownWord.Transliterate();

                //Words with same transliterated known and unknown words are ignored
                bool predicate(WordPair a)
                {
                    return a.KnownWordTransliterated == wp.KnownWordTransliterated &&
                                        a.UnknownWordTransliterated == wp.UnknownWordTransliterated;
                }

                //Ignored from database
                if (languageWords.FirstOrDefault(predicate) is not null)
                {
                    continue;
                }

                //Ignored from previously saved results
                if (returnedResults.FirstOrDefault(predicate) is not null)
                {
                    continue;
                }

                //Distances higher than 3 are ignored
                CalculateCLLD(knownLangugageId, unknownLanguageId, wp);
                if (wp.Distance > 3)
                {
                    continue;
                }

                returnedResults.Add(wp);
            }

            return returnedResults;
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
