using LinkedLanguages.BL.SPARQL.Query;
using LinkedLanguages.DAL;

using NUnit.Framework;

using System;
using System.Linq;


using static LinkedLanguages.Tests.Helpers.TestServices;

namespace LinkedLanguages.Tests.LevenshteinDistanceTests
{
    public class TransliterationTests
    {
        private WordPairsSparqlQuery sparqlQuery;

        [SetUp]
        public void Setup()
        {
            sparqlQuery = GetTestWordPairsSparqlQuery();
        }

        [Test]
        public void TransliterationToAsciiTest()
        {
            var results = sparqlQuery.Execute(new WordPairParameterDto("eng", LanguageSeed.EnglishLanguageId, "rus",
                                                                       LanguageSeed.RussianLangaugeId, 1));

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
