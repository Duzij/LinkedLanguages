using LinkedLanguages.BL.DTO;
using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedLanguages.BL.Facades
{
    public class SetupFacade
    {
        private readonly ApplicationDbContext appContext;
        private readonly IAppUserProvider appUserProvider;

        public SetupFacade(ApplicationDbContext appContext,
                              IAppUserProvider appUserProvider
        )
        {
            this.appContext = appContext;
            this.appUserProvider = appUserProvider;
        }

        public async Task<UserProfileDto> GetUserProfileAsync()
        {
            System.Guid userId = appUserProvider.GetUserId();

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

        public async Task SaveUserProfile(UserProfileDto userProfile)
        {
            System.Guid userId = appUserProvider.GetUserId();

            using (appContext)
            {
                System.Collections.Generic.List<DAL.Models.KnownLanguageToUser> savedKnown = await appContext.KnownLanguageToUsers
                    .Where(k => k.ApplicationUserId == userId)
                    .ToListAsync();

                System.Collections.Generic.List<DAL.Models.UnknownLanguageToUser> savedUnknown = await appContext.UnknownLanguageToUsers
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

                foreach (LanguageDto lang in userProfile.KnownLanguages)
                {
                    await appContext.KnownLanguageToUsers
                       .AddAsync(new DAL.Models.KnownLanguageToUser()
                       {
                           ApplicationUserId = userId,
                           LanguageId = lang.Value
                       });
                }

                foreach (LanguageDto lang in userProfile.UnknownLanguages)
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
