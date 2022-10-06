using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;

using System;
using System.Linq;

namespace LinkedLanguages.BL.Query
{
    public class UnusedUserWordPairsQuery
    {
        private readonly ApplicationDbContext appDbContext;
        private readonly WordPairsUserQuery wordPairUser;

        public UnusedUserWordPairsQuery(ApplicationDbContext appDbContext, WordPairsUserQuery approvedWordPairs)
        {
            this.appDbContext = appDbContext;
            wordPairUser = approvedWordPairs;
        }

        /// <summary>
        /// Query returns unused word pairs excluding already used word pairs for current user
        /// </summary>
        /// <param name="unknownLanguageCode"></param>
        /// <returns></returns>
        public IQueryable<WordPair> GetQueryable(string knownLanguageCode, string unknownLanguageCode)
        {
            IQueryable<Guid> userWordPairIds = wordPairUser
                .GetQueryable(knownLanguageCode, unknownLanguageCode)
                .Select(a => a.WordPairId);

            return appDbContext.WordPairs
                .Where(wp => wp.UnknownLanguageCode == unknownLanguageCode)
                .Where(wp => wp.KnownLanguageCode == knownLanguageCode)
                .Where(uwp => !userWordPairIds.Contains(uwp.Id));
        }
    }
}
