using LinkedLanguages.BL.DTO;
using LinkedLanguages.BL.SPARQL.Query;
using LinkedLanguages.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedLanguages.BL.Facades
{
    public class LanguageFacade
    {
        private readonly ApplicationDbContext appContext;
        private readonly PairsStatisticsSparqlQuery sparqlPairsStatisticsQuery;
        private readonly IMemoryCache memoryCache;

        public LanguageFacade(ApplicationDbContext appContext,
                              PairsStatisticsSparqlQuery sparqlPairsStatisticsQuery,
                              IMemoryCache memoryCache
        )
        {
            this.appContext = appContext;
            this.sparqlPairsStatisticsQuery = sparqlPairsStatisticsQuery;
            this.memoryCache = memoryCache;
        }

        public async Task<List<LanguageDto>> GetLanguages()
        {
            return await appContext.Languages
                .AsNoTracking()
                .Select(l => new LanguageDto
                {
                    Value = l.Id,
                    Label = l.Name
                })
                .ToListAsync();
        }

        public int GetCountOfLanguagesPredicates(UserProfileDto statisticsDto)
        {
            string unknownCode = appContext.Languages
                .AsNoTracking()
                .First(a => a.Id == statisticsDto.UnknownLanguages.First().Value)
                .Code;

            string knownCode = appContext.Languages
                .AsNoTracking()
                .First(a => a.Id == statisticsDto.KnownLanguages.First().Value)
            .Code;

            string key = $"{knownCode}-{unknownCode}";
            if (memoryCache.TryGetValue(key, out int value))
            {
                return value;
            }
            else
            {
                int queryValue = sparqlPairsStatisticsQuery.Execute(new LanguageCodesDto(knownCode, unknownCode));
                memoryCache.Set(key, queryValue);
                return queryValue;
            }
        }

    }
}
