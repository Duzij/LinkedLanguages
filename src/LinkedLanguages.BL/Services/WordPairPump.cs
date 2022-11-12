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
                LanguageOffset offset = await dbContext.LanguageOffsets.FirstOrDefaultAsync(a => a.Key == key);
                int pageNumber = 0;

                if (offset != null)
                {
                    pageNumber = offset.PageNumer++;
                }
                else
                {
                    await dbContext.LanguageOffsets.AddAsync(new LanguageOffset() { Key = key, PageNumer = pageNumber });
                }
                await dbContext.SaveChangesAsync();

                List<WordPair> results = pairsQuery.Execute(new WordPairParameterDto(knownLangCode, knownLangugageId, unknownLangCode, unknownLanguageId, pageNumber));

                TransliteratedWordsDto[] databaseWords = await transliteratedWordParisQuery.GetQueryable(unknownLanguageId, knownLangugageId).ToArrayAsync();

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
}
