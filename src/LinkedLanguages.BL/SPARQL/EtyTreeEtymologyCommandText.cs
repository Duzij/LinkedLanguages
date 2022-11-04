namespace LinkedLanguages.BL.SPARQL
{
    public static class EtyTreeEtymologyCommandText
    {
        public static string GetTwoWayLanguageWhereUnionCommandText()
        {
            return @"
            WHERE  { 
            { ?myLangugageWord  a ety:EtymologyEntry  .
              ?foreignWord  a ety:EtymologyEntry  .
              ?myLangugageWord  ety:etymologicallyRelatedTo  ?foreignWord .
              ?myLangugageWord  rdf:label ?myLanguageLabel .
              ?foreignWord      rdf:label ?foreignWordLabel .
              FILTER langMatches(lang(?myLanguageLabel), @myLang)
              FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }
            UNION 
            { ?myLangugageWord  a ety:EtymologyEntry  .
              ?foreignWord  a ety:EtymologyEntry  .
              ?myLangugageWord  ety:etymologicallyDerivesFrom ?foreignWord .
              ?myLangugageWord rdf:label  ?myLanguageLabel .
              ?foreignWord rdf:label             ?foreignWordLabel 
              FILTER langMatches(lang(?myLanguageLabel), @myLang)
              FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }
            UNION
            { ?myLangugageWord  a ety:EtymologyEntry  .
              ?foreignWord  a ety:EtymologyEntry  .
              ?foreignWord  ety:etymologicallyRelatedTo ?myLangugageWord  .
              ?foreignWord  rdf:label ?foreignWordLabel .
              ?myLangugageWord      rdf:label             ?myLanguageLabel
              FILTER langMatches(lang(?myLanguageLabel), @myLang)
              FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }
            UNION
            { ?myLangugageWord  a ety:EtymologyEntry  .
              ?foreignWord  a ety:EtymologyEntry  .
              ?foreignWord  ety:etymologicallyDerivesFrom ?myLangugageWord .
              ?foreignWord  rdf:label  ?foreignWordLabel .
              ?myLangugageWord  rdf:label  ?myLanguageLabel
              FILTER langMatches(lang(?myLanguageLabel), @myLang)
              FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }}";
        }
    }
}
