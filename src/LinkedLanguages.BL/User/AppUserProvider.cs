
using LinkedLanguages.DAL;

using Microsoft.AspNetCore.Http;

using System;
using System.Linq;
using System.Security.Claims;

namespace LinkedLanguages.BL
{
    public class AppUserProvider : IAppUserProvider
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IHttpContextAccessor httpContext;

        public AppUserProvider(ApplicationDbContext dbContext, IHttpContextAccessor httpContext)
        {
            this.dbContext = dbContext;
            this.httpContext = httpContext;
        }
        public Guid GetUserId()
        {
            return httpContext.HttpContext.User.GetUserId();
        }

        public string GetUserKnownLanguage()
        {
            return dbContext.KnownLanguageToUsers
                .Where(kltu => kltu.ApplicationUserId == httpContext.HttpContext.User.GetUserId())
                .Select(k => k.Language.Code)
                .FirstOrDefault();
        }
    }
}
