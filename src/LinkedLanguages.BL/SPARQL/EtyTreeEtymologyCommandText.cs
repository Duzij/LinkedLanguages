namespace LinkedLanguages.BL.SPARQL
{
    public static class EtyTreeEtymologyCommandText
    {
        public static string GetTwoWayLanguageWhereUnionCommandText()
        {
            return @"
            WHERE  { 
            { ?myLanguageWord  a ety:EtymologyEntry  .
              ?foreignLanguageWord  a ety:EtymologyEntry  .
              ?myLanguageWord  ety:etymologicallyRelatedTo  ?foreignLanguageWord .
              ?myLanguageWord  rdf:label ?myLanguageLabel .
              ?foreignLanguageWord      rdf:label ?foreignWordLabel .
              FILTER langMatches(lang(?myLanguageLabel), @myLang)
              FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }
            UNION 
            { ?myLanguageWord  a ety:EtymologyEntry  .
              ?foreignLanguageWord  a ety:EtymologyEntry  .
              ?myLanguageWord  ety:etymologicallyDerivesFrom ?foreignLanguageWord .
              ?myLanguageWord rdf:label  ?myLanguageLabel .
              ?foreignLanguageWord rdf:label             ?foreignWordLabel 
              FILTER langMatches(lang(?myLanguageLabel), @myLang)
              FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }
            UNION
            { ?myLanguageWord  a ety:EtymologyEntry  .
              ?foreignLanguageWord  a ety:EtymologyEntry  .
              ?foreignLanguageWord  ety:etymologicallyRelatedTo ?myLanguageWord  .
              ?foreignLanguageWord  rdf:label ?foreignWordLabel .
              ?myLanguageWord      rdf:label             ?myLanguageLabel
              FILTER langMatches(lang(?myLanguageLabel), @myLang)
              FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }
            UNION
            { ?myLanguageWord  a ety:EtymologyEntry  .
              ?foreignLanguageWord  a ety:EtymologyEntry  .
              ?foreignLanguageWord  ety:etymologicallyDerivesFrom ?myLanguageWord .
              ?foreignLanguageWord  rdf:label  ?foreignWordLabel .
              ?myLanguageWord  rdf:label  ?myLanguageLabel
              FILTER langMatches(lang(?myLanguageLabel), @myLang)
              FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }}";
        }
    }
}
