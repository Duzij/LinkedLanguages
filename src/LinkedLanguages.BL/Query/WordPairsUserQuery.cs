using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;
using System.Linq;

namespace LinkedLanguages.BL.Query
{
    public class WordPairsUserQuery
    {
        private readonly ApplicationDbContext appDbContext;
        private readonly IAppUserProvider appUserProvider;

        public WordPairsUserQuery(ApplicationDbContext appDbContext, IAppUserProvider appUserProvider)
        {
            this.appDbContext = appDbContext;
            this.appUserProvider = appUserProvider;
        }

        public IQueryable<WordPairToApplicationUser> GetQueryable(string knownLanguageCode, string unknownLanguageCode)
        {
            return appDbContext.WordPairToApplicationUsers
              .Where(wp => wp.WordPair.UnknownLanguageCode == unknownLanguageCode)
              .Where(wp => wp.WordPair.KnownLanguageCode == knownLanguageCode)
              .Where(wp => wp.ApplicationUserId == appUserProvider.GetUserId());
        }
    }
}
