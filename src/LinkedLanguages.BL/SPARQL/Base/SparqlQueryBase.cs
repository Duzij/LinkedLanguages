using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace LinkedLanguages.BL.SPARQL.Base
{
    public abstract class SparqlQueryBase<ReturnT, ParamT>
    {
        public readonly SparqlEndpointOptions options;
        private readonly Stopwatch stopWatch;
        private readonly ILogger<SparqlQueryBase<ReturnT, ParamT>> logger;

        public abstract string CommandText { get; set; }
        protected abstract ReturnT ParseResult(SparqlResultSet resultSet, ParamT param);
        protected abstract void SetQueryParams(SparqlParameterizedString queryString, ParamT param);

        public SparqlRemoteEndpoint endpoint { get; private set; }
        public virtual int TimeOut => 40000;

        public SparqlQueryBase(IOptions<SparqlEndpointOptions> options, ILogger<SparqlQueryBase<ReturnT, ParamT>> logger)
        {
            this.options = options.Value;
            stopWatch = new Stopwatch();
            this.logger = logger;
        }

        public ReturnT Execute(ParamT param)
        {
            stopWatch.Start();
            SparqlQuery query = PrepareQuery(param);

            SparqlResultSet resultSet;
            try
            {
                resultSet = endpoint.QueryWithResultSet(query.ToString());
                stopWatch.Stop();
                logger.LogInformation($"SPQRQL query execution took {stopWatch.ElapsedMilliseconds} miliseconds.");

            }
            catch (System.Exception ex)
            {
                logger.LogWarning($"{nameof(ex)} timeouted.");
                throw new InvalidOperationException($"Timeout", ex);
            }

            if (resultSet.IsEmpty)
            {
                logger.LogWarning($"SPARQL result set is empty.");
                throw new InvalidOperationException($"SPARQL result set is empty.");
            }

            return ParseResult(resultSet, param);
        }


        public ReturnT ExecuteDontThrow(ParamT param)
        {
            SparqlQuery query = PrepareQuery(param);
            try
            {
                SparqlResultSet resultSet = endpoint.QueryWithResultSet(query.ToString());
                return !resultSet.IsEmpty ? ParseResult(resultSet, param) : default;
            }
            catch (System.Exception)
            {
                return default;
            }
        }

        private SparqlQuery PrepareQuery(ParamT param)
        {
            endpoint = new SparqlRemoteEndpoint(options.EndpointUrl)
            {
                Timeout = TimeOut
            };

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlParameterizedString queryString = new SparqlParameterizedString(CommandText);

            queryString.Namespaces.AddNamespace("lemon", new Uri("http://www.w3.org/ns/lemon/ontolex#"));
            queryString.Namespaces.AddNamespace("skos", new Uri("http://www.w3.org/2004/02/skos/core#"));
            queryString.Namespaces.AddNamespace("kaiko", new Uri("http://kaiko.getalp.org/dbnary#"));
            queryString.Namespaces.AddNamespace("ety", new Uri("http://etytree-virtuoso.wmflabs.org/dbnaryetymology#"));
            queryString.Namespaces.AddNamespace("rdfns", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/2000/01/rdf-schema#"));

            SetQueryParams(queryString, param);

            SparqlQuery query = parser.ParseFromString(queryString);
            return query;
        }
    }
}