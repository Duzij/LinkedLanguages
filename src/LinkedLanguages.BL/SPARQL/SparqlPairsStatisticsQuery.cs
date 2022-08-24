using LinkedLanguages.DAL.Models;

using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;

using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace LinkedLanguages.BL
{
    public class SparqlPairsStatisticsQuery
    {
        private readonly SparqlEndpointOptions options;

        public SparqlPairsStatisticsQuery(IOptions<SparqlEndpointOptions> options)
        {
            this.options = options.Value;
        }

        public int Execute(string knownCode,
                           string unknownLangCode)
        {
            var endpoint = new SparqlRemoteEndpoint(new Uri(options.EndpointUrl));

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlParameterizedString queryString = new SparqlParameterizedString();

            queryString.Namespaces.AddNamespace("ety", new Uri("http://etytree-virtuoso.wmflabs.org/dbnaryetymology#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/2000/01/rdf-schema#"));

            queryString.CommandText = @"
            SELECT DISTINCT COUNT(?myLanguageLabel) as ?count
            WHERE
              { ?myWord a ety:EtymologyEntry ;
                       ety:etymologicallyRelatedTo  ?foreignWord ;
                       rdf:label ?myLanguageLabel .
                ?foreignWord     rdf:label ?foreignWordLabel
                FILTER langMatches(lang(?myLanguageLabel), @myLang)
                FILTER langMatches(lang(?foreignWordLabel), @forLang)
              }  ";

            queryString.SetLiteral("myLang", knownCode);
            queryString.SetLiteral("forLang", unknownLangCode);

            SparqlQuery query = parser.ParseFromString(queryString);
            var resultSet = endpoint.QueryWithResultSet(query.ToString());

            var result = 0;

            if (resultSet is SparqlResultSet)
            {
                result = int.Parse(resultSet.ToList().First().GetLiteral("count"));
            }

            return result;
        }
    }
}
