using LinkedLanguages.DAL.Models;

using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;

using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace LinkedLanguages.BL.SPARQL
{
    public class SparqlPairsQuery
    {
        private readonly SparqlEndpointOptions options;

        public SparqlPairsQuery(IOptions<SparqlEndpointOptions> options)
        {
            this.options = options.Value;
        }

        public List<WordPair> Execute(string knownCode,
                                      Guid knownLangId,
                                      string unknownLangCode,
                                      Guid unknownLangId,
                                      int page,
                                      int itemsOnPage)
        {
            var results = new List<WordPair>();

            var endpoint = new SparqlRemoteEndpoint(new Uri(options.EndpointUrl));

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlParameterizedString queryString = new SparqlParameterizedString();

            queryString.Namespaces.AddNamespace("ety", new Uri("http://etytree-virtuoso.wmflabs.org/dbnaryetymology#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/2000/01/rdf-schema#"));

            queryString.CommandText = @"SELECT DISTINCT  ?myLanguageLabel ?myWord ?foreignWord ?foreignWordLabel";
            queryString.CommandText += EtyTreeEtymologyQuery.GetTwoWayLanguageWhereUnionQuery();

            queryString.CommandText += $"LIMIT {itemsOnPage} OFFSET {page * itemsOnPage}";

            queryString.SetLiteral("myLang", knownCode);
            queryString.SetLiteral("forLang", unknownLangCode);

            SparqlQuery query = parser.ParseFromString(queryString);
            var resultSet = endpoint.QueryWithResultSet(query.ToString());
            if (resultSet is SparqlResultSet)
            {
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
                        KnownLanguageId = knownLangId,
                        KnownLanguage = knownCode,
                        UnknownLanguageId = unknownLangId,
                        UnknownLanguageCode = unknownLangCode,
                        KnownWordUri = muri,
                        UnknownWordUri = furi
                    });
                }
            }

            return results;
        }
    }
}
