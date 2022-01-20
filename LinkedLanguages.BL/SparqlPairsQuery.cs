using LinkedLanguages.DAL.Models;

using System;
using System.Collections.Generic;

using VDS.RDF;
using VDS.RDF.Nodes;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

using static LinkedLanguages.BL.WordPairFacade;

namespace LinkedLanguages.BL
{
    public class SparqlPairsQuery
    {
        public SparqlPairsQuery()
        {

        }

        public List<WordPair> Execute(string myLangCode, string foreignLangCode, int page, int itemsOnPage)
        {
            var results = new List<WordPair>();

            var endpoint = new SparqlRemoteEndpoint(new Uri("https://etytree-virtuoso.wmflabs.org/sparql/"));

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlParameterizedString queryString = new SparqlParameterizedString();

            queryString.Namespaces.AddNamespace("ety", new Uri("http://etytree-virtuoso.wmflabs.org/dbnaryetymology#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/2000/01/rdf-schema#"));

            queryString.CommandText = @"
            SELECT DISTINCT  ?myLanguageLabel ?myWord ?foreignWord ?foreignWordLabel
            WHERE
              { ?name  a ety:EtymologyEntry ;
                       ety:etymologicallyRelatedTo  ?foreignWord ;
                       rdf:label ?myLanguageLabel .
                ?foreignWord     rdf:label ?foreignWordLabel
                FILTER langMatches(lang(?myLanguageLabel), @myLang)
                FILTER langMatches(lang(?foreignWordLabel), @forLang)
              }  ";

            queryString.CommandText += $"LIMIT {itemsOnPage} OFFSET {page * itemsOnPage}";

            queryString.SetLiteral("myLang", myLangCode);
            queryString.SetLiteral("forLang", foreignLangCode);

            SparqlQuery query = parser.ParseFromString(queryString);
            var resultSet = endpoint.QueryWithResultSet(query.ToString());
            if (resultSet is SparqlResultSet)
            {
                var trippleCollection = resultSet.Results.ToArray();
                foreach (var t in trippleCollection)
                {
                    var fw = t.GetLiteral("foreignWordLabel");
                    var mw = t.GetLiteral("myLanguageLabel");

                    results.Add(new WordPair()
                    {
                        Id = Guid.NewGuid(),
                        KnownWord = mw,
                        UnknownWord = fw
                    });
                }
            }

            return results;
        }
    }
}
