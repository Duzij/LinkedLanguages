using IdentityServer4.EntityFramework.Options;

using LinkedLanguages.BL;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Moq;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedLanguages.Tests
{
    public class WordPairPumpTests
    {
        [Test]
        public async Task PumpNotPerformedIfNotNeeded()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            OperationalStoreOptions storeOptions = new OperationalStoreOptions
            {
                //populate needed members
            };

            IOptions<OperationalStoreOptions> operationalStoreOptions = Options.Create(storeOptions);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDbContext")
            .Options;

            using (var dbContext = new ApplicationDbContext(options, operationalStoreOptions))
            {
                var wordPairs = new List<WordPair>
                {
                    new WordPair {
                        Id = Guid.NewGuid(),
                        KnownLanguageId = LanguageSeed.EnglishLanguageId,
                        UnknownLanguageId = LanguageSeed.LatinLanguageId,
                        KnownWord = "testEnglish",
                        UnknownWord = "testLatin"
                    },
                };

                dbContext.UnusedWordPairs.AddRange(wordPairs);
                dbContext.SaveChanges();

                var memoryCache = Mock.Of<IMemoryCache>();
                var cachEntry = Mock.Of<ICacheEntry>();

                var mockMemoryCache = Mock.Get(memoryCache);
                mockMemoryCache
                    .Setup(m => m.CreateEntry(It.IsAny<object>()))
                    .Returns(cachEntry);

                var appUserProvider = new Mock<IAppUserProvider>();
                var unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, appUserProvider.Object);

                var wordPairPump = new WordPairPump(new SparqlPairsQuery(), memoryCache, unusedUserWordPairsQuery, dbContext);

                await wordPairPump.Pump("eng", "ll");

                Assert.AreEqual(1, dbContext.UnusedWordPairs.Count());
            }

        }

        [Test]
        public async Task PumpPerformedWhenDbEmpty()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            OperationalStoreOptions storeOptions = new OperationalStoreOptions
            {
                //populate needed members
            };

            IOptions<OperationalStoreOptions> operationalStoreOptions = Options.Create(storeOptions);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDbContext")
            .Options;

            using (var dbContext = new ApplicationDbContext(options, operationalStoreOptions))
            {
                var mockCache = new Mock<IMemoryCache>();
                var appUserProvider = new Mock<IAppUserProvider>();
                var unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, appUserProvider.Object);

                var wordPairPump = new WordPairPump(new SparqlPairsQuery(), mockCache.Object, unusedUserWordPairsQuery, dbContext);
                await wordPairPump.Pump("eng", "ll");

                Assert.AreEqual(3, dbContext.UnusedWordPairs.Count());
            }
        }

        [Test]
        public void PumpPerformedWhenAllWordsUsed()
        {
            
        }
    }
}
