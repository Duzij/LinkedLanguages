using LinkedLanguages.BL.SPARQL.Base;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using VDS.RDF.Query;

namespace LinkedLanguages.BL.SPARQL.Query
{
    public readonly record struct WordUriDto(string Uri);

    public class WordDefinitionSparqlQuery : SparqlQueryBase<IEnumerable<string>, WordUriDto>
    {
        public WordDefinitionSparqlQuery(IOptions<SparqlEndpointOptions> options) : base(options)
        {
        }

        public override string CommandText { get; set; } =
            @"SELECT ?defVal ?value
            WHERE   {
                    <@uri> kaiko:describes ?o .
                    
                    OPTIONAL {
                                ?o lemon:sense ?def .
                                ?def skos:definition ?defVal .
                                ?defVal rdfns:value ?value .
                             }
                    OPTIONAL {
                                ?o kaiko:describes ?o2 .
                                ?o2 lemon:sense ?def .
                                ?def skos:definition ?defVal .
                                ?defVal rdfns:value ?value .
                             }
            }";

        protected override List<string> ParseResult(SparqlResultSet resultSet, WordUriDto param)
        {
            return resultSet.Results.Select(a => a.GetLiteral("value")).ToList();
        }

        protected override void SetQueryParams(SparqlParameterizedString queryString, WordUriDto param)
        {
            //hack
            queryString.CommandText = queryString.CommandText.Replace("@uri", param.Uri);
        }
    }
}
