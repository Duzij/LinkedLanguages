using LinkedLanguages.BL;
using LinkedLanguages.BL.DTO;
using LinkedLanguages.BL.Query;
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
        private WordPairsUserQuery wordPairsUserQuery;

        [SetUp]
        public void Setup()
        {
            dbContext = GetNewTestDbContext();
            Guid wpId = Guid.NewGuid();

            List<WordPair> wordPairs = new List<WordPair>
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

            List<WordPairToApplicationUser> usedWordPairs = new List<WordPairToApplicationUser>
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
            appUserProvider.Setup(a => a.GetUserKnownLanguageCode()).Returns("eng");
            wordPairsUserQuery = new WordPairsUserQuery(dbContext, appUserProvider.Object);
        }


        [Test]
        public void WordPairsUserQuery()
        {
            List<WordPairToApplicationUser> unusedUserWordPairs = wordPairsUserQuery.GetQueryable("eng", "lat").ToList();
            Assert.That(unusedUserWordPairs.Count, Is.EqualTo(1));
        }

        [Test]
        public void UnusedUserWordPairsQuery()
        {
            UnusedUserWordPairsQuery unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, wordPairsUserQuery);

            List<WordPair> unusedUserWordPairs = unusedUserWordPairsQuery.GetQueryable("eng", "lat").ToList();
            Assert.That(unusedUserWordPairs.Count, Is.EqualTo(0));
        }


        /// <summary>
        /// Three words are retrieved and approved.
        /// Forth word is pumped automatically
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task ApproveThreeWordPairs()
        {
            UnusedUserWordPairsQuery unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, wordPairsUserQuery);

            WordPairPump wordPairPump = new WordPairPump(new WordPairsSparqlQuery(GetMoqOptions()), unusedUserWordPairsQuery, dbContext);

            WordPairFacade facade = new WordPairFacade(dbContext, wordPairPump, appUserProvider.Object, unusedUserWordPairsQuery, wordPairsUserQuery);

            WordPairDto firstWordPair = await facade.GetNextWord(LanguageSeed.LatinLanguageId);
            Assert.NotNull(firstWordPair);
            await facade.Approve(firstWordPair.Id);

            WordPairDto secondWordPair = await facade.GetNextWord(LanguageSeed.LatinLanguageId);
            Assert.That(secondWordPair.UnknownWord, Is.Not.SameAs(firstWordPair.UnknownWord));
            await facade.Approve(secondWordPair.Id);

            WordPairDto thirdWordPair = await facade.GetNextWord(LanguageSeed.LatinLanguageId);
            Assert.That(thirdWordPair.UnknownWord, Is.Not.SameAs(secondWordPair.UnknownWord));
            await facade.Approve(thirdWordPair.Id);

            WordPairDto forthWordPair = await facade.GetNextWord(LanguageSeed.LatinLanguageId);
            Assert.That(forthWordPair.UnknownWord, Is.Not.SameAs(thirdWordPair.UnknownWord));
            await facade.Approve(forthWordPair.Id);

            Assert.That(dbContext.WordPairs.Count(), Is.EqualTo(7));
        }
    }
}
