using LinkedLanguages.BL;
using LinkedLanguages.BL.DTO;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace LinkedLanguages.Controllers
{
    [Authorize]
    [ApiController]
    [Route("profile")]
    [Consumes("application/json")]
    [Produces("application/json")]
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
