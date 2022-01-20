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
        /// <param name="knownLang"></param>
        /// <param name="foreignLang"></param>
        public async Task Pump(string knownLang, string foreignLang)
        {
            var remainingUnusedWords = unusedUserWordPairs.GetQueryable(foreignLang);

            if (!remainingUnusedWords.Any())
            {
                memoryCache.TryGetValue("offset", out int offset);
                if (offset == 0)
                {
                    offset = 1;
                    memoryCache.Set("offset", offset++);
                }

                var results = pairsQuery.Execute(knownLang, foreignLang, offset, 3);

                //Add new words to database
                await dbContext.UnusedWordPairs.AddRangeAsync(results);
                await dbContext.SaveChangesAsync();
            }

        }
    }
}
