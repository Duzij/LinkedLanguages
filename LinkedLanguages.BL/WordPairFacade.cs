using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<WordPairDto> GetNextWord(string unknownLangCode)
        {
            string knownLang = appUserProvider.GetUserKnownLanguage();
            await wordPairPump.Pump(knownLang, unknownLangCode);

            return unusedUserWordPairs.GetQueryable(unknownLangCode)
                .Select(u => new WordPairDto() { 
                    Id = u.Id,
                    UnknownWord = u.UnknownWord,
                    KnownWord = u.KnownWord
                })
                .First();
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

            await dbContext.WordPairToApplicationUsers.AddAsync(wp);
            await dbContext.SaveChangesAsync();
        }

        private void ThrowExceptionIfNotExists(Guid wordPairId)
        {
            if (!dbContext.WordPairs.Any(wp => wp.Id == wordPairId))
            {
                throw new InvalidOperationException($"Word pair with id {wordPairId} not found");
            }
        }

        public async Task Decline(Guid wordPairId)
        {
            var wp = new WordPairToApplicationUser()
            {
                ApplicationUserId = appUserProvider.GetUserId(),
                Id = Guid.NewGuid(),
                WordPairId = wordPairId,
                Rejected = true
            };

            await dbContext.WordPairToApplicationUsers.AddAsync(wp);
            await dbContext.SaveChangesAsync();
        }
    }
}
