using LinkedLanguages.BL.DTO;
using LinkedLanguages.BL.Services;
using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedLanguages.BL
{
    public partial class WordPairFacade
    {
        private readonly ApplicationDbContext dbContext;
        private readonly WordPairPump wordPairPump;
        private readonly IAppUserProvider appUserProvider;
        private readonly UnusedUserWordPairsQuery unusedUserWordPairs;

        public WordPairFacade(ApplicationDbContext dbContext,
                              WordPairPump wordPairPump,
                              IAppUserProvider appUserProvider,
                              UnusedUserWordPairsQuery unusedUserWordPairs)
        {
            this.dbContext = dbContext;
            this.wordPairPump = wordPairPump;
            this.appUserProvider = appUserProvider;
            this.unusedUserWordPairs = unusedUserWordPairs;
        }

        public async Task<WordPairDto> GetNextWord(Guid unknownLangId)
        {
            string knownLang = appUserProvider.GetUserKnownLanguage();

            var unknownLangCode = dbContext.Languages.First(a => a.Id == unknownLangId).Code;

            await wordPairPump.Pump(knownLang, unknownLangCode);

            var nextWord = unusedUserWordPairs.GetQueryable(knownLang, unknownLangCode)
                .Select(u => new WordPairDto(u.Id, u.UnknownWord, u.KnownWord))
                .FirstOrDefault();

            return nextWord == default(WordPairDto) ? throw new WordNotFoundException() : nextWord;
        }

        public async Task Approve(Guid wordPairId)
        {
            ThrowExceptionIfNotExists(wordPairId);

            var wp = new WordPairToApplicationUser()
            {
                ApplicationUserId = appUserProvider.GetUserId(),
                Id = Guid.NewGuid(),
                WordPairId = wordPairId
            };

            _ = await dbContext.WordPairToApplicationUsers.AddAsync(wp);
            _ = await dbContext.SaveChangesAsync();
        }

        private void ThrowExceptionIfNotExists(Guid wordPairId)
        {
            if (!dbContext.WordPairs.Any(wp => wp.Id == wordPairId))
            {
                throw new InvalidOperationException($"Word pair with id {wordPairId} not found");
            }
        }

        public async Task Reject(Guid wordPairId)
        {
            var wp = new WordPairToApplicationUser()
            {
                ApplicationUserId = appUserProvider.GetUserId(),
                Id = Guid.NewGuid(),
                WordPairId = wordPairId,
                Rejected = true
            };

            _ = await dbContext.WordPairToApplicationUsers.AddAsync(wp);
            _ = await dbContext.SaveChangesAsync();
        }
    }
}
