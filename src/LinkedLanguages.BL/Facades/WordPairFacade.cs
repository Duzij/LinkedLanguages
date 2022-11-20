using LinkedLanguages.BL.DTO;
using LinkedLanguages.BL.Exception;
using LinkedLanguages.BL.Query;
using LinkedLanguages.BL.Services;
using LinkedLanguages.BL.SPARQL.Query;
using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedLanguages.BL.Facades
{
    public partial class WordPairFacade
    {
        private readonly ApplicationDbContext dbContext;
        private readonly WordPairPump wordPairPump;
        private readonly IAppUserProvider appUserProvider;
        private readonly UnusedUserWordPairsQuery unusedUserWordPairs;
        private readonly WordDefinitionSparqlQuery wordDefinitionSparqlQuery;

        public WordPairFacade(ApplicationDbContext dbContext,
                              WordPairPump wordPairPump,
                              IAppUserProvider appUserProvider,
                              UnusedUserWordPairsQuery unusedUserWordPairs,
                              WordDefinitionSparqlQuery wordDefinitionSparqlQuery)
        {
            this.dbContext = dbContext;
            this.wordPairPump = wordPairPump;
            this.appUserProvider = appUserProvider;
            this.unusedUserWordPairs = unusedUserWordPairs;
            this.wordDefinitionSparqlQuery = wordDefinitionSparqlQuery;
        }

        public async Task<WordPairDefinitonsDto> GetDefinitions(Guid wordPairId)
        {
            var wordPair = await dbContext.WordPairs
                .AsNoTracking()
                .Where(a => a.Id == wordPairId)
                .Select(a => new
                {
                    a.KnownWordUri,
                    a.UnknownWordUri
                })
                .FirstAsync();

            string[] knownDefinitions;
            string[] unknownDefinitions;

            try
            {
                knownDefinitions = wordDefinitionSparqlQuery.Execute(new WordUriDto(wordPair.KnownWordUri)).ToArray();
            }
            catch (InvalidOperationException)
            {
                knownDefinitions = new string[0];
            }

            try
            {
                unknownDefinitions = wordDefinitionSparqlQuery.Execute(new WordUriDto(wordPair.UnknownWordUri)).ToArray();
            }
            catch (InvalidOperationException)
            {
                unknownDefinitions = new string[0];
            }

            return new WordPairDefinitonsDto
                (
                    knownDefinitions,
                    unknownDefinitions
                );
        }

        public async Task<WordPairDto> GetNextWord(Guid unknownLangId)
        {
            string knownLang = appUserProvider.GetUserKnownLanguageCode();

            string unknownLangCode = dbContext.Languages.First(a => a.Id == unknownLangId).Code;

            try
            {
                await wordPairPump.Pump(knownLang, unknownLangCode);
            }
            catch (InvalidOperationException)
            {
                //Empty SPARQL set is returned, no more words to pump
                throw new WordNotFoundException();
            }

            WordPairDto nextWord = unusedUserWordPairs.GetQueryable(knownLang, unknownLangCode)
                .Select(u => new WordPairDto(u.Id, u.UnknownWord, u.KnownWord))
                .FirstOrDefault();

            return nextWord == default ? throw new WordNotFoundException() : nextWord;
        }

        public async Task Approve(Guid wordPairId)
        {
            Guard.ThrowExceptionIfNotExists(dbContext, wordPairId);

            WordPairToApplicationUser wp = new WordPairToApplicationUser()
            {
                ApplicationUserId = appUserProvider.GetUserId(),
                Id = Guid.NewGuid(),
                WordPairId = wordPairId
            };

            await dbContext.WordPairToApplicationUsers.AddAsync(wp);
            await dbContext.SaveChangesAsync();
        }

        public async Task Reject(Guid wordPairId)
        {
            WordPairToApplicationUser wp = new WordPairToApplicationUser()
            {
                ApplicationUserId = appUserProvider.GetUserId(),
                Id = Guid.NewGuid(),
                WordPairId = wordPairId,
                Rejected = true
            };

            await dbContext.WordPairToApplicationUsers.AddAsync(wp);
            await dbContext.SaveChangesAsync();
        }

    }
}
