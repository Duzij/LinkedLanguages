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
                                                       int page);
    public class WordPairsSparqlQuery : SparqlQueryBase<List<WordPair>, WordPairParameterDto>
    {
        public WordPairsSparqlQuery(IOptions<SparqlEndpointOptions> options) : base(options)
        {
        }

        public override string CommandText { get; set; } = string.Concat("SELECT DISTINCT ?myLanguageLabel ?myWord ?foreignWord ?foreignWordLabel ?unknownSeeAlsoLink ?knownSeeAlsoLink ",
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

                string furi = t.GetLiteral("foreignWord");
                string muri = t.GetLiteral("myWord");

                string knownSeeAlsoLink = t.GetLiteral("knownSeeAlsoLink");
                string unknownSeeAlsoLink = t.GetLiteral("unknownSeeAlsoLink");

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
                    UnknownWordUri = furi,
                    KnownSeeAlsoLink = knownSeeAlsoLink,
                    UnknownSeeAlsoLink = unknownSeeAlsoLink,
                });
            }

            return results;
        }
    }
}
