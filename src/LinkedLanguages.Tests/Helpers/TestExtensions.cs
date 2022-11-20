using Duende.IdentityServer.EntityFramework.Options;

using LinkedLanguages.BL.SPARQL;
using LinkedLanguages.BL.SPARQL.Query;
using LinkedLanguages.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;
using System;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace LinkedLanguages.Tests.Helpers
{
    public static class TestServices
    {
        public static PairsStatisticsSparqlQuery GetTestPairsStatisticsSparqlQuery() => new PairsStatisticsSparqlQuery(GetMoqOptions(), new Logger<PairsStatisticsSparqlQuery>(GetTestLoggerFactory()));
        public static WordDefinitionSparqlQuery GetTestWordDefinitionSparqlQuery() => new WordDefinitionSparqlQuery(GetMoqOptions(), new Logger<WordDefinitionSparqlQuery>(GetTestLoggerFactory()));
        public static WordPairsSparqlQuery GetTestWordPairsSparqlQuery() => new WordPairsSparqlQuery(GetMoqOptions(), new Logger<WordPairsSparqlQuery>(GetTestLoggerFactory()));
        public static WordSeeAlsoLinkSparqlQuery GetTestWordSeeAlsoLinkSparqlQuery() => new WordSeeAlsoLinkSparqlQuery(GetMoqOptions(), new Logger<WordSeeAlsoLinkSparqlQuery>(GetTestLoggerFactory()));

        public static ILoggerFactory GetTestLoggerFactory()
        {
            return LoggerFactory.Create(builder =>
             {
                 builder.AddConsole();
             });
        }

        public static IOptions<SparqlEndpointOptions> GetMoqOptions()
        {
            return Options.Create(new SparqlEndpointOptions() { EndpointUrl = new Uri("https://xtest.vse.cz/sparql"), ItemsOnPage = 3 });
        }

        public static ILogger GetMoqLogger()
        {
            return new Mock<ILogger>().Object;
        }

        public static Guid GetUserId { get; set; } = Guid.Parse("52d742a9-9240-4542-bdfa-64bfe3f979b9");

        public static IMemoryCache GetMemoryCache()
        {
            var memoryCache = Mock.Of<IMemoryCache>();
            var cachEntry = Mock.Of<ICacheEntry>();

            var mockMemoryCache = Mock.Get(memoryCache);
            mockMemoryCache
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
