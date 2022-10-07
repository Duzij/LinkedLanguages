using LinkedLanguages.BL.Query;
using LinkedLanguages.BL.SPARQL.Query;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;
using Microsoft.EntityFrameworkCore;

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

            var remainingUnusedWords = unusedUserWordPairs.GetQueryable(knownLangCode, unknownLangCode);

            if (!remainingUnusedWords.Any())
            {
                //Offset will have to be reparated per language
                var key = $"{knownLangCode}-{unknownLangCode}";
                var offset = await dbContext.LanguageOffsets.FirstOrDefaultAsync(a => a.Key == key);
                int pageNumber = 0;
                if (offset != null)
                {
                    offset.PageNumer++;
                }
                else
                {
                    await dbContext.LanguageOffsets.AddAsync(new LanguagePageNumber() { Key = key, PageNumer = pageNumber++ });
                }

                var results = pairsQuery.Execute(new WordPairParameterDto(knownLangCode, knownLangugageId, unknownLangCode, unknownLangugageId, pageNumber, 3));

                //Add new words to database
                await dbContext.WordPairs.AddRangeAsync(results);
                await dbContext.SaveChangesAsync();
            }

        }
    }
}
