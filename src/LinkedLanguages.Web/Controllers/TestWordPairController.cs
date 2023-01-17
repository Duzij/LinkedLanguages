using LinkedLanguages.BL.DTO;
using LinkedLanguages.BL.Exception;
using LinkedLanguages.BL.Facades;
using LinkedLanguages.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkedLanguages.Web.Controllers
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
                WordPairDto word = await testWordPairFacade.GetTestWordPair();
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
            IList<WordPairDto> words = await testWordPairFacade.GetLearnedWordPairs();
            return Ok(words);
        }

        [HttpGet("allstatistics")]
        public async Task<IActionResult> GetWordStatistics()
        {
            List<NotLearnedStatisticsDto> statistics = await testWordPairFacade.GetApprovedWordStatisticsExceptSelected();
            return Ok(statistics);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetLearnedWordStatistics()
        {
            List<LearnedWordStatisticsDto> statistics = await testWordPairFacade.GetLearnedWordStatistics();
            return Ok(statistics);
        }

        [HttpPost("reveal")]
        public async Task<IActionResult> RevealTestWordPair(SubmitWordDto submitWord)
        {
            try
            {
                WordPairDto word = await testWordPairFacade.RevealTestWordPair(submitWord);
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
