using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;

using System;
using System.Linq;

namespace LinkedLanguages.BL
{
    public class UnusedUserWordPairsQuery
    {
        private readonly ApplicationDbContext appDbContext;
        private readonly ApprovedWordPairsQuery approvedWordPairs;

        public UnusedUserWordPairsQuery(ApplicationDbContext appDbContext, ApprovedWordPairsQuery approvedWordPairs)
        {
            this.appDbContext = appDbContext;
            this.approvedWordPairs = approvedWordPairs;
        }

        /// <summary>
        /// Query returns unused word pairs excluding already used word pairs for current user
        /// </summary>
        /// <param name="unknownLanguageCode"></param>
        /// <returns></returns>
        public IQueryable<WordPair> GetQueryable(string knownLanguageCode, string unknownLanguageCode)
        {
            var userWordPairIds = approvedWordPairs.GetQueryable(knownLanguageCode, unknownLanguageCode).Select(a => a.Id);

            return appDbContext.WordPairs
                .Where(wp => wp.UnknownLanguageCode == unknownLanguageCode)
                .Where(wp => wp.KnownLanguageCode == knownLanguageCode)
                .Where(uwp => !userWordPairIds.Contains(uwp.Id));
        }
    }
}
