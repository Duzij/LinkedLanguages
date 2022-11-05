using LinkedLanguages.BL;
using LinkedLanguages.BL.DTO;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkedLanguages.Controllers
{
    [Authorize]
    [ApiController]
    [Route("profile")]
    public class ProfileController : ControllerBase
    {
        private readonly LanguageFacade languageFacade;
        private readonly ILogger<ProfileController> logger;

        public ProfileController(LanguageFacade languageFacade, ILogger<ProfileController> logger)
        {
            this.languageFacade = languageFacade;
            this.logger = logger;
        }

        [HttpGet()]
        public async Task<UserProfileDto> GetUserProfile()
        {
            var profile = await languageFacade.GetUserProfileAsync();
            return profile;
        }


        [HttpPost()]
        public async Task<UserProfileDto> SaveUserProfile(UserProfileDto userProfile)
        {
            logger.LogInformation("Saving user profile");
            await languageFacade.SaveUserProfile(userProfile);

            return userProfile;
        }

    }
}
