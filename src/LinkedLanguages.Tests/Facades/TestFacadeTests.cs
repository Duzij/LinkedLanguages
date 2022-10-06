using LinkedLanguages.BL;
using LinkedLanguages.BL.DTO;
using LinkedLanguages.BL.Exception;
using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;
using Moq;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using static LinkedLanguages.Tests.Helpers.TestServices;


namespace LinkedLanguages.Tests.Facades
{
    public class TestFacadeTests
    {
        private Mock<IAppUserProvider> appUserProvider;
        private ApplicationDbContext dbContext;
        private Guid wpId;
        private TestWordPairFacade testWordPairFacade;

        [SetUp]
        public void Setup()
        {
            dbContext = GetNewTestDbContext();
            wpId = Guid.NewGuid();

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
            _ = dbContext.SaveChanges();

            appUserProvider = new Mock<IAppUserProvider>();
            _ = appUserProvider.Setup(a => a.GetUserId()).Returns(GetUserId);
            _ = appUserProvider.Setup(a => a.GetUserKnownLanguageCode()).Returns("eng");

            testWordPairFacade = new TestWordPairFacade(dbContext);
        }

        [Test]
        public void SubmitWordTestSuccessfully()
        {
            Assert.DoesNotThrow(() => testWordPairFacade.SubmitTestWordPair(new BL.DTO.SubmitWordDto(wpId, "known")));
            Assert.DoesNotThrow(() => testWordPairFacade.SubmitTestWordPair(new SubmitWordDto(wpId, "KNOWN")));
            Assert.DoesNotThrow(() => testWordPairFacade.SubmitTestWordPair(new SubmitWordDto(wpId, " kNoWn ")));
        }

        [Test]
        public void SubmitWordTestUnsuccessfully()
        {
            Assert.Throws<SubmittedWordIncorrectException>(() =>
            {
                testWordPairFacade.SubmitTestWordPair(new SubmitWordDto(wpId, "unknow0"));
            });

        }
    }
}
