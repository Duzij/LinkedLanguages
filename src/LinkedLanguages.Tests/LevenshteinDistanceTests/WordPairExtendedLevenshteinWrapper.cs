using LinkedLanguages.BL.CLLD;
using LinkedLanguages.DAL.Models;
using System.Collections.Generic;

namespace LinkedLanguages.Tests.LevenshteinDistanceTests
{
    public class WordPairExtendedLevenshteinWrapper
    {
        public int Distance { get; set; }
        public WordPair WordPair { get; }


        public WordPairExtendedLevenshteinWrapper(WordPair wordPair, Dictionary<string, string> charactersMapping = null)
        {
            WordPair = new WordPair()
            {
                KnownWord = wordPair.KnownWord.ToLowerInvariant().Trim(),
                UnknownWord = wordPair.UnknownWord.ToLowerInvariant().Trim(),
                UnknownWordUri = wordPair.UnknownWordUri.ToLowerInvariant().Trim(),
            };

            if (wordPair.UnknownWord is not null && wordPair.KnownWord is not null)
            {
                Distance = CrossLingualLevenshteinDistanceCalculator.Calc(wordPair.UnknownWord, wordPair.KnownWord, charactersMapping);
            }
        }

        public WordPairExtendedLevenshteinWrapper(WordPairExtendedLevenshteinWrapper wrapper, Dictionary<string, string> charactersMapping = null)
        {
            WordPair = new WordPair()
            {
                KnownWord = wrapper.WordPair.KnownWord.ToLowerInvariant().Trim(),
                UnknownWord = wrapper.WordPair.UnknownWord.ToLowerInvariant().Trim(),
                UnknownWordUri = wrapper.WordPair.UnknownWordUri.ToLowerInvariant().Trim(),
            };

            if (wrapper.WordPair.UnknownWord is not null && wrapper.WordPair.KnownWord is not null)
            {
                Distance = CrossLingualLevenshteinDistanceCalculator.Calc(WordPair.UnknownWord, WordPair.KnownWord, charactersMapping);
            }
        }

        public override string ToString()
        {
            return $"KnownWord: {WordPair.KnownWord}, UnknownWord: {WordPair.UnknownWord}({WordPair.UnknownWordUri}), Distance: {Distance}";
        }

        public string ToStringWithoutDistanceInfo()
        {
            return $"KnownWord: {WordPair.KnownWord}, UnknownWord: {WordPair.UnknownWord}";
        }
    }
}
