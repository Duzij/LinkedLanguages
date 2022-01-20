using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Writing;

namespace LinkedLanguages.BL
{
    public static class SparqlExtensions
    {
        public static string GetLiteral(this SparqlResult result, string valueName)
        {
            string data = string.Empty;

            if (result.TryGetBoundValue(valueName, out INode n))
            {
                switch (n.NodeType)
                {
                    case NodeType.Uri:
                        data = ((IUriNode)n).Uri.AbsoluteUri;
                        break;
                    case NodeType.Literal:
                        //You may want to inspect the DataType and Language properties and generate
                        //a different string here
                        data = ((ILiteralNode)n).Value;
                        break;
                    default:
                        throw new RdfOutputException("Unexpected Node Type");
                }
            }

            return data;
        }
    }
}
