using LinkedLanguages.BL.DTO;
using LinkedLanguages.BL.SPARQL.Query;
using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedLanguages.BL
{
    public class LanguageFacade
    {
        private readonly ApplicationDbContext appContext;
        private readonly IAppUserProvider appUserProvider;
        private readonly PairsStatisticsSparqlQuery sparqlPairsStatisticsQuery;
        private readonly IMemoryCache memoryCache;

        public LanguageFacade(ApplicationDbContext appContext,
                              IAppUserProvider appUserProvider,
                              PairsStatisticsSparqlQuery sparqlPairsStatisticsQuery,
                              IMemoryCache memoryCache
        )
        {
            this.appContext = appContext;
            this.appUserProvider = appUserProvider;
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

        public async Task<UserProfileDto> GetUserProfileAsync()
        {
            var userId = appUserProvider.GetUserId();

            return new UserProfileDto()
            {
                UserId = userId,
                KnownLanguages = await appContext.KnownLanguageToUsers
                .AsNoTracking()
                .Where(a => a.ApplicationUserId == userId)
                .Select(kl => new LanguageDto
                {
                    Value = kl.LanguageId,
                    Label = kl.Language.Name
                })
                .ToListAsync(),

                UnknownLanguages = await appContext.UnknownLanguageToUsers
                .AsNoTracking()
                .Where(a => a.ApplicationUserId == userId)
                .Select(kl => new LanguageDto
                {
                    Value = kl.LanguageId,
                    Label = kl.Language.Name
                })
                .ToListAsync(),
            };
        }

        public int GetCountOfPredicates(UserProfileDto statisticsDto)
        {
            var unknownCode = appContext.Languages
                .AsNoTracking()
                .First(a => a.Id == statisticsDto.UnknownLanguages.First().Value)
                .Code;

            var knownCode = appContext.Languages
                .AsNoTracking()
                .First(a => a.Id == statisticsDto.KnownLanguages.First().Value)
                .Code;

            var key = $"{knownCode}-{unknownCode}";
            if (memoryCache.TryGetValue(key, out int value))
            {
                return value;
            }
            else
            {
                var queryValue = sparqlPairsStatisticsQuery.Execute(new LanguageCodesDto(knownCode, unknownCode));
                memoryCache.Set(key, queryValue);
                return queryValue;
            }
        }

        public async Task SaveUserProfile(UserProfileDto userProfile)
        {
            var userId = appUserProvider.GetUserId();

            using (appContext)
            {
                var savedKnown = await appContext.KnownLanguageToUsers
                    .Where(k => k.ApplicationUserId == userId)
                    .ToListAsync();

                var savedUnknown = await appContext.UnknownLanguageToUsers
                    .Where(u => u.ApplicationUserId == userId)
                    .ToListAsync();

                if (savedKnown.Any())
                {
                    appContext.RemoveRange(savedKnown);
                }
                if (savedUnknown.Any())
                {
                    appContext.RemoveRange(savedUnknown);
                }

                foreach (var lang in userProfile.KnownLanguages)
                {
                    await appContext.KnownLanguageToUsers
                        .AddAsync(new DAL.Models.KnownLanguageToUser()
                        {
                            ApplicationUserId = userId,
                            LanguageId = lang.Value
                        });
                }

                foreach (var lang in userProfile.UnknownLanguages)
                {
                    await appContext.UnknownLanguageToUsers
                        .AddAsync(new DAL.Models.UnknownLanguageToUser()
                        {
                            ApplicationUserId = userId,
                            LanguageId = lang.Value
                        });
                }
                await appContext.SaveChangesAsync();
            }
        }
    }
}
