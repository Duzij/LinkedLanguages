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
    public class WordPairOperationsTests
    {
        private Mock<IAppUserProvider> appUserProvider;
        private ApplicationDbContext dbContext;
        private ApprovedWordPairsQuery approvedWordPairsQuery;

        [SetUp]
        public void Setup()
        {
            dbContext = GetNewTestDbContext();
            var wpId = Guid.NewGuid();

            var wordPairs = new List<WordPair>
                {
                    new WordPair {
                        Id = wpId,
                        KnownLanguageId = LanguageSeed.EnglishLanguageId,
                        UnknownLanguageId = LanguageSeed.LatinLanguageId,
                        UnknownLanguageCode = "lat",
                        KnownLanguageCode = "eng",
                        KnownWord = "known",
                        UnknownWord = "unknown"
                    }
                };

            var usedWordPairs = new List<WordPairToApplicationUser>
                {
                    new WordPairToApplicationUser {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = GetUserId,
                        Rejected = true,
                        WordPairId = wpId
                    }
                };

            dbContext.WordPairToApplicationUsers.AddRange(usedWordPairs);
            dbContext.WordPairs.AddRange(wordPairs);
            dbContext.SaveChanges();

            appUserProvider = new Mock<IAppUserProvider>();
            appUserProvider.Setup(a => a.GetUserId()).Returns(GetUserId);
            appUserProvider.Setup(a => a.GetUserKnownLanguage()).Returns("eng");
        }


        [Test]
        public void ApprovedUserWordPairsQuery()
        {
            approvedWordPairsQuery = new ApprovedWordPairsQuery(dbContext, appUserProvider.Object);

            var unusedUserWordPairs = approvedWordPairsQuery.GetQueryable("eng", "lat").ToList();
            Assert.That(unusedUserWordPairs.Count, Is.EqualTo(1));
        }

        [Test]
        public void UnusedUserWordPairsQuery()
        {
            var unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, approvedWordPairsQuery);

            var unusedUserWordPairs = unusedUserWordPairsQuery.GetQueryable("eng", "lat").ToList();
            Assert.That(unusedUserWordPairs.Count, Is.EqualTo(0));
        }


        /// <summary>
        /// Three words are retrieved and approved.
        /// Forth word is pumped automatically
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task ApproveThreeWordPair()
        {
            var unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, approvedWordPairsQuery);

            var wordPairPump = new WordPairPump(new WordPairsSparqlQuery(GetMoqOptions()), GetMemoryCache(), unusedUserWordPairsQuery, dbContext);

            var facade = new WordPairFacade(dbContext, wordPairPump, appUserProvider.Object, unusedUserWordPairsQuery, approvedWordPairsQuery);

            var firstWordPair = await facade.GetNextWord(LanguageSeed.LatinLanguageId);
            Assert.NotNull(firstWordPair);
            await facade.Approve(firstWordPair.Id);

            var secondWordPair = await facade.GetNextWord(LanguageSeed.LatinLanguageId);
            Assert.That(secondWordPair.UnknownWord, Is.Not.SameAs(firstWordPair.UnknownWord));
            await facade.Approve(secondWordPair.Id);

            var thirdWordPair = await facade.GetNextWord(LanguageSeed.LatinLanguageId);
            Assert.That(thirdWordPair.UnknownWord, Is.Not.SameAs(secondWordPair.UnknownWord));
            await facade.Approve(thirdWordPair.Id);

            var forthWordPair = await facade.GetNextWord(LanguageSeed.LatinLanguageId);
            Assert.That(forthWordPair.UnknownWord, Is.Not.SameAs(thirdWordPair.UnknownWord));
            await facade.Approve(forthWordPair.Id);

            Assert.That(dbContext.WordPairs.Count(), Is.EqualTo(6));
        }
    }
}
