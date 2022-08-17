using LinkedLanguages.DAL;

using Microsoft.Extensions.Caching.Memory;

using System.Linq;
using System.Threading.Tasks;

namespace LinkedLanguages.BL
{
    public class WordPairPump
    {
        private readonly SparqlPairsQuery pairsQuery;
        private readonly IMemoryCache memoryCache;
        private readonly UnusedUserWordPairsQuery unusedUserWordPairs;
        private readonly ApplicationDbContext dbContext;

        public WordPairPump(SparqlPairsQuery pairsQuery,
                            IMemoryCache memoryCache,
                            UnusedUserWordPairsQuery unusedUserWordPairs,
                            ApplicationDbContext dbContext)
        {
            this.pairsQuery = pairsQuery;
            this.memoryCache = memoryCache;
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

            var remainingUnusedWords = unusedUserWordPairs.GetQueryable(unknownLangCode);

            if (!remainingUnusedWords.Any())
            {
                //Offset will have to be reparated per language
                memoryCache.TryGetValue("offset", out int offset);
                memoryCache.Set("offset", offset + 1);

                var results = pairsQuery.Execute(knownLangCode, knownLangugageId, unknownLangCode, unknownLangugageId, offset, 3);

                //Add new words to database
                await dbContext.WordPairs.AddRangeAsync(results);
                await dbContext.SaveChangesAsync();
            }

        }
    }
}
