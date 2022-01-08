using LinkedLanguages.DAL;

using Microsoft.Extensions.Caching.Memory;

using System.Linq;
using System.Threading.Tasks;

using VDS.RDF.Query;

namespace LinkedLanguages.BL
{
    public class WordPairPump
    {
        private readonly PairsQuery pairsQuery;
        private readonly ApplicationDbContext appDbContext;
        private readonly IAppUserProvider appUserProvider;
        private readonly IMemoryCache memoryCache;
        private readonly UnusedUserWordPairsQuery unusedUserWordPairs;

        public WordPairPump(PairsQuery pairsQuery,
                            ApplicationDbContext dbContext,
                            IAppUserProvider appUserProvider,
                            IMemoryCache memoryCache,
                            UnusedUserWordPairsQuery unusedUserWordPairs)
        {
            this.pairsQuery = pairsQuery;
            this.appDbContext = dbContext;
            this.appUserProvider = appUserProvider;
            this.memoryCache = memoryCache;
            this.unusedUserWordPairs = unusedUserWordPairs;
        }

        /// <summary>
        /// New words pump will be performed only when neccessary
        /// </summary>
        /// <param name="knownLang"></param>
        /// <param name="foreignLang"></param>
        public void Pump(string knownLang, string foreignLang)
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

                pairsQuery.Pump(knownLang, foreignLang, offset, 3);
            }

        }
    }
}
