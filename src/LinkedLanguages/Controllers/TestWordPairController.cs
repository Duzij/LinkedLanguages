using LinkedLanguages.BL;
using LinkedLanguages.BL.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkedLanguages.Controllers
{
    [Authorize]
    [ApiController]
    [Route("testwordpair")]
    public class TestWordPairController : Controller
    {
        private readonly ILogger<WordPairController> logger;
        private readonly TestWordPairFacade testWordPairFacade;

        public TestWordPairController(ILogger<WordPairController> logger, TestWordPairFacade testWordPairFacade)
        {
            this.logger = logger;
            this.testWordPairFacade = testWordPairFacade;
        }

        [HttpGet("get/{languageId}")]
        public async Task<IActionResult> SubmitTestWordPair(Guid wordPairId, string submitedWord)
        {
            try
            {
                await testWordPairFacade.SubmitTestWordPair(wordPairId, submitedWord);
                logger.LogInformation("Submitted word is correct.");
                return Ok();
            }
            catch (SubmittedWordIncorrectException)
            {
                return NotFound("Submitted word is not correct.");
            }
            catch (WordNotFoundException)
            {
                return NotFound("Word does not exist.");
            }
        }
    }
}
