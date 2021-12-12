using System;

using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Parsing.Handlers;
using VDS.RDF.Query;
using VDS.RDF.Storage;
using VDS.RDF.Writing.Formatting;

namespace LinkedLanguages.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var endpoint = new SparqlRemoteEndpoint(
                new Uri("https://etytree-virtuoso.wmflabs.org/sparql/"));

            //First we need an instance of the SparqlQueryParser
            SparqlQueryParser parser = new SparqlQueryParser();

            SparqlParameterizedString queryString = new SparqlParameterizedString();

            //Add a namespace declaration
            queryString.Namespaces.AddNamespace("ety", new Uri("http://etytree-virtuoso.wmflabs.org/dbnaryetymology#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/2000/01/rdf-schema#"));

            //Set the SPARQL command
            //For more complex queries we can do this in multiple lines by using += on the
            //CommandText property
            //Note we can use @name style parameters here
            queryString.CommandText = @"
            SELECT DISTINCT  ?myLanguageLabel ?name ?x ?foreignWordLabel
            WHERE
              { ?name  a ety:EtymologyEntry ;
                       ety:etymologicallyRelatedTo  ?x ;
                       rdf:label ?myLanguageLabel .
                ?x     rdf:label ?foreignWordLabel
                FILTER langMatches(lang(?myLanguageLabel), @myLang)
                FILTER langMatches(lang(?foreignWordLabel), @forLang)
              }  LIMIT   10";

            //Inject a Value for the parameter
            queryString.SetLiteral("myLang", "eng");
            queryString.SetLiteral("forLang", "ll");

            //When we call ToString() we get the full command text with namespaces appended as PREFIX
            //declarations and any parameters replaced with their declared values
            System.Console.WriteLine(queryString.ToString());

            //We can turn this into a query by parsing it as in our previous example
            SparqlQuery query = parser.ParseFromString(queryString);
            var resultSet = endpoint.QueryWithResultSet(query.ToString());
            if (resultSet is SparqlResultSet)
            {
                //Print out the Results
                var trippleCollection = resultSet.Results.ToArray();
                foreach (var t in trippleCollection)
                {
                    System.Console.WriteLine(t.ToString());
                }
            }
        }
    }
}
