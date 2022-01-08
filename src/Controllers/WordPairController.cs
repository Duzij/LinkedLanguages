using LinkedLanguages.BL;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Security.Claims;
using System.Threading.Tasks;

using static LinkedLanguages.BL.WordPairFacade;

namespace LinkedLanguages.Controllers
{
    [Authorize]
    [ApiController]
    [Route("wordpair")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class WordPairController : ControllerBase
    {
        private readonly ILogger<WordPairController> logger;
        private readonly WordPairFacade wordPairFacade;

        public WordPairController(ILogger<WordPairController> logger, WordPairFacade wordPairFacade)
        {
            this.logger = logger;
            this.wordPairFacade = wordPairFacade;
        }

        [HttpGet("get/{langCode}")]
        public WordPairDTO GetWordPair(string langCode)
        {
            var word = wordPairFacade.GetNextWord(langCode);
            return word;
        }

        [HttpPost("approve/{id:guid}")]
        public async Task ApproveWordPair(Guid wordPairId)
        {
            logger.LogInformation("Word approved");
            await wordPairFacade.Approve(wordPairId);
        }

        [HttpPost("decline/{id:guid}")]
        public async Task DeclineWordPair(Guid wordPairId)
        {
            logger.LogInformation("Word declined");
            await wordPairFacade.Decline(wordPairId);
        }
    }
}
