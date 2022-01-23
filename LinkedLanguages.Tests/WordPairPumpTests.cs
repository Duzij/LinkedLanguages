using IdentityServer4.EntityFramework.Options;

using LinkedLanguages.BL;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Moq;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static LinkedLanguages.Tests.TestServices;

namespace LinkedLanguages.Tests
{

    public class WordPairPumpTests
    {
        public Guid TestUserTest { get; set; } = Guid.Parse("52d742a9-9240-4542-bdfa-64bfe3f979b9");

        [Test]
        public async Task PumpNotPerformedIfNotNeeded()
        {
            using (var dbContext = GetNewTestDbContext())
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

                dbContext.WordPairs.AddRange(wordPairs);
                dbContext.SaveChanges();

                var appUserProvider = new Mock<IAppUserProvider>();
                appUserProvider.Setup(a => a.GetUserId()).Returns(TestUserTest);
                var unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, appUserProvider.Object);

                var wordPairPump = new WordPairPump(new SparqlPairsQuery(), GetMemoryCache(), unusedUserWordPairsQuery, dbContext);

                await wordPairPump.Pump("eng", "lat");

                Assert.AreEqual(1, dbContext.WordPairs.Count());
            }

        }

        [Test]
        public async Task PumpPerformedWhenDbEmpty()
        {
            using (var dbContext = GetNewTestDbContext())
            {
                var appUserProvider = new Mock<IAppUserProvider>();
                var unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, appUserProvider.Object);

                var wordPairPump = new WordPairPump(new SparqlPairsQuery(), TestServices.GetMemoryCache(), unusedUserWordPairsQuery, dbContext);
                await wordPairPump.Pump("eng", "lat");

                Assert.AreEqual(3, dbContext.WordPairs.Count());
            }
        }

        [Test]
        public async Task PumpPerformedWhenAllWordsRejected()
        {
            using (var dbContext = GetNewTestDbContext())
            {
                var wordPairsToAppUser = new List<WordPairToApplicationUser>
                {
                    new WordPairToApplicationUser {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = TestUserTest,
                        Rejected = true,
                        WordPairId = Guid.NewGuid()
                    },
                };

                dbContext.WordPairToApplicationUsers.AddRange(wordPairsToAppUser);
                dbContext.SaveChanges();

                var appUserProvider = new Mock<IAppUserProvider>();
                var unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, appUserProvider.Object);

                var wordPairPump = new WordPairPump(new SparqlPairsQuery(), TestServices.GetMemoryCache(), unusedUserWordPairsQuery, dbContext);
                await wordPairPump.Pump("eng", "lat");

                Assert.AreEqual(3, dbContext.WordPairs.Count());
            }
        }
    }
}
