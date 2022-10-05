﻿using LinkedLanguages.BL;
using LinkedLanguages.BL.Exception;
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

        public WordPairController(ILogger<WordPairController> logger, WordPairFacade wordPairFacade)
        {
            this.logger = logger;
            this.wordPairFacade = wordPairFacade;
        }

        [HttpGet("get/test/{languageId}")]
        public async Task<IActionResult> GetTestWordPair(Guid languageId)
        {
            try
            {
                var word = await wordPairFacade.GetTestWordPair(languageId);
                return Ok(word);
            }
            catch (WordNotFoundException)
            {
                return NotFound("Word does not exist.");
            }
        }

        [HttpGet("get/{languageId}")]
        public async Task<IActionResult> GetWordPair(Guid languageId)
        {
            try
            {
                var word = await wordPairFacade.GetNextWord(languageId);
                return Ok(word);
            }
            catch (WordNotFoundException)
            {
                return NotFound("Word does not exist.");
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