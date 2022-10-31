using VDS.RDF.Query;
using Microsoft.Extensions.Options;
using System.Linq;
using System;
using LinkedLanguages.BL.SPARQL.Base;
using Microsoft.Extensions.Logging;
using System.Web;

namespace LinkedLanguages.BL.SPARQL.Query
{
    public readonly record struct SeeAlsoLinkDto(string uri, string name);
    public readonly record struct WordPairSeeAlsoLinksDto(SeeAlsoLinkDto[] knownLinks, SeeAlsoLinkDto[] unknownLinks);
    public class WordSeeAlsoLinkSparqlQuery : SparqlQueryBase<SeeAlsoLinkDto[], WordUriDto>
    {
        public WordSeeAlsoLinkSparqlQuery(IOptions<SparqlEndpointOptions> options, ILogger<SparqlQueryBase<SeeAlsoLinkDto[], WordUriDto>> logger) : base(options, logger)
        {
        }
        public override int TimeOut => 5000;

        public override string CommandText { get; set; } = "SELECT ?value " +
            "WHERE {" +
            " ?uri <http://www.w3.org/2000/01/rdf-schema#seeAlso> ?value ." +
            " FILTER (?uri = <@uri>)" +
            "}";

        protected override void SetQueryParams(SparqlParameterizedString queryString, WordUriDto param)
        {
            queryString.CommandText = queryString.CommandText.Replace("@uri", param.Uri);
        }

        protected override SeeAlsoLinkDto[] ParseResult(SparqlResultSet resultSet, WordUriDto param)
        {
            SparqlResult[] trippleCollection = resultSet.Results.ToArray();
            return trippleCollection.Select(t => new SeeAlsoLinkDto(t.GetLiteral("value"), HttpUtility.UrlDecode(new Uri(t.GetLiteral("value")).Segments[2]))).ToArray();
        }
    }
}
