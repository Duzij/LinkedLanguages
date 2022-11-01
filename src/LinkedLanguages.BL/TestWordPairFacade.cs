﻿using LinkedLanguages.BL.DTO;
using LinkedLanguages.BL.Exception;
using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedLanguages.BL
{
    public class TestWordPairFacade
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IAppUserProvider appUserProvider;

        public TestWordPairFacade(ApplicationDbContext dbContext, IAppUserProvider appUserProvider)
        {
            this.dbContext = dbContext;
            this.appUserProvider = appUserProvider;
        }

        public async Task ResetTestProgressAsync()
        {
            IQueryable<WordPairToApplicationUser> userLearnedWordPairs = dbContext.WordPairToApplicationUsers
              .Where(uwp => uwp.ApplicationUserId == appUserProvider.GetUserId());

            await userLearnedWordPairs.ForEachAsync(a => a.Learned = false);

            await dbContext.SaveChangesAsync();
        }

        public async Task<WordPairDto> RevealTestWordPair(SubmitWordDto submitWord)
        {
            WordPairToApplicationUser userWordPair = dbContext.WordPairToApplicationUsers
             .Where(uwp => uwp.ApplicationUserId == appUserProvider.GetUserId())
             .FirstOrDefault(uwp => uwp.WordPairId == submitWord.WordPairId);

            userWordPair.NumberOfFailedSubmissions = -1;

            await dbContext.SaveChangesAsync();

            return await dbContext.WordPairToApplicationUsers.AsNoTracking()
                .Where(a => a.WordPairId == submitWord.WordPairId)
                .Select(a => new WordPairDto(a.WordPair.Id, a.WordPair.UnknownWord, a.WordPair.KnownWord))
                .FirstAsync();
        }

        public async Task SubmitTestWordPair(SubmitWordDto submitWord)
        {
            Guard.ThrowExceptionIfNotExists(dbContext, submitWord.WordPairId);

            WordPair wp = dbContext.WordPairs.First(wp => wp.Id == submitWord.WordPairId);

            WordPairToApplicationUser userWordPair = dbContext.WordPairToApplicationUsers
              .Where(uwp => uwp.ApplicationUserId == appUserProvider.GetUserId())
              .FirstOrDefault(uwp => uwp.WordPairId == wp.Id);

            if (string.Compare(wp.KnownWord, submitWord.SubmitedWord.Trim(), ignoreCase: true) != 0)
            {
                userWordPair.NumberOfFailedSubmissions++;
                await dbContext.SaveChangesAsync();

                throw new SubmittedWordIncorrectException();
            }
            else
            {
                userWordPair.Learned = true;
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
