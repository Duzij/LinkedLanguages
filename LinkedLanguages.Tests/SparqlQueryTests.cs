using LinkedLanguages.BL;
using LinkedLanguages.DAL;

using NUnit.Framework;

using System.Linq;

namespace LinkedLanguages.Tests
{
    public class SparqlQueryTests
    {
        private SparqlPairsQuery sparqlQuery;

        [SetUp]
        public void Setup()
        {
            sparqlQuery = new SparqlPairsQuery(TestServices.GetMoqOptions());
        }

        [Test]
        public void PumpEnglishAndLatin()
        {
            var results = sparqlQuery.Execute("eng", LanguageSeed.EnglishLanguageId, "lat", LanguageSeed.LatinLanguageId, 0, 10);
            Assert.AreEqual(10, results.Count);
        }

        [Test]
        public void ValidateEnglishAndLatin()
        {
            var firstPageResults = sparqlQuery.Execute("eng", LanguageSeed.EnglishLanguageId, "lat", LanguageSeed.LatinLanguageId, 0, 3);
            var secondPageResults = sparqlQuery.Execute("eng", LanguageSeed.EnglishLanguageId, "lat", LanguageSeed.LatinLanguageId, 1, 3);
            Assert.AreEqual(3, firstPageResults.Count);
            Assert.AreEqual(3, secondPageResults.Count);
            Assert.True(!firstPageResults.Intersect(secondPageResults).Any());
        }
    }
}