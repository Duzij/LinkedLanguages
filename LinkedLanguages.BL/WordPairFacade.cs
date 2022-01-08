using LinkedLanguages.DAL;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedLanguages.BL
{
    public class WordPairFacade
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

        public WordPairDTO GetNextWord(string unknownLangCode)
        {
            string knownLang = appUserProvider.GetUserKnownLanguage();
            wordPairPump.Pump(knownLang, unknownLangCode);

            return unusedUserWordPairs.GetQueryable(unknownLangCode)
                .Select(u => new WordPairDTO() { 
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

        public class WordPairDTO
        {
            public Guid Id { get; set; }
            public string UnknownWord { get; set; }
            public string KnownWord { get; set; }
        }
    }
}
