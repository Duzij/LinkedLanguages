using LinkedLanguages.BL;
using LinkedLanguages.BL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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
        public async Task<IEnumerable<LanguageDto>> GetAllLanguages()
        {
            logger.LogInformation("Getting all languages");
            return await languageFacade.GetLanguages();
        }

        [HttpPost("statistics")]
        public async Task<int> GetLanguageStatistics(UserProfileDto statisticsDto)
        {
            logger.LogInformation("Getting statistics");
            var count = await languageFacade.GetCountOfPredicates(statisticsDto);
            return count;
        }
    }
}
