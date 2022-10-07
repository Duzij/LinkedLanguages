using LinkedLanguages.BL.DTO;
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
            var userLearnedWordPairs = dbContext.WordPairToApplicationUsers
              .Where(uwp => uwp.ApplicationUserId == appUserProvider.GetUserId());

            await userLearnedWordPairs.ForEachAsync(a => a.Larned = false);

            await dbContext.SaveChangesAsync();
        }

        public async Task<WordPairDto> RevealTestWordPair(SubmitWordDto submitWord)
        {
            return await dbContext.WordPairToApplicationUsers.AsNoTracking()
                .Where(a => a.WordPairId == submitWord.WordPairId)
                .Select(a => new WordPairDto(a.WordPair.Id, a.WordPair.UnknownWord, a.WordPair.KnownWord))
                .FirstAsync();
        }

        public async Task SubmitTestWordPair(SubmitWordDto submitWord)
        {
            Guard.ThrowExceptionIfNotExists(dbContext, submitWord.WordPairId);

            WordPair wp = dbContext.WordPairs.First(wp => wp.Id == submitWord.WordPairId);

            if (string.Compare(wp.KnownWord, submitWord.SubmitedWord.Trim(), ignoreCase: true) != 0)
            {
                throw new SubmittedWordIncorrectException();
            }

            var userWordPair = dbContext.WordPairToApplicationUsers
                .Where(uwp => uwp.ApplicationUserId == appUserProvider.GetUserId())
                .FirstOrDefault(uwp => uwp.WordPairId == wp.Id);

            userWordPair.Larned = true;

            await dbContext.SaveChangesAsync();
        }
    }
}
