using LinkedLanguages.Data;

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

        public List<LangugageDto> GetLanguages()
        {
            return new List<LangugageDto>();
        }

        public UserProfileDto GetUserProfile(int userId)
        {
            return new UserProfileDto();
        }

        public async Task SaveUserProfile(UserProfileDto userProfile)
        {
            foreach (var lang in userProfile.KnownLanguage)
            {
                await appContext.KnownLanguageToUsers.AddAsync(new DAL.Models.KnownLanguageToUser() { ApplicationUserId = userProfile.UserId, LanguageId = lang.Id });
            }

            foreach (var lang in userProfile.KnownLanguage)
            {
                await appContext.UnknownLanguageToUsers.AddAsync(new DAL.Models.UnknownLanguageToUser() { ApplicationUserId = userProfile.UserId, LanguageId = lang.Id });
            }
        }
    }

    public class UserProfileDto
    {
        public int UserId { get; set; }
        public List<LangugageDto> KnownLanguage { get; set; }
        public List<LangugageDto> UnknownLanguage { get; set; }
    }


    public class LangugageDto
    {
        public int Id { get; set; }
        public string LanguageName { get; set; }

    }
}
