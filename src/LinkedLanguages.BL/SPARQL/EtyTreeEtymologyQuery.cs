using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedLanguages.BL.SPARQL
{
    public static class EtyTreeEtymologyQuery
    {
        public static string GetTwoWayLanguageWhereUnionQuery()
        {
            return @"
             WHERE {
            {   ?name a ety:EtymologyEntry;
                ety:etymologicallyRelatedTo ?x;
                rdf:label ?myLanguageLabel .
                ?x rdf:label ?foreignWordLabel
                FILTER langMatches(lang(?myLanguageLabel), @myLang)
                FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }
                UNION
            {   ?name a ety:EtymologyEntry;
                ety:etymologicallyDerivesFrom ?x;
                rdf:label ?myLanguageLabel .
                ?x rdf:label ?foreignWordLabel
                FILTER langMatches(lang(?myLanguageLabel), @myLang)
                FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }
                UNION
            {   ?x a ety:EtymologyEntry;
                ety:etymologicallyRelatedTo ?name;
                rdf:label ?foreignWordLabel .
                ?name rdf:label ?myLanguageLabel
                FILTER langMatches(lang(?myLanguageLabel), @myLang)
                FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }
                UNION
            {   ?x a ety:EtymologyEntry;
                ety:etymologicallyDerivesFrom ?name;
                rdf:label ?foreignWordLabel .
                ?name rdf:label ?myLanguageLabel
                FILTER langMatches(lang(?myLanguageLabel), @myLang)
                FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }}";
        }
    }
}
