using LinkedLanguages.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LinkedLanguages.BL.Query
{
    public readonly record struct TransliteratedWordsDto(string KnownWordTransliterated, string UnknownWordTransliterated);

    public class TransliteratedWordParisQuery
    {
        private readonly ApplicationDbContext appDbContext;

        public TransliteratedWordParisQuery(ApplicationDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public IQueryable<TransliteratedWordsDto> GetQueryable(Guid unknownLanguageId, Guid knownLangugageId)
        {
            return appDbContext.WordPairs
                      .AsNoTracking()
                      .Where(a => a.UnknownLanguageId == unknownLanguageId && a.KnownLanguageId == knownLangugageId)
                      .Select(wp => new TransliteratedWordsDto(wp.KnownWordTransliterated, wp.UnknownWordTransliterated));
        }
    }
}
