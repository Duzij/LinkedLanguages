﻿using LinkedLanguages.BL.SPARQL.Query;
using LinkedLanguages.DAL;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;

using static LinkedLanguages.Tests.Helpers.TestServices;

namespace LinkedLanguages.Tests.LevenshteinDistanceTests
{
    public class FrenchExtendedLevenshteinDistanceTests
    {
        public Dictionary<string, string> removeInvariantsCharactersMapping = new Dictionary<string, string>()
            {
                {"e","é" },
                {"a","â" }
            };
        private readonly Dictionary<string, string> mappingEnglishToFrench = new Dictionary<string, string>()
            {
                {"c",  "ch" },
                {"k",  "que"},
                {"hy",  "é"},
                {"y",  "ie"},
                {"u",  "o, ou"},
                {"st",  "t"}
            };

        public IEnumerable<WordPairExtendedLevenshteinWrapper> wordPairWrappers { get; private set; }

        [SetUp]
        public void Setup()
        {
            var sparqlQuery = GetTestWordPairsSparqlQuery();
            var results = sparqlQuery.Execute(new WordPairParameterDto("eng", LanguageSeed.EnglishLanguageId, "fra", LanguageSeed.RussianLangaugeId, 1));

            wordPairWrappers = results.Select(i => new WordPairExtendedLevenshteinWrapper(i, removeInvariantsCharactersMapping))
                                      .Where(a => a.Distance != 0);
        }


        [Test]
        public void NullCharactersMappingTest()
        {
            Console.WriteLine("Order before characters mapping");
            foreach (var item in wordPairWrappers.OrderBy(d => d.Distance))
            {
                Console.WriteLine(item.ToString());
            }
        }

        [Test]
        public void FrenchCharactersMappingTest()
        {
            Console.WriteLine("Order after characters mapping");
            var alteredWordPairWrappers = wordPairWrappers.Select(i => new WordPairExtendedLevenshteinWrapper(i, mappingEnglishToFrench));
            foreach (var item in alteredWordPairWrappers.OrderBy(d => d.Distance))
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
