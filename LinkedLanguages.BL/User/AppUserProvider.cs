
using LinkedLanguages.DAL;

using System;
using System.Linq;
using System.Security.Claims;

namespace LinkedLanguages.BL
{
    public class AppUserProvider : IAppUserProvider
    {
        private readonly ClaimsPrincipal claimsPrincipal;
        private readonly ApplicationDbContext dbContext;

        public AppUserProvider(ClaimsPrincipal claimsPrincipal, ApplicationDbContext dbContext)
        {
            this.claimsPrincipal = claimsPrincipal;
            this.dbContext = dbContext;
        }
        public Guid GetUserId()
        {
            return claimsPrincipal.GetUserId();
        }

        public string GetUserKnownLanguage()
        {
            return dbContext.KnownLanguageToUsers
                .Where(kltu => kltu.ApplicationUserId == claimsPrincipal.GetUserId())
                .Select(k => k.Language.Code)
                .FirstOrDefault();
        }
    }
}
