using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Writing;

namespace LinkedLanguages.BL.SPARQL
{
    public static class SparqlExtensions
    {
        public static string GetLiteral(this SparqlResult result, string valueName)
        {
            string data = string.Empty;

            if (result.TryGetBoundValue(valueName, out INode n))
            {
                data = n.NodeType switch
                {
                    NodeType.Uri => ((IUriNode)n).Uri.AbsoluteUri,
                    NodeType.Literal => ((ILiteralNode)n).Value,
                    _ => throw new RdfOutputException("Unexpected Node Type"),
                };
            }

            return data;
        }
    }
}
