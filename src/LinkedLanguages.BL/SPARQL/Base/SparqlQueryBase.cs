using Microsoft.Extensions.Options;
using System;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace LinkedLanguages.BL.SPARQL.Base
{
    public abstract class SparqlQueryBase<ReturnT, ParamT>
    {
        public readonly SparqlEndpointOptions options;

        public abstract string CommandText { get; set; }
        protected abstract ReturnT ParseResult(SparqlResultSet resultSet, ParamT param);
        protected abstract void SetQueryParams(SparqlParameterizedString queryString, ParamT param);

        public SparqlRemoteEndpoint endpoint { get; private set; }

        public SparqlQueryBase(IOptions<SparqlEndpointOptions> options)
        {
            this.options = options.Value;
        }

        public ReturnT Execute(ParamT param)
        {
            endpoint = new SparqlRemoteEndpoint(options.EndpointUrl)
            {
                Timeout = 50000
            };

            SparqlQueryParser parser = new SparqlQueryParser();
            var queryString = new SparqlParameterizedString(CommandText);

            queryString.Namespaces.AddNamespace("lemon", new Uri("http://www.w3.org/ns/lemon/ontolex#"));
            queryString.Namespaces.AddNamespace("skos", new Uri("http://www.w3.org/2004/02/skos/core#"));
            queryString.Namespaces.AddNamespace("kaiko", new Uri("http://kaiko.getalp.org/dbnary#"));
            queryString.Namespaces.AddNamespace("ety", new Uri("http://etytree-virtuoso.wmflabs.org/dbnaryetymology#"));
            queryString.Namespaces.AddNamespace("rdfns", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/2000/01/rdf-schema#"));

            SetQueryParams(queryString, param);

            SparqlQuery query = parser.ParseFromString(queryString);
            var resultSet = endpoint.QueryWithResultSet(query.ToString());

            return !resultSet.IsEmpty ? ParseResult(resultSet, param) : throw new InvalidOperationException($"SPARQL result set is empty.");
        }

    }
}