using LinkedLanguages.BL;
using LinkedLanguages.BL.Exception;
using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;
using Moq;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static LinkedLanguages.Tests.Helpers.TestServices;


namespace LinkedLanguages.Tests.Facades
{
    public class TestFacadeTests
    {
        private Mock<IAppUserProvider> appUserProvider;
        private ApplicationDbContext dbContext;
        private Guid wpId;
        private TestWordPairFacade testWordPairFacade;
        private readonly ApprovedWordPairsQuery approvedWordPairsQuery;

        [SetUp]
        public void Setup()
        {
            dbContext = GetNewTestDbContext();
            wpId = Guid.NewGuid();

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

            testWordPairFacade = new TestWordPairFacade(dbContext, appUserProvider.Object);
        }

        [Test]
        public async Task SubmitWordTestSuccessfully()
        {
            Assert.DoesNotThrowAsync(async () => await testWordPairFacade.SubmitTestWordPair(wpId, "known"));
            Assert.DoesNotThrowAsync(async () => await testWordPairFacade.SubmitTestWordPair(wpId, "KNOWN"));
            Assert.DoesNotThrowAsync(async () => await testWordPairFacade.SubmitTestWordPair(wpId, " kNoWn "));
        }

        [Test]
        public async Task SubmitWordTestUnsuccessfully()
        {
            Assert.ThrowsAsync<SubmittedWordIncorrectException>(async () =>
            {
                await testWordPairFacade.SubmitTestWordPair(wpId, "unknow0");
            });

        }
    }
}
