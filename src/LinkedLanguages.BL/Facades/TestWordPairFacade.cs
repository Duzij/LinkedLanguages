using LinkedLanguages.BL.DTO;
using LinkedLanguages.BL.Exception;
using LinkedLanguages.BL.Query;
using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedLanguages.BL.Facades
{
    public class TestWordPairFacade
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ApprovedWordPairsQuery approvedWordPairs;
        private readonly IAppUserProvider appUserProvider;

        public TestWordPairFacade(ApplicationDbContext dbContext, ApprovedWordPairsQuery approvedWordPairs, IAppUserProvider appUserProvider)
        {
            this.dbContext = dbContext;
            this.approvedWordPairs = approvedWordPairs;
            this.appUserProvider = appUserProvider;
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

        public async Task<WordPairDto> GetTestWordPair()
        {
            string knownLang = appUserProvider.GetUserKnownLanguageCode();
            string unknownLangCode = appUserProvider.GetUserUnknownLanguageCode();

            WordPairDto nextWord = await approvedWordPairs.GetQueryable(knownLang, unknownLangCode)
                .Where(w => !w.Learned)
                .Select(u => new WordPairDto(u.WordPairId, u.WordPair.UnknownWord, ""))
                .FirstOrDefaultAsync();

            return nextWord == default ? throw new WordNotFoundException() : nextWord;
        }

        public async Task<IList<WordPairDto>> GetLearnedWordPairs()
        {
            return await dbContext.WordPairToApplicationUsers
                 .AsNoTracking()
                 .Where(a => a.ApplicationUserId == appUserProvider.GetUserId())
                 .Where(a => a.Learned)
                 .Select(a => new WordPairDto(a.WordPairId, a.WordPair.UnknownWord, a.WordPair.KnownWord))
                 .ToArrayAsync();
        }

        public async Task<List<LearnedWordStatisticsDto>> GetLearnedWordStatistics()
        {
            var learnedWordPairs = await dbContext.WordPairToApplicationUsers
                 .AsNoTracking()
                 .Where(a => a.ApplicationUserId == appUserProvider.GetUserId())
                 .Where(a => a.Learned)
                 .Select(a => new { a.WordPair.UnknownLanguage.Name, a.NumberOfFailedSubmissions })
                 .ToListAsync();

            List<LearnedWordStatisticsDto> returnedValue = new List<LearnedWordStatisticsDto>();

            var lanuagesStatistics = learnedWordPairs.GroupBy(a => a.Name);

            foreach (var lang in lanuagesStatistics)
            {
                List<double> list = new List<double>();
                foreach (var item in lang.ToList())
                {
                    double successRate = item.NumberOfFailedSubmissions is 0 ? 1 : item.NumberOfFailedSubmissions is -1 ? 0 : 1 / item.NumberOfFailedSubmissions;
                    list.Add(successRate);
                }

                returnedValue.Add(new LearnedWordStatisticsDto(lang.Key, Convert.ToInt16(Math.Round(list.Average(), 2) * 100), lang.Count()));
            }

            return returnedValue;
        }

        public async Task<List<NotLearnedStatisticsDto>> GetApprovedWordStatisticsExceptSelected()
        {
            var learnedWordPairs = await dbContext.WordPairToApplicationUsers
                .AsNoTracking()
                .Where(a => a.ApplicationUserId == appUserProvider.GetUserId())
                .Where(a => !a.Learned && !a.Rejected)
                .Where(a => a.WordPair.UnknownLanguageCode != appUserProvider.GetUserUnknownLanguageCode() || a.WordPair.KnownLanguageCode != appUserProvider.GetUserKnownLanguageCode())
                .Select(a => new
                {
                    UnknownLanguage = a.WordPair.UnknownLanguage.Name,
                    KnownLanguage = a.WordPair.KnownLanguage.Name
                })
                .ToListAsync();

            return learnedWordPairs
                .GroupBy(a => "Known: " + a.KnownLanguage + " | Unknown: " + a.UnknownLanguage)
                .Select(a => new NotLearnedStatisticsDto(a.Key, a.Count()))
                .ToList();
        }

    }
}
