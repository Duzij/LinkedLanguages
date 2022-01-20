using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedLanguages.BL
{
    public class UnusedUserWordPairsQuery
    {
        private readonly ApplicationDbContext appDbContext;
        private readonly IAppUserProvider appUserProvider;

        public UnusedUserWordPairsQuery(ApplicationDbContext appDbContext, IAppUserProvider appUserProvider)
        {
            this.appDbContext = appDbContext;
            this.appUserProvider = appUserProvider;
        }

        /// <summary>
        /// Query returns unused word pairs excluding already used word pairs for current user
        /// </summary>
        /// <param name="unknownLanguageCode"></param>
        /// <returns></returns>
        public IQueryable<WordPair> GetQueryable(string unknownLanguageCode)
        {
            var userWordPairIds = appDbContext.WordPairToApplicationUsers
              .Where(wp => !wp.Rejected)
              .Where(wp => wp.WordPair.UnknownLanguageCode == unknownLanguageCode)
              .Where(wp => wp.ApplicationUserId == appUserProvider.GetUserId());

            return appDbContext.UnusedWordPairs.Where(uwp => !userWordPairIds.Select(uwp => uwp.WordPairId).Contains(uwp.Id));
        }
    }
}
