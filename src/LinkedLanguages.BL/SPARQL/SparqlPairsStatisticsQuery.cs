using Microsoft.Extensions.Options;

using System;
using System.Linq;

using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace LinkedLanguages.BL.SPARQL
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

            queryString.CommandText = @"SELECT (COUNT(lang(?foreignWordLabel )) as ?CountOfEtymologies)";
            queryString.CommandText += EtyTreeEtymologyQuery.GetTwoWayLanguageWhereUnionQuery();
            queryString.CommandText += "GROUP BY lang(?foreignWordLabel)";

            queryString.SetLiteral("myLang", knownCode);
            queryString.SetLiteral("forLang", unknownLangCode);

            SparqlQuery query = parser.ParseFromString(queryString);
            endpoint.Timeout = 50000;
            var resultSet = endpoint.QueryWithResultSet(query.ToString());

            var result = 0;

            if (resultSet is not null)
            {
                result = int.Parse(resultSet.ToList().First().GetLiteral("CountOfEtymologies"));
            }

            return result;
        }

    }
}
