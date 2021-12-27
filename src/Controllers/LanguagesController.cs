using LinkedLanguages.BL;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LinkedLanguages.Controllers
{
    [Authorize]
    [ApiController]
    [Route("languages")]
    public class LanguagesController : ControllerBase
    {
        private readonly LanguageFacade languageFacade;
        private readonly ILogger<ProfileController> logger;

        public LanguagesController(LanguageFacade languageFacade, ILogger<ProfileController> logger)
        {
            this.languageFacade = languageFacade;
            this.logger = logger;
        }


        [HttpGet()]
        public async Task<IEnumerable<LanguageDto>> GetUserProfile()
        {
            logger.LogInformation("Getting all languages");
            return await languageFacade.GetLanguages();
        }
    }
}
