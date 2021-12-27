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
            var userId = Convert.ToInt32(claimsPrincipal.GetUserId());
            return await languageFacade.GetUserProfileAsync(userId);
        }


        [HttpPost()]
        public async Task SaveUserProfile(UserProfileDto userProfile)
        {
            var userId = Convert.ToInt32(claimsPrincipal.GetUserId());

            userProfile.UserId = userId;

            await languageFacade.SaveUserProfile(userProfile);
        }

    }
}
