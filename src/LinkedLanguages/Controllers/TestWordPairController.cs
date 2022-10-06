using LinkedLanguages.BL;
using LinkedLanguages.BL.DTO;
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
        private readonly WordPairFacade wordPairFacade;

        public TestWordPairController(ILogger<WordPairController> logger, TestWordPairFacade testWordPairFacade, WordPairFacade wordPairFacade)
        {
            this.logger = logger;
            this.testWordPairFacade = testWordPairFacade;
            this.wordPairFacade = wordPairFacade;
        }


        [HttpGet()]
        public async Task<IActionResult> GetTestWordPair()
        {
            try
            {
                BL.DTO.WordPairDto word = await wordPairFacade.GetTestWordPair();
                return Ok(word);
            }
            catch (WordNotFoundException)
            {
                return NotFound("Word does not exist.");
            }
        }

        [HttpPost()]
        public IActionResult SubmitTestWordPair(SubmitWordDto submitWord)
        {
            try
            {
                testWordPairFacade.SubmitTestWordPair(submitWord);
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
