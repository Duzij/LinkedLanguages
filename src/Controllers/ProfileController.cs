using LinkedLanguages.BL;
using LinkedLanguages.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Security.Claims;
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
        private readonly ClaimsPrincipal claimsPrincipal;
        private readonly ILogger<ProfileController> logger;

        public ProfileController(LanguageFacade languageFacade, ClaimsPrincipal claimsPrincipal, ILogger<ProfileController> logger)
        {
            this.languageFacade = languageFacade;
            this.claimsPrincipal = claimsPrincipal;
            this.logger = logger;
        }

        [HttpGet()]
        public async Task<UserProfileDto> GetUserProfile()
        {
            var userId = claimsPrincipal.GetUserId();
             var profile = await languageFacade.GetUserProfileAsync(userId);
            return profile;
        }


        [HttpPost()]
        public async Task<UserProfileDto> SaveUserProfile(UserProfileDto userProfile)
        {
            userProfile.UserId = Guid.Parse(claimsPrincipal.GetUserId());
            logger.LogInformation("Saving user profile");
            await languageFacade.SaveUserProfile(userProfile);

            return userProfile;
        }

    }
}
