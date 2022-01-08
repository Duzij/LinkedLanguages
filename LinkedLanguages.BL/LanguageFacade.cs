using LinkedLanguages.DAL;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedLanguages.BL
{
    public class LanguageFacade
    {
        private readonly ApplicationDbContext appContext;
        private readonly IAppUserProvider appUserProvider;

        public LanguageFacade(ApplicationDbContext appContext, IAppUserProvider appUserProvider)
        {
            this.appContext = appContext;
            this.appUserProvider = appUserProvider;
        }

        public async Task<List<LanguageDto>> GetLanguages()
        {
            return await appContext.Languages
                .AsNoTracking()
                .Select(l => new LanguageDto {
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
                .Select(kl => new LanguageDto {
                    Value = kl.LanguageId,
                    Label = kl.Language.Name
                })
                .ToListAsync(),

                UnknownLanguages = await appContext.UnknownLanguageToUsers
                .AsNoTracking()
                .Select(kl => new LanguageDto {
                    Value = kl.LanguageId,
                    Label = kl.Language.Name
                })
                .ToListAsync(),
            };
        }

        public async Task SaveUserProfile(UserProfileDto userProfile)
        {
            using (appContext)
            {
                var savedKnown = await appContext.KnownLanguageToUsers.Where(k => k.ApplicationUserId == userProfile.UserId).ToListAsync();
                var savedUnknown = await appContext.UnknownLanguageToUsers.Where(u => u.ApplicationUserId == userProfile.UserId).ToListAsync();

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
                    await appContext.KnownLanguageToUsers.AddAsync(new DAL.Models.KnownLanguageToUser() { ApplicationUserId = userProfile.UserId, LanguageId = lang.Value });
                }

                foreach (var lang in userProfile.UnknownLanguages)
                {
                    await appContext.UnknownLanguageToUsers.AddAsync(new DAL.Models.UnknownLanguageToUser() { ApplicationUserId = userProfile.UserId, LanguageId = lang.Value });
                }
                await appContext.SaveChangesAsync();
            }
        }
    }

    public class UserProfileDto
    {
        public Guid UserId { get; set; }
        public List<LanguageDto> KnownLanguages { get; set; }
        public List<LanguageDto> UnknownLanguages { get; set; }
    }


    public class LanguageDto
    {
        public Guid Value { get; set; }
        public string Label { get; set; }
    }
}
