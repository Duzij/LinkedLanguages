using LinkedLanguages.BL;
using LinkedLanguages.BL.Services;
using LinkedLanguages.BL.SPARQL.Query;
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
        private ApplicationDbContext dbContext;
        private Mock<IAppUserProvider> appUserProvider;
        private ApprovedWordPairsQuery approvedQuery;

        public Guid TestUserId { get; set; } = Guid.Parse("52d742a9-9240-4542-bdfa-64bfe3f979b9");

        [SetUp]
        public void Setup()
        {
            dbContext = GetNewTestDbContext();
            var wordPairs = new List<WordPair>
                {
                    new WordPair {
                        Id = Guid.NewGuid(),
                        KnownLanguageId = LanguageSeed.EnglishLanguageId,
                        UnknownLanguageId = LanguageSeed.LatinLanguageId,
                        KnownWord = "testEnglish",
                        UnknownWord = "testLatin",
                        KnownLanguageCode = "eng",
                        UnknownLanguageCode = "lat"
                    },
                };

            dbContext.WordPairs.AddRange(wordPairs);
            _ = dbContext.SaveChanges();
            appUserProvider = new Mock<IAppUserProvider>();
            _ = appUserProvider.Setup(a => a.GetUserId()).Returns(TestUserId);
            approvedQuery = new ApprovedWordPairsQuery(dbContext, appUserProvider.Object);
        }

        [Test]
        public async Task PumpNotPerformedIfNotNeeded()
        {
            var unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, approvedQuery);

            var wordPairPump = new WordPairPump(new WordPairsSparqlQuery(GetMoqOptions()), GetMemoryCache(), unusedUserWordPairsQuery, dbContext);

            await wordPairPump.Pump("eng", "lat");

            Assert.That(dbContext.WordPairs.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task PumpPerformedWhenDbEmpty()
        {
            var unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, approvedQuery);

            var wordPairPump = new WordPairPump(new WordPairsSparqlQuery(GetMoqOptions()), GetMemoryCache(), unusedUserWordPairsQuery, dbContext);
            await wordPairPump.Pump("eng", "lat");

            Assert.That(dbContext.WordPairs.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task PumpPerformedWhenAllWordsRejected()
        {
            var appUserProvider = new Mock<IAppUserProvider>();
            var unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, approvedQuery);

            var wordPairPump = new WordPairPump(new WordPairsSparqlQuery(GetMoqOptions()), GetMemoryCache(), unusedUserWordPairsQuery, dbContext);
            await wordPairPump.Pump("eng", "lat");

            Assert.That(dbContext.WordPairs.Count(), Is.EqualTo(3));
        }
    }
}
