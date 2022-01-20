using LinkedLanguages.DAL;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedLanguages.BL
{
    public partial class WordPairFacade
    {
        private readonly ApplicationDbContext appDbContext;
        private readonly WordPairPump wordPairPump;
        private readonly IAppUserProvider appUserProvider;
        private readonly UnusedUserWordPairsQuery unusedUserWordPairs;

        public WordPairFacade(ApplicationDbContext appContext,
                              WordPairPump wordPairPump,
                              IAppUserProvider appUserProvider,
                              UnusedUserWordPairsQuery unusedUserWordPairs)
        {
            this.appDbContext = appContext;
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

        public Task Approve(Guid wordPairId)
        {
            throw new NotImplementedException();
        }

        public Task Decline(Guid wordPairId)
        {
            throw new NotImplementedException();
        }
    }
}
