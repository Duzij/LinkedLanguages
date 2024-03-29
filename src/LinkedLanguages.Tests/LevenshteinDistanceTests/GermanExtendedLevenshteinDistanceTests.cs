﻿using LinkedLanguages.BL.SPARQL.Query;
using LinkedLanguages.DAL;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using static LinkedLanguages.Tests.Helpers.TestServices;

namespace LinkedLanguages.Tests.LevenshteinDistanceTests
{
    public class GermanExtendedLevenshteinDistanceTests
    {
        public Dictionary<string, string> removeInvariantsCharactersMapping = new Dictionary<string, string>()
            {
                {"u",  "ü" },
                {"o",  "ö"},
                {"a",  "ä"}
            };
        private readonly Dictionary<string, string> mappingEnglishToGerman = new Dictionary<string, string>()
            {
                {"th",  "d" },
                {"d",  "t"},
                {"c",  "k"},
                {"f",  "b,v"},
                {"p",  "f"},
                {"pp",  "p"},
                {"x",  "chs"},
                {"sh",  "sch"},
                {"s",  "sch"},
                {"v",  "b"},
                {"t",  "ss"},
                {"n",  "nn"}
            };

        public IEnumerable<WordPairExtendedLevenshteinWrapper> wordPairWrappers { get; private set; }

        [SetUp]
        public void Setup()
        {
            var sparqlQuery = GetTestWordPairsSparqlQuery();
            var results = sparqlQuery.Execute(new WordPairParameterDto("eng", LanguageSeed.EnglishLanguageId, "deu", LanguageSeed.RussianLangaugeId, 1));

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
        public void GermanCharactersMappingTest()
        {
            Console.WriteLine("Order after characters mapping");
            var alteredWordPairWrappers = wordPairWrappers.Select(i => new WordPairExtendedLevenshteinWrapper(i, mappingEnglishToGerman));
            foreach (var item in alteredWordPairWrappers.OrderBy(d => d.Distance))
            {
                Console.WriteLine(item.ToString());
            }
        }

        [Ignore("Takes up to 30 min")]
        public void OnlyDifferencesAppear()
        {
            var sparqlQuery = GetTestWordPairsSparqlQuery();

            var list = new List<WordPairExtendedLevenshteinWrapper>();
            var index = 0;

            while (list.Count() < 250)
            {
                var result = sparqlQuery.Execute(new WordPairParameterDto("eng", LanguageSeed.EnglishLanguageId, "deu", LanguageSeed.RussianLangaugeId, index++));

                var wp = new WordPairExtendedLevenshteinWrapper(result.First(), removeInvariantsCharactersMapping);
                if (wp.Distance == 0)
                {
                    continue;
                }
                else
                {
                    var wpWithMapping = new WordPairExtendedLevenshteinWrapper(result.First(), mappingEnglishToGerman);
                    if (wpWithMapping.Distance != wp.Distance)
                    {
                        list.Add(wpWithMapping);
                        Console.WriteLine($"Mapping distance change:{wp.ToStringWithoutDistanceInfo()}. Before:{wp.Distance}, After{wpWithMapping.Distance}");
                    }
                }
            }

        }
    }
}
