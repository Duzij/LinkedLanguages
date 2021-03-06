using LinkedLanguages.BL;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;

using Moq;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static LinkedLanguages.Tests.TestServices;

namespace LinkedLanguages.Tests
{
    public class WordPairOperationsTests
    {
        [Test]
        public void UnusedUserWordPairsQuery()
        {
            using (var dbContext = GetNewTestDbContext())
            {
                var wpId = Guid.NewGuid();

                var wordPairs = new List<WordPair>
                {
                    new WordPair {
                        Id = wpId,
                        KnownLanguageId = LanguageSeed.EnglishLanguageId,
                        UnknownLanguageId = LanguageSeed.LatinLanguageId,
                        UnknownLanguageCode = "lat",
                        KnownLanguage = "eng",
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

                var appUserProvider = new Mock<IAppUserProvider>();
                appUserProvider.Setup(a => a.GetUserId()).Returns(GetUserId);
                appUserProvider.Setup(a => a.GetUserKnownLanguage()).Returns("eng");

                var unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, appUserProvider.Object);

                var unusedUserWordPairs = unusedUserWordPairsQuery.GetQueryable("lat").ToList();
                Assert.AreEqual(0, unusedUserWordPairs.Count);
            }
        }


        /// <summary>
        /// Three words are retrieved and approved.
        /// Forth word is pumped automatically
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task ApproveThreeWordPair()
        {
            using (var dbContext = GetNewTestDbContext())
            {
                var appUserProvider = new Mock<IAppUserProvider>();
                appUserProvider.Setup(a => a.GetUserId()).Returns(GetUserId);
                appUserProvider.Setup(a => a.GetUserKnownLanguage()).Returns("eng");

                var unusedUserWordPairsQuery = new UnusedUserWordPairsQuery(dbContext, appUserProvider.Object);

                var wordPairPump = new WordPairPump(new SparqlPairsQuery(GetMoqOptions()), GetMemoryCache(), unusedUserWordPairsQuery, dbContext);

                var facade = new WordPairFacade(dbContext, wordPairPump, appUserProvider.Object, unusedUserWordPairsQuery);


                var firstWordPair = await facade.GetNextWord(LanguageSeed.LatinLanguageId);
                Assert.NotNull(firstWordPair);
                await facade.Approve(firstWordPair.Id);

                var secondWordPair = await facade.GetNextWord(LanguageSeed.LatinLanguageId);
                Assert.AreNotSame(firstWordPair.UnknownWord, secondWordPair.UnknownWord);
                await facade.Approve(secondWordPair.Id);

                var thirdWordPair = await facade.GetNextWord(LanguageSeed.LatinLanguageId);
                Assert.AreNotSame(secondWordPair.UnknownWord, thirdWordPair.UnknownWord);
                await facade.Approve(thirdWordPair.Id);

                var forthWordPair = await facade.GetNextWord(LanguageSeed.LatinLanguageId);
                Assert.AreNotSame(thirdWordPair.UnknownWord, forthWordPair.UnknownWord);
                await facade.Approve(forthWordPair.Id);

                Assert.AreEqual(6, dbContext.WordPairs.Count());
            }

        }
    }
}
