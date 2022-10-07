using AnyAscii;
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
            var knownLangugageId = dbContext.Languages
                .Where(l => l.Code == knownLangCode)
                .Select(l => l.Id)
                .Single();

            var unknownLangugageId = dbContext.Languages
                .Where(l => l.Code == unknownLangCode)
                .Select(l => l.Id)
                .Single();

            var remainingWordsForCurrentUser = unusedUserWordPairs.GetQueryable(knownLangCode, unknownLangCode);

            if (!remainingWordsForCurrentUser.Any())
            {
                //Offset will have to be reparated per language
                var key = $"{knownLangCode}-{unknownLangCode}";
                var offset = await dbContext.LanguageOffsets.FirstOrDefaultAsync(a => a.Key == key);
                int pageNumber = 0;
                if (offset != null)
                {
                    pageNumber = offset.PageNumer++;
                }
                else
                {
                    await dbContext.LanguageOffsets.AddAsync(new LanguagePageNumber() { Key = key, PageNumer = pageNumber });
                }

                var results = pairsQuery.Execute(new WordPairParameterDto(knownLangCode, knownLangugageId, unknownLangCode, unknownLangugageId, pageNumber));

                results = await PostProcessResults(results, knownLangugageId, unknownLangugageId);

                if (results.Any())
                {
                    //Add new words to database
                    await dbContext.WordPairs.AddRangeAsync(results);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    //Pump some more words
                    await Pump(knownLangCode, unknownLangCode);
                }
            }

        }

        private async Task<List<WordPair>> PostProcessResults(List<WordPair> results, Guid knownLangugageId, Guid unknownLanguageId)
        {
            var postResults = new List<WordPair>();

            var languageWords = await dbContext.WordPairs.AsNoTracking()
                .Where(a => a.UnknownLanguageId == unknownLanguageId && a.KnownLanguageId == knownLangugageId)
                .ToArrayAsync();

            foreach (var wp in results)
            {
                //Same words are ignored, as they are easy to recognize
                if (string.Compare(wp.UnknownWord, wp.KnownWord, ignoreCase: true) == 0)
                {
                    continue;
                }

                var foundWord = languageWords.FirstOrDefault
                    (
                        a => a.KnownWord.Transliterate() == wp.KnownWord.Transliterate() &&
                        a.UnknownWord.Transliterate() == wp.UnknownWord.Transliterate()
                    );

                if (foundWord is null)
                {
                    postResults.Add(wp);
                }
            }
            return postResults;
        }
    }
}
