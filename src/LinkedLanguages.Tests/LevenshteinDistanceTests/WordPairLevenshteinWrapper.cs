using AnyAscii;

using DamerauLevenshteinDistance.Console;

using LinkedLanguages.DAL.Models;

namespace LinkedLanguages.Tests.LevenshteinDistanceTests
{
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
            BeforeTransliteration = new WordPair { KnownWord = WordPair.KnownWord, UnknownWord = WordPair.UnknownWord };

            WordPair.KnownWord = $"{WordPair.KnownWord.Transliterate()}";
            WordPair.UnknownWord = $"{WordPair.UnknownWord.Transliterate()}";

            return new WordPairLevenshteinWrapper(WordPair, BeforeTransliteration);
        }
    }
}
