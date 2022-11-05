using LinkedLanguages.BL.SPARQL.Base;
using LinkedLanguages.DAL.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using VDS.RDF.Query;

namespace LinkedLanguages.BL.SPARQL.Query
{
    public readonly record struct WordPairParameterDto(string knownLangCode,
                                                       Guid knownLangId,
                                                       string unknownLangCode,
                                                       Guid unknownLangId,
                                                       int page);
    public class WordPairsSparqlQuery : SparqlQueryBase<List<WordPair>, WordPairParameterDto>
    {
        public WordPairsSparqlQuery(IOptions<SparqlEndpointOptions> options, ILogger<SparqlQueryBase<List<WordPair>, WordPairParameterDto>> logger) : base(options, logger)
        {
        }

        public override string CommandText { get; set; } = string.Concat("SELECT ?myLanguageLabel ?myLanguageWord ?foreignLanguageWord ?foreignWordLabel",
                                                                          EtyTreeEtymologyCommandText.GetTwoWayLanguageWhereUnionCommandText());

        protected override void SetQueryParams(SparqlParameterizedString queryString, WordPairParameterDto param)
        {
            queryString.CommandText += $" LIMIT {options.ItemsOnPage} OFFSET {param.page * options.ItemsOnPage}";

            queryString.SetLiteral("myLang", param.knownLangCode);
            queryString.SetLiteral("forLang", param.unknownLangCode);
        }

        protected override List<WordPair> ParseResult(SparqlResultSet resultSet, WordPairParameterDto param)
        {
            List<WordPair> results = new List<WordPair>();

            SparqlResult[] trippleCollection = resultSet.Results.ToArray();
            foreach (SparqlResult t in trippleCollection)
            {
                string fw = t.GetLiteral("foreignWordLabel");
                string mw = t.GetLiteral("myLanguageLabel");

                string furi = t.GetLiteral("foreignLanguageWord");
                string muri = t.GetLiteral("myLanguageWord");

                results.Add(new WordPair()
                {
                    Id = Guid.NewGuid(),
                    KnownWord = mw,
                    UnknownWord = fw,
                    KnownLanguageId = param.knownLangId,
                    KnownLanguageCode = param.knownLangCode,
                    UnknownLanguageId = param.unknownLangId,
                    UnknownLanguageCode = param.unknownLangCode,
                    KnownWordUri = muri,
                    UnknownWordUri = furi
                });
            }

            return results;
        }
    }
}
