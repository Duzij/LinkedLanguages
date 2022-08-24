using AnyAscii;

using DamerauLevenshteinDistance.Console;

using LinkedLanguages.BL;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;

using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedLanguages.Tests
{
    public class ExpandedLevenshteinDistanceTests
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

    public class WordPairLevenshteinWrapper
    {
        public int Distance { get; set; }
        public WordPairLevenshteinWrapper(WordPair wordPair)
        {
            WordPair = wordPair;

            if (wordPair.UnknownWord is not null && wordPair.KnownWord is not null)
            {
                Distance = DamerauLevenshteinCalculator.Calc(wordPair.UnknownWord, wordPair.KnownWord);
            }
        }

        public WordPairLevenshteinWrapper(WordPair wordPair, WordPair beforeTransliteration) : this(wordPair)
        {
            BeforeTransliteration = beforeTransliteration;
        }

        public WordPair WordPair { get; }

        public WordPair BeforeTransliteration { get; set; }

        public override string ToString()
        {
            return $"KnownWord: {WordPair.KnownWord} {BeforeTransliteration?.KnownWord}, UnknownWord: {WordPair.UnknownWord} {BeforeTransliteration?.UnknownWord}, Distance: {Distance}";
        }

        public WordPairLevenshteinWrapper RemoveDiacritis()
        {
            //WordPair.KnownWord = $"{WordPair.KnownWord.RemoveDiacritics()}, (previously {WordPair.KnownWord})";
            //WordPair.UnknownWord = $"{WordPair.UnknownWord.RemoveDiacritics()}, (previously {WordPair.UnknownWord})";
            BeforeTransliteration = new WordPair { KnownWord = WordPair.KnownWord, UnknownWord = WordPair.UnknownWord };

            WordPair.KnownWord = $"{WordPair.KnownWord.Transliterate()}";
            WordPair.UnknownWord = $"{WordPair.UnknownWord.Transliterate()}";

            return new WordPairLevenshteinWrapper(WordPair, BeforeTransliteration);
        }
    }
}
