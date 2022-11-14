﻿using LinkedLanguages.BL.DTO;
using LinkedLanguages.BL.Exception;
using LinkedLanguages.BL.Query;
using LinkedLanguages.BL.Services;
using LinkedLanguages.BL.SPARQL.Query;
using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedLanguages.BL
{
    public partial class WordPairFacade
    {
        private readonly ApplicationDbContext dbContext;
        private readonly WordPairPump wordPairPump;
        private readonly IAppUserProvider appUserProvider;
        private readonly UnusedUserWordPairsQuery unusedUserWordPairs;
        private readonly ApprovedWordPairsQuery approvedWordPairs;
        private readonly WordDefinitionSparqlQuery wordDefinitionSparqlQuery;

        public WordPairFacade(ApplicationDbContext dbContext,
                              WordPairPump wordPairPump,
                              IAppUserProvider appUserProvider,
                              UnusedUserWordPairsQuery unusedUserWordPairs,
                              ApprovedWordPairsQuery approvedWordPairs,
                              WordDefinitionSparqlQuery wordDefinitionSparqlQuery)
        {
            this.dbContext = dbContext;
            this.wordPairPump = wordPairPump;
            this.appUserProvider = appUserProvider;
            this.unusedUserWordPairs = unusedUserWordPairs;
            this.approvedWordPairs = approvedWordPairs;
            this.wordDefinitionSparqlQuery = wordDefinitionSparqlQuery;
        }

        public async Task<WordPairDefinitonsDto> GetDefinition(Guid wordPairId)
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

            return nextWord == default(WordPairDto) ? throw new WordNotFoundException() : nextWord;
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

        public async Task<WordPairDto> GetTestWordPair()
        {
            string knownLang = appUserProvider.GetUserKnownLanguageCode();
            string unknownLangCode = appUserProvider.GetUserUnknownLanguageCode();

            WordPairDto nextWord = await approvedWordPairs.GetQueryable(knownLang, unknownLangCode)
                .Where(w => !w.Learned)
                .Select(u => new WordPairDto(u.WordPairId, u.WordPair.UnknownWord, ""))
                .FirstOrDefaultAsync();

            return nextWord == default(WordPairDto) ? throw new WordNotFoundException() : nextWord;
        }

        public async Task<IList<WordPairDto>> GetLearnedWordPairs()
        {
            return await dbContext.WordPairToApplicationUsers
                 .AsNoTracking()
                 .Where(a => a.ApplicationUserId == appUserProvider.GetUserId())
                 .Where(a => a.Learned)
                 .Select(a => new WordPairDto(a.WordPairId, a.WordPair.UnknownWord, a.WordPair.KnownWord))
                 .ToArrayAsync();
        }

        public async Task<List<LearnedWordStatisticsDto>> GetLearnedWordStatistics()
        {
            var learnedWordPairs = await dbContext.WordPairToApplicationUsers
                 .AsNoTracking()
                 .Where(a => a.ApplicationUserId == appUserProvider.GetUserId())
                 .Where(a => a.Learned)
                 .Select(a => new { a.WordPair.UnknownLanguage.Name, a.NumberOfFailedSubmissions })
                 .ToListAsync();

            var returnedValue = new List<LearnedWordStatisticsDto>();

            var lanuagesStatistics = learnedWordPairs.GroupBy(a => a.Name);

            foreach (var lang in lanuagesStatistics)
            {
                var list = new List<double>();
                foreach (var item in lang.ToList())
                {
                    double successRate = 0;

                    if (item.NumberOfFailedSubmissions is 0)
                    {
                        successRate = 1;
                    }
                    else if (item.NumberOfFailedSubmissions is -1)
                    {
                        successRate = 0;
                    }
                    else
                    {
                        successRate = 1 / item.NumberOfFailedSubmissions;
                    }

                    list.Add(successRate);
                }

                returnedValue.Add(new LearnedWordStatisticsDto(lang.Key, Convert.ToInt16(Math.Round(list.Average(), 2) * 100), lang.Count()));
            }

            return returnedValue;
        }

        public async Task<List<NotLearnedStatisticsDto>> GetWordStatistics()
        {
            var learnedWordPairs = await dbContext.WordPairToApplicationUsers
                .AsNoTracking()
                .Where(a => a.ApplicationUserId == appUserProvider.GetUserId())
                .Where(a => !a.Learned)
                .Select(a => new
                {
                    UnknownLanguage = a.WordPair.UnknownLanguage.Name,
                    KnownLanguage = a.WordPair.KnownLanguage.Name
                })
                .ToListAsync();

            return learnedWordPairs
                .GroupBy(a => "Known: " + a.KnownLanguage + " | Unknown: " + a.UnknownLanguage)
                .Select(a => new NotLearnedStatisticsDto(a.Key, a.Count()))
                .ToList();
        }
    }
}
