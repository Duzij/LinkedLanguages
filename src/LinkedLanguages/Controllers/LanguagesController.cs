using LinkedLanguages.BL;
using LinkedLanguages.BL.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public int GetLanguageStatistics(UserProfileDto statisticsDto)
        {
            logger.LogInformation("Getting statistics");
            if (statisticsDto.UnknownLanguages.Any() && statisticsDto.KnownLanguages.Any())
            {
                var count = languageFacade.GetCountOfPredicates(statisticsDto);
                return count;
            }
            return 0;
        }
    }
}
