namespace LinkedLanguages.BL.SPARQL
{
    public static class EtyTreeEtymologyCommandText
    {
        public static string GetTwoWayLanguageWhereUnionCommandText()
        {
            return @"
             WHERE {
            {   ?myWord a ety:EtymologyEntry;
                ety:etymologicallyRelatedTo ?foreignWord;
                rdf:label ?myLanguageLabel .

                OPTIONAL { ?myWord rdf:seeAlso ?knownSeeAlsoLink }

                ?foreignWord rdf:label ?foreignWordLabel

                FILTER langMatches(lang(?myLanguageLabel), @myLang)
                FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }
                UNION
            {   ?myWord a ety:EtymologyEntry;
                ety:etymologicallyDerivesFrom ?foreignWord;
                rdf:label ?myLanguageLabel .

                OPTIONAL { ?myWord rdf:seeAlso ?knownSeeAlsoLink }

                ?foreignWord rdf:label ?foreignWordLabel

                FILTER langMatches(lang(?myLanguageLabel), @myLang)
                FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }
                UNION
            {   ?foreignWord a ety:EtymologyEntry;
                ety:etymologicallyRelatedTo ?myWord;
                rdf:label ?foreignWordLabel .

                OPTIONAL { ?foreignWord rdf:seeAlso ?unknownSeeAlsoLink }

                ?myWord rdf:label ?myLanguageLabel

                FILTER langMatches(lang(?myLanguageLabel), @myLang)
                FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }
                UNION
            {   ?foreignWord a ety:EtymologyEntry;
                ety:etymologicallyDerivesFrom ?myWord;
                rdf:label ?foreignWordLabel .

                OPTIONAL { ?foreignWord rdf:seeAlso ?unknownSeeAlsoLink }

                ?myWord rdf:label ?myLanguageLabel

                FILTER langMatches(lang(?myLanguageLabel), @myLang)
                FILTER langMatches(lang(?foreignWordLabel), @forLang)
            }}";
        }
    }
}
