using IdentityServer4.EntityFramework.Options;

using LinkedLanguages.DAL.Models;

using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LinkedLanguages.DAL
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<KnownLanguageToUser> KnownLanguageToUsers { get; set; }
        public DbSet<UnknownLanguageToUser> UnknownLanguageToUsers { get; set; }
        public DbSet<Language> Languages { get; set; }

        public DbSet<WordPair> WordPairs { get; set; }
        public DbSet<WordPairToApplicationUser> WordPairToApplicationUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Language>().HasData(LanguageSeed.GetStaticLanguages());
            base.OnModelCreating(builder);
        }
    }
}
