using LinkedLanguages.BL.SPARQL.Base;
using LinkedLanguages.DAL.Models;
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
                                                       int page,
                                                       int itemsOnPage);
    public class WordPairsSparqlQuery : SparqlQueryBase<List<WordPair>, WordPairParameterDto>
    {
        public WordPairsSparqlQuery(IOptions<SparqlEndpointOptions> options) : base(options)
        {
        }

        public override string CommandText { get; set; } = string.Concat("SELECT DISTINCT ?myLanguageLabel ?myWord ?foreignWord ?foreignWordLabel ",
                                                                          EtyTreeEtymologyCommandText.GetTwoWayLanguageWhereUnionCommandText());

        protected override void SetQueryParams(SparqlParameterizedString queryString, WordPairParameterDto param)
        {
            queryString.CommandText += $" LIMIT {param.itemsOnPage} OFFSET {param.page * param.itemsOnPage}";

            queryString.SetLiteral("myLang", param.knownLangCode);
            queryString.SetLiteral("forLang", param.unknownLangCode);
        }

        protected override List<WordPair> ParseResult(SparqlResultSet resultSet, WordPairParameterDto param)
        {
            var results = new List<WordPair>();

            var trippleCollection = resultSet.Results.ToArray();
            foreach (var t in trippleCollection)
            {
                var fw = t.GetLiteral("foreignWordLabel");
                var mw = t.GetLiteral("myLanguageLabel");

                var furi = t.GetLiteral("foreignWord");
                var muri = t.GetLiteral("myWord");

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
