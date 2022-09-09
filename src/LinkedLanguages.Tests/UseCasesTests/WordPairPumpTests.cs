using LinkedLanguages.BL;
using LinkedLanguages.BL.Services;
using LinkedLanguages.BL.SPARQL;
using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;

using Moq;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static LinkedLanguages.Tests.Helpers.TestServices;

namespace LinkedLanguages.Tests.UseCasesTests
{

    public class WordPairPumpTests
    {
        public Guid TestUserId { get; set; } = Guid.Parse("52d742a9-9240-4542-bdfa-64bfe3f979b9");

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
                        UnknownWord = "testLatin",
                        KnownLanguage = "eng",
                        UnknownLanguageCode = "lat"
                    },
                };

                dbContext.WordPairs.AddRange(wordPairs);
                dbContext.SaveChanges();

                var appUserProvider = new Mock<IAppUserProvider>();
                appUserProvider.Setup(a => a.GetUserId()).Returns(TestUserId);
                var unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, appUserProvider.Object);

                var wordPairPump = new WordPairPump(new SparqlPairsQuery(GetMoqOptions()), GetMemoryCache(), unusedUserWordPairsQuery, dbContext);

                await wordPairPump.Pump("eng", "lat");

                Assert.That(dbContext.WordPairs.Count(), Is.EqualTo(1));
            }

        }

        [Test]
        public async Task PumpPerformedWhenDbEmpty()
        {
            using (var dbContext = GetNewTestDbContext())
            {
                var appUserProvider = new Mock<IAppUserProvider>();
                var unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, appUserProvider.Object);

                var wordPairPump = new WordPairPump(new SparqlPairsQuery(GetMoqOptions()), GetMemoryCache(), unusedUserWordPairsQuery, dbContext);
                await wordPairPump.Pump("eng", "lat");

                Assert.That(dbContext.WordPairs.Count(), Is.EqualTo(3));
            }
        }

        [Test]
        public async Task PumpPerformedWhenAllWordsRejected()
        {
            using (var dbContext = GetNewTestDbContext())
            {
                var wordPairsToAppUser = new List<WordPairToApplicationUser>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = TestUserId,
                        Rejected = true,
                        WordPairId = Guid.NewGuid()
                    },
                };

                dbContext.WordPairToApplicationUsers.AddRange(wordPairsToAppUser);
                dbContext.SaveChanges();

                var appUserProvider = new Mock<IAppUserProvider>();
                var unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, appUserProvider.Object);

                var wordPairPump = new WordPairPump(new SparqlPairsQuery(GetMoqOptions()), GetMemoryCache(), unusedUserWordPairsQuery, dbContext);
                await wordPairPump.Pump("eng", "lat");

                Assert.That(dbContext.WordPairs.Count(), Is.EqualTo(3));
            }
        }
    }
}
