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
        /// Query returns unused word pairs excluding already used word pairs for current user. Word pairs are ordered by user count in a descending order and a distance in an ascending order 
        /// </summary>
        /// <param name="unknownLanguageCode">ISO 639-3 language code</param>
        /// <param name="knownLanguageCode">ISO 639-3 language code</param>
        /// <returns>Word pair query</returns>
        public IQueryable<WordPair> GetQueryable(string knownLanguageCode, string unknownLanguageCode)
        {
            IQueryable<Guid> userWordPairIds = wordPairUser
                .GetQueryable(knownLanguageCode, unknownLanguageCode)
                .Select(a => a.WordPairId);

            return appDbContext.WordPairs
                .Where(wp => wp.UnknownLanguageCode == unknownLanguageCode)
                .Where(wp => wp.KnownLanguageCode == knownLanguageCode)
                .Where(wp => !userWordPairIds.Contains(wp.Id))
                .Where(wp => wp.RejectedCount == null || wp.RejectedCount < appDbContext.Users.Count() / 2)
                .OrderBy(wp => wp.Distance)
                .OrderByDescending(wp => wp.UsedCount);
        }
    }
}
