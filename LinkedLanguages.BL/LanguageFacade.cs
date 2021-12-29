using LinkedLanguages.Data;

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

        public LanguageFacade(ApplicationDbContext appContext)
        {
            this.appContext = appContext;
        }

        public async Task<List<LanguageDto>> GetLanguages()
        {
            return await appContext.Languages.AsNoTracking().Select(l => new LanguageDto { Id = l.Id, LanguageName = l.Name, Code = l.Code }).ToListAsync();
        }

        public async Task<UserProfileDto> GetUserProfileAsync(string userId)
        {
            return new UserProfileDto()
            {
                UserId = Guid.Parse(userId), 
                KnownLanguages = await appContext.KnownLanguageToUsers.AsNoTracking().Select(kl => new LanguageDto { Id = kl.Id, Code = kl.Language.Code, LanguageName = kl.Language.Name }).ToListAsync(),
                UnknownLanguages = await appContext.UnknownLanguageToUsers.AsNoTracking().Select(kl => new LanguageDto { Id = kl.Id, Code = kl.Language.Code, LanguageName = kl.Language.Name }).ToListAsync(),
            };
        }

        public async Task SaveUserProfile(UserProfileDto userProfile)
        {
            using (appContext)
            {

                var savedKnown = await appContext.KnownLanguageToUsers.Where(k => k.ApplicationUserId == userProfile.UserId).ToListAsync();
                var savedUnknown = await appContext.UnknownLanguageToUsers.Where(u => u.ApplicationUserId == userProfile.UserId).ToListAsync();

                appContext.RemoveRange(savedKnown, savedUnknown);

                foreach (var lang in userProfile.KnownLanguages)
                {
                    await appContext.KnownLanguageToUsers.AddAsync(new DAL.Models.KnownLanguageToUser() { ApplicationUserId = userProfile.UserId, LanguageId = lang.Id });
                }

                foreach (var lang in userProfile.KnownLanguages)
                {
                    await appContext.UnknownLanguageToUsers.AddAsync(new DAL.Models.UnknownLanguageToUser() { ApplicationUserId = userProfile.UserId, LanguageId = lang.Id });
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
        public Guid Id { get; set; }
        public string LanguageName { get; set; }
        public string Code { get; internal set; }
    }
}
