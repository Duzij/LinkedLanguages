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
                WordPairDto word = await wordPairFacade.GetTestWordPair();
                return Ok(word);
            }
            catch (WordNotFoundException)
            {
                return NotFound("Word does not exist.");
            }
        }

        [HttpGet("learned")]
        public async Task<IActionResult> GetLearnedWordPairs()
        {
            IList<WordPairDto> words = await wordPairFacade.GetLearnedWordPairs();
            return Ok(words);
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetTestProgress()
        {
            await testWordPairFacade.ResetTestProgressAsync();
            return Ok();
        }

        [HttpPost("reveal")]
        public async Task<IActionResult> RevealTestWordPair(SubmitWordDto submitWord)
        {
            try
            {
                var word = await testWordPairFacade.RevealTestWordPair(submitWord);
                logger.LogInformation($"Submitted word {word.UnknownWord} is revealed.");
                return Ok(word.KnownWord);
            }
            catch (SubmittedWordIncorrectException)
            {
                return BadRequest("Submitted word is not correct.");
            }
            catch (WordNotFoundException)
            {
                return NotFound("Word does not exist.");
            }
        }

        [HttpPost()]
        public async Task<IActionResult> SubmitTestWordPair(SubmitWordDto submitWord)
        {
            try
            {
                await testWordPairFacade.SubmitTestWordPair(submitWord);
                logger.LogInformation("Submitted word is correct.");
                return Ok();
            }
            catch (SubmittedWordIncorrectException)
            {
                return BadRequest("Submitted word is not correct.");
            }
            catch (WordNotFoundException)
            {
                return NotFound("Word does not exist.");
            }
        }
    }
}
