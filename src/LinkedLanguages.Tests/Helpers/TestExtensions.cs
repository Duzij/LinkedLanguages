using Duende.IdentityServer.EntityFramework.Options;

using LinkedLanguages.BL.SPARQL;
using LinkedLanguages.DAL;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Moq;

using System;

namespace LinkedLanguages.Tests.Helpers
{
    public static class TestServices
    {
        public static IOptions<SparqlEndpointOptions> GetMoqOptions()
        {
            return Options.Create(new SparqlEndpointOptions() { EndpointUrl = new Uri("https://xtest.vse.cz/sparql") });
        }


        public static Guid GetUserId { get; set; } = Guid.Parse("52d742a9-9240-4542-bdfa-64bfe3f979b9");

        public static IMemoryCache GetMemoryCache()
        {
            var memoryCache = Mock.Of<IMemoryCache>();
            var cachEntry = Mock.Of<ICacheEntry>();

            var mockMemoryCache = Mock.Get(memoryCache);
            _ = mockMemoryCache
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(cachEntry);
            return memoryCache;
        }

        public static ApplicationDbContext GetNewTestDbContext()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            OperationalStoreOptions storeOptions = new OperationalStoreOptions();

            IOptions<OperationalStoreOptions> operationalStoreOptions = Options.Create(storeOptions);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            var dbContext = new ApplicationDbContext(options, operationalStoreOptions);

            dbContext.Languages.AddRange(LanguageSeed.GetStaticLanguages());
            dbContext.SaveChanges();

            return dbContext;
        }
    }
}
