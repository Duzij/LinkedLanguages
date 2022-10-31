﻿using LinkedLanguages.BL.SPARQL.Query;
using LinkedLanguages.BL.User;
using LinkedLanguages.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedLanguages.BL
{
    public class SeeAlsoLinksFacade
    {
        private readonly WordSeeAlsoLinkSparqlQuery wordSeeAlsoLinkSparqlQuery;
        private readonly ApplicationDbContext dbContext;
        private readonly IAppUserProvider appUserProvider;

        public SeeAlsoLinksFacade(WordSeeAlsoLinkSparqlQuery wordSeeAlsoLinkSparqlQuery, ApplicationDbContext dbContext)
        {
            this.wordSeeAlsoLinkSparqlQuery = wordSeeAlsoLinkSparqlQuery;
            this.dbContext = dbContext;
            appUserProvider = appUserProvider;
        }

        public async Task<WordPairSeeAlsoLinksDto> GetLinks(Guid wordPairId)
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

            return new WordPairSeeAlsoLinksDto(
                wordSeeAlsoLinkSparqlQuery.ExecuteDontThrow(new WordUriDto(wordPair.KnownWordUri)),
                 wordSeeAlsoLinkSparqlQuery.ExecuteDontThrow(new WordUriDto(wordPair.UnknownWordUri))
                 );
        }
    }
}
