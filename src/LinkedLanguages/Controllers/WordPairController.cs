using LinkedLanguages.BL;
using LinkedLanguages.BL.DTO;
using LinkedLanguages.BL.Exception;
using LinkedLanguages.BL.SPARQL.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkedLanguages.Controllers
{
    [Authorize]
    [ApiController]
    [Route("wordpair")]
    public class WordPairController : Controller
    {
        private readonly ILogger<WordPairController> logger;
        private readonly WordPairFacade wordPairFacade;
        private readonly SeeAlsoLinksFacade seeAlsoLinksFacade;

        public WordPairController(ILogger<WordPairController> logger, WordPairFacade wordPairFacade, SeeAlsoLinksFacade seeAlsoLinksFacade)
        {
            this.logger = logger;
            this.wordPairFacade = wordPairFacade;
            this.seeAlsoLinksFacade = seeAlsoLinksFacade;
        }

        [HttpGet("link/{wordPairId}")]
        public async Task<IActionResult> GetLinks(Guid wordPairId)
        {
            try
            {
                var word = await seeAlsoLinksFacade.GetLinks(wordPairId);
                return Ok(word);
            }
            catch (WordNotFoundException)
            {
                return NotFound("Word does not exist.");
            }
        }

        [HttpGet("get/{languageId}")]
        public async Task<IActionResult> GetNextWord(Guid languageId)
        {
            try
            {
                WordPairDto word = await wordPairFacade.GetNextWord(languageId);
                return Ok(word);
            }
            catch (WordNotFoundException)
            {
                return NotFound("Word does not exist.");
            }
        }

        [HttpGet("definiton/{wordPairId}")]
        public async Task<IActionResult> GetDefinition(Guid wordPairId)
        {
            try
            {
                WordPairDefinitonsDto definitions = await wordPairFacade.GetDefinition(wordPairId);
                return Ok(definitions);
            }
            catch (WordNotFoundException)
            {
                return NotFound("Definition does not exist.");
            }
        }

        [HttpPost("approve/{wordPairId}")]
        public async Task ApproveWordPair(Guid wordPairId)
        {
            logger.LogInformation("Word approved");
            await wordPairFacade.Approve(wordPairId);
        }

        [HttpPost("reject/{wordPairId}")]
        public async Task RejectWordPair(Guid wordPairId)
        {
            logger.LogInformation("Word rejected");
            await wordPairFacade.Reject(wordPairId);
        }
    }
}
