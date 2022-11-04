using LinkedLanguages.BL.Query;
using LinkedLanguages.BL.Services;
using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;
using Microsoft.Extensions.Logging;
using Moq;

using NUnit.Framework;

using System;
using System.Linq;
using System.Threading.Tasks;

using static LinkedLanguages.Tests.Helpers.TestServices;

namespace LinkedLanguages.Tests.UseCasesTests
{

    public class WordPairPumpTests
    {
        private WordPair wordPair;
        private ApplicationDbContext dbContext;
        private Mock<IAppUserProvider> appUserProvider;
        private WordPairsUserQuery approvedQuery;

        public Guid TestUserId { get; set; } = Guid.Parse("52d742a9-9240-4542-bdfa-64bfe3f979b9");

        [SetUp]
        public void Setup()
        {
            dbContext = GetNewTestDbContext();
            wordPair = new WordPair
            {
                Id = Guid.Parse("fe5de3d6-f804-4740-b8ce-c760320ee8a2"),
                KnownLanguageId = LanguageSeed.EnglishLanguageId,
                UnknownLanguageId = LanguageSeed.LatinLanguageId,
                KnownWord = "testEnglish",
                UnknownWord = "testLatin",
                KnownLanguageCode = "eng",
                UnknownLanguageCode = "lat"
            };

            dbContext.WordPairs.Add(wordPair);
            dbContext.SaveChanges();
            appUserProvider = new Mock<IAppUserProvider>();
            appUserProvider.Setup(a => a.GetUserId()).Returns(TestUserId);
            approvedQuery = new WordPairsUserQuery(dbContext, appUserProvider.Object);
        }

        [Test]
        public async Task PumpNotPerformedIfNotNeeded()
        {
            UnusedUserWordPairsQuery unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, approvedQuery);
            var transliteratedWordParisQuery = new TransliteratedWordParisQuery(dbContext);

            WordPairPump wordPairPump = new WordPairPump(GetTestWordPairsSparqlQuery(), GetTestWordSeeAlsoLinkSparqlQuery(), unusedUserWordPairsQuery, transliteratedWordParisQuery, dbContext, new Mock<ILogger<WordPairPump>>().Object);

            await wordPairPump.Pump("eng", "lat");

            Assert.That(dbContext.WordPairs.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task PumpPerformedWhenDbEmpty()
        {
            dbContext.WordPairs.RemoveRange(dbContext.WordPairs);
            dbContext.SaveChanges();

            UnusedUserWordPairsQuery unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, approvedQuery);
            var transliteratedWordParisQuery = new TransliteratedWordParisQuery(dbContext);

            WordPairPump wordPairPump = new WordPairPump(GetTestWordPairsSparqlQuery(), GetTestWordSeeAlsoLinkSparqlQuery(), unusedUserWordPairsQuery, transliteratedWordParisQuery, dbContext, new Mock<ILogger<WordPairPump>>().Object);
            await wordPairPump.Pump("eng", "lat");

            Assert.That(dbContext.WordPairs.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task PumpPerformedWhenAllWordsRejected()
        {
            //on the first page, some words are ignored
            dbContext.LanguageOffsets.Add(new() { Id = Guid.NewGuid(), Key = "eng-lat", PageNumer = 2 });

            dbContext.WordPairToApplicationUsers.Add(new()
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = GetUserId,
                Rejected = true,
                WordPairId = wordPair.Id,
                WordPair = wordPair
            });
            await dbContext.SaveChangesAsync();

            UnusedUserWordPairsQuery unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, approvedQuery);
            var transliteratedWordParisQuery = new TransliteratedWordParisQuery(dbContext);

            WordPairPump wordPairPump = new WordPairPump(GetTestWordPairsSparqlQuery(), GetTestWordSeeAlsoLinkSparqlQuery(), unusedUserWordPairsQuery, transliteratedWordParisQuery, dbContext, new Mock<ILogger<WordPairPump>>().Object);
            await wordPairPump.Pump("eng", "lat");

            Assert.That(dbContext.WordPairs.Count(), Is.EqualTo(2));
        }
    }
}
