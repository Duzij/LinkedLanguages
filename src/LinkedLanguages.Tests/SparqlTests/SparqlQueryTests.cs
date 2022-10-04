using LinkedLanguages.BL.SPARQL;
using LinkedLanguages.DAL;
using LinkedLanguages.Tests.Helpers;

using NUnit.Framework;

using System.Linq;

namespace LinkedLanguages.Tests.SparqlTests
{
    public class SparqlQueryTests
    {
        private SparqlPairsQuery sparqlQuery;
        private SparqlPairsStatisticsQuery statisticsQuery;

        [SetUp]
        public void Setup()
        {
            sparqlQuery = new SparqlPairsQuery(TestServices.GetMoqOptions());
            statisticsQuery = new SparqlPairsStatisticsQuery(TestServices.GetMoqOptions());
        }

        [Test]
        public void PumpEnglishAndLatin()
        {
            var results = sparqlQuery.Execute("eng", LanguageSeed.EnglishLanguageId, "lat", LanguageSeed.LatinLanguageId, 0, 10);
            Assert.That(results.Count, Is.EqualTo(10));
            Assert.That(results.FirstOrDefault().KnownWordUri, Is.Not.Null);
            Assert.That(results.FirstOrDefault().UnknownWordUri, Is.Not.Null);

            Assert.That(results.FirstOrDefault().KnownLanguageCode, Is.Not.Null);
            Assert.That(results.FirstOrDefault().UnknownLanguageCode, Is.Not.Null);

            Assert.That(results.FirstOrDefault().KnownWord, Is.Not.Null);
            Assert.That(results.FirstOrDefault().UnknownWord, Is.Not.Null);
        }

        [Test]
        public void ValidateEnglishAndLatin()
        {
            var firstPageResults = sparqlQuery.Execute("eng", LanguageSeed.EnglishLanguageId, "lat", LanguageSeed.LatinLanguageId, 0, 3);
            var secondPageResults = sparqlQuery.Execute("eng", LanguageSeed.EnglishLanguageId, "lat", LanguageSeed.LatinLanguageId, 1, 3);
            Assert.That(firstPageResults.Count, Is.EqualTo(3));
            Assert.That(secondPageResults.Count, Is.EqualTo(3));
            Assert.True(!firstPageResults.Intersect(secondPageResults).Any());
        }

        [Test]
        public void GetCount()
        {
            var result = statisticsQuery.Execute("eng", "lat");
            Assert.That(result, Is.EqualTo(23753));
        }
    }
}