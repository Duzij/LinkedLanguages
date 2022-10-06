using LinkedLanguages.BL.DTO;
using LinkedLanguages.BL.Exception;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;
using System.Linq;

namespace LinkedLanguages.BL
{
    public class TestWordPairFacade
    {
        private readonly ApplicationDbContext dbContext;

        public TestWordPairFacade(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void SubmitTestWordPair(SubmitWordDto submitWord)
        {
            Guard.ThrowExceptionIfNotExists(dbContext, submitWord.WordPairId);

            WordPair wp = dbContext.WordPairs.First(wp => wp.Id == submitWord.WordPairId);

            if (string.Compare(wp.KnownWord, submitWord.SubmitedWord, ignoreCase: true) < 0)
            {
                throw new SubmittedWordIncorrectException();
            }
        }
    }
}
