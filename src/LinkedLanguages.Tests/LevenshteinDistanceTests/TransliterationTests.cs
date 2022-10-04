using LinkedLanguages.BL.SPARQL;
using LinkedLanguages.DAL;
using LinkedLanguages.Tests.Helpers;

using NUnit.Framework;

using System;
using System.Linq;

namespace LinkedLanguages.Tests.LevenshteinDistanceTests
{
    public class TransliterationTests
    {
        private SparqlPairsQuery sparqlQuery;

        [SetUp]
        public void Setup()
        {
            sparqlQuery = new SparqlPairsQuery(TestServices.GetMoqOptions());
        }

        [Test]
        public void TransliterationToAsciiTest()
        {
            var results = sparqlQuery.Execute("eng", LanguageSeed.EnglishLanguageId, "rus", LanguageSeed.RussianLangaugeId, 1, 100);

            Console.WriteLine("Before normalization");

            foreach (var item in results.Select(i => new WordPairLevenshteinWrapper(i)).OrderBy(d => d.Distance))
            {
                Console.WriteLine(item.ToString());
            }

            Console.WriteLine("After normalization");

            foreach (var item in results.Select(i => new WordPairLevenshteinWrapper(i).RemoveDiacritis()).OrderBy(d => d.Distance))
            {
                Console.WriteLine(item.ToString());
            }
        }

    }
}
