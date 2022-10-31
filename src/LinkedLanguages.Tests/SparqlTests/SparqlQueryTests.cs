using LinkedLanguages.BL.SPARQL.Query;
using LinkedLanguages.DAL;

using NUnit.Framework;
using System.Linq;

using static LinkedLanguages.Tests.Helpers.TestServices;

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
            sparqlQuery = GetTestWordPairsSparqlQuery();
            statisticsQuery = GetTestPairsStatisticsSparqlQuery();
            wordDefinitionQuery = GetTestWordDefinitionSparqlQuery();
        }

        [Test]
        public void ValidateWordPairPumpDetails()
        {
            System.Collections.Generic.List<DAL.Models.WordPair> results = sparqlQuery.Execute(new WordPairParameterDto("eng", LanguageSeed.EnglishLanguageId, "lat", LanguageSeed.LatinLanguageId, 0));
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
            System.Collections.Generic.List<DAL.Models.WordPair> firstPageResults = sparqlQuery.Execute(new WordPairParameterDto("eng", LanguageSeed.EnglishLanguageId, "lat", LanguageSeed.LatinLanguageId, 0));
            System.Collections.Generic.List<DAL.Models.WordPair> secondPageResults = sparqlQuery.Execute(new WordPairParameterDto("eng", LanguageSeed.EnglishLanguageId, "lat", LanguageSeed.LatinLanguageId, 1));
            Assert.That(firstPageResults.Count, Is.EqualTo(3));
            Assert.That(secondPageResults.Count, Is.EqualTo(3));
            Assert.True(!firstPageResults.Intersect(secondPageResults).Any());
        }

        [Test]
        public void GetCount()
        {
            int result = statisticsQuery.Execute(new LanguageCodesDto("eng", "lat"));
            Assert.That(result, Is.EqualTo(141119));
        }

        [Test]
        public void GetDefinition()
        {
            System.Collections.Generic.IEnumerable<string> results = wordDefinitionQuery.Execute(new WordUriDto("http://etytree-virtuoso.wmflabs.org/dbnary/eng/deu/__ee_Anschauung"));
            Assert.That(results.Count, Is.EqualTo(2));
        }
    }
}