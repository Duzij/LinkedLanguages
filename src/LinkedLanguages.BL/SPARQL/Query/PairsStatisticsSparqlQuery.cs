using LinkedLanguages.BL.SPARQL.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using VDS.RDF.Query;


namespace LinkedLanguages.BL.SPARQL.Query
{
    public readonly record struct LanguageCodesDto(string knownLangCode, string unknownLangCode);

    public class PairsStatisticsSparqlQuery : SparqlQueryBase<int, LanguageCodesDto>
    {
        public PairsStatisticsSparqlQuery(IOptions<SparqlEndpointOptions> options, ILogger<SparqlQueryBase<int, LanguageCodesDto>> logger) : base(options, logger)
        {
        }

        public override int TimeOut => 75000;


        public override string CommandText { get; set; } = string.Concat("SELECT (COUNT(lang(?foreignWordLabel )) as ?CountOfEtymologies) ",
                                                      EtyTreeEtymologyCommandText.GetTwoWayLanguageWhereUnionCommandText(),
                                                      " GROUP BY lang(?foreignWordLabel)");

        protected override int ParseResult(SparqlResultSet resultSet, LanguageCodesDto param)
        {
            return int.Parse(resultSet.ToList().First().GetLiteral("CountOfEtymologies"));
        }

        protected override void SetQueryParams(SparqlParameterizedString queryString, LanguageCodesDto param)
        {
            queryString.SetLiteral("myLang", param.knownLangCode);
            queryString.SetLiteral("forLang", param.unknownLangCode);
        }
    }
}
