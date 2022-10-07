using LinkedLanguages.BL.SPARQL.Query;
using LinkedLanguages.DAL;
using LinkedLanguages.Tests.Helpers;

using NUnit.Framework;
using System.Linq;

namespace LinkedLanguages.Tests.SparqlTests
{
    public class SparqlQueryTests
    {
        private WordPairsSparqlQuery sparqlQuery;
        private PairsStatisticsSparqlQuery statisticsQuery;

        public WordDefinitionSparqlQuery wordDefinitionQuery { get; private set; }

        [SetUp]
        public void Setup()
        {
            sparqlQuery = new WordPairsSparqlQuery(TestServices.GetMoqOptions());
            statisticsQuery = new PairsStatisticsSparqlQuery(TestServices.GetMoqOptions());
            wordDefinitionQuery = new WordDefinitionSparqlQuery(TestServices.GetMoqOptions());
        }

        [Test]
        public void ValidateWordPairPumpDetails()
        {
            var results = sparqlQuery.Execute(new WordPairParameterDto("eng", LanguageSeed.EnglishLanguageId, "lat", LanguageSeed.LatinLanguageId, 0));
            Assert.That(results.Count, Is.EqualTo(3));
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
            var firstPageResults = sparqlQuery.Execute(new WordPairParameterDto("eng", LanguageSeed.EnglishLanguageId, "lat", LanguageSeed.LatinLanguageId, 0));
            var secondPageResults = sparqlQuery.Execute(new WordPairParameterDto("eng", LanguageSeed.EnglishLanguageId, "lat", LanguageSeed.LatinLanguageId, 1));
            Assert.That(firstPageResults.Count, Is.EqualTo(3));
            Assert.That(secondPageResults.Count, Is.EqualTo(3));
            Assert.True(!firstPageResults.Intersect(secondPageResults).Any());
        }

        [Test]
        public void GetCount()
        {
            var result = statisticsQuery.Execute(new LanguageCodesDto("eng", "lat"));
            Assert.That(result, Is.EqualTo(23753));
        }

        [Test]
        public void GetDefinition()
        {
            var results = wordDefinitionQuery.Execute(new WordUriDto("http://etytree-virtuoso.wmflabs.org/dbnary/eng/deu/__ee_Anschauung"));
            Assert.That(results.Count, Is.EqualTo(2));
        }
    }
}