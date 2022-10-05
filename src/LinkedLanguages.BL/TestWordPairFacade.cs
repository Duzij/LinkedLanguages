using LinkedLanguages.BL.Exception;
using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedLanguages.BL
{
    public class TestWordPairFacade
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IAppUserProvider appUserProvider;

        public TestWordPairFacade(ApplicationDbContext dbContext,
                              IAppUserProvider appUserProvider)
        {
            this.dbContext = dbContext;
            this.appUserProvider = appUserProvider;
        }

        public async Task SubmitTestWordPair(Guid wordPairId, string submitedWord)
        {
            Guard.ThrowExceptionIfNotExists(dbContext, wordPairId);

            var wp = dbContext.WordPairs.First(wp => wp.Id == wordPairId);

            if (string.Compare(wp.KnownWord, submitedWord, ignoreCase: true) < 0)
            {
                throw new SubmittedWordIncorrectException();
            }
        }
    }
}
