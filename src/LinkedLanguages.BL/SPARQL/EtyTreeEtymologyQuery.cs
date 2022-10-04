namespace LinkedLanguages.BL.SPARQL
{
    public static class EtyTreeEtymologyQuery
    {
        public static string GetTwoWayLanguageWhereUnionQuery()
        {
            return @"
             WHERE {
            {   ?myWord a ety:EtymologyEntry;
                ety:etymologicallyRelatedTo ?foreignWord;
                rdf:label ?myLanguageLabel .

                ?foreignWord rdf:label ?foreignWordLabel

                FILTER langMatches(lang(?myLanguageLabel), @myLang)
                FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }
                UNION
            {   ?myWord a ety:EtymologyEntry;
                ety:etymologicallyDerivesFrom ?foreignWord;
                rdf:label ?myLanguageLabel .

                ?foreignWord rdf:label ?foreignWordLabel

                FILTER langMatches(lang(?myLanguageLabel), @myLang)
                FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }
                UNION
            {   ?foreignWord a ety:EtymologyEntry;
                ety:etymologicallyRelatedTo ?myWord;
                rdf:label ?foreignWordLabel .

                ?myWord rdf:label ?myLanguageLabel

                FILTER langMatches(lang(?myLanguageLabel), @myLang)
                FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }
                UNION
            {   ?foreignWord a ety:EtymologyEntry;
                ety:etymologicallyDerivesFrom ?myWord;
                rdf:label ?foreignWordLabel .

                ?myWord rdf:label ?myLanguageLabel

                FILTER langMatches(lang(?myLanguageLabel), @myLang)
                FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }}";
        }
    }
}
