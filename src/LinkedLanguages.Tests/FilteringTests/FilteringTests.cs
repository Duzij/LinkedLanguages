using LinkedLanguages.BL.Services;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;


namespace LinkedLanguages.Tests.FilteringTests
{
    public class FilteringTests
    {
        public List<WordPair> TestResults { get; set; }
        [SetUp]
        public void Setup()
        {
            TestResults = new List<WordPair>()
            {
                new WordPair() {KnownWord = "test",UnknownWord = "test" }, //same words
                new WordPair() {KnownWord = "-test",UnknownWord = "test" }, //suffixes or prefixes
                new WordPair() {KnownWord = "test",UnknownWord = "-test" }, //suffixes or prefixes
                new WordPair() {KnownWord = "test-",UnknownWord = "test" }, //suffixes or prefixes
                new WordPair() {KnownWord = "test",UnknownWord = "-test", Distance=4 }, //high CLLD
                new WordPair() {KnownWord = "test1",UnknownWord = "test2", KnownWordTransliterated = "transliterated", UnknownWordTransliterated="transliterated" }, //transliterated word are the same
                new WordPair() {KnownWord = "test1",UnknownWord = "test2", KnownWordTransliterated = "transliterated", UnknownWordTransliterated="transliterated" }, //transliterated duplicate
            };
        }

        [Test]
        public void AllTestResultsAreFiltered()
        {
            var results = TestResults.FilterSameWords()
                       .FilterSuffixesAndPrefixes()
                       .FilterWordPairsWithHighCLLD(LanguageSeed.EnglishLanguageId, LanguageSeed.EnglishLanguageId)
                       .FilterTransliteratedDuplicates()
                       .ToList();


            Assert.IsTrue(results.Count == 1);
        }

    }
}