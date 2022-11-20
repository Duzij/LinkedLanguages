using LinkedLanguages.BL.DTO;
using LinkedLanguages.BL.Facades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkedLanguages.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("profile")]
    public class ProfileController : ControllerBase
    {
        private readonly LanguageFacade languageFacade;
        private readonly SetupFacade setupFacade;
        private readonly ILogger<ProfileController> logger;

        public ProfileController(LanguageFacade languageFacade, SetupFacade setupFacade, ILogger<ProfileController> logger)
        {
            this.languageFacade = languageFacade;
            this.setupFacade = setupFacade;
            this.logger = logger;
        }

        [HttpGet()]
        public async Task<UserProfileDto> GetUserProfile()
        {
            UserProfileDto profile = await setupFacade.GetUserProfileAsync();
            return profile;
        }

        [HttpPost()]
        public async Task<UserProfileDto> SaveUserProfile(UserProfileDto userProfile)
        {
            logger.LogInformation("Saving user profile");
            await setupFacade.SaveUserProfile(userProfile);

            return userProfile;
        }

    }
}
