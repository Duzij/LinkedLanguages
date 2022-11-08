using AnyAscii;
using LinkedLanguages.BL.CLLD;
using LinkedLanguages.BL.Query;
using LinkedLanguages.DAL;
using LinkedLanguages.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkedLanguages.BL.Services
{
    public static class WordPairResultsPipeline
    {
        public static IEnumerable<WordPair> FilterSameWords(this IEnumerable<WordPair> results)
        {
            return results.Where(wp => string.Compare(wp.UnknownWord, wp.KnownWord, ignoreCase: true) != 0);
        }

        public static IEnumerable<WordPair> FilterSuffixesAndPrefixes(this IEnumerable<WordPair> results)
        {
            return results.Where(
                    wp => wp.UnknownWord.First() != '-' && wp.UnknownWord.Last() != '-' &&
                    wp.KnownWord.First() != '-' && wp.KnownWord.Last() != '-');
        }

        public static IEnumerable<WordPair> FilterWordPairsWithHighCLLD(this IEnumerable<WordPair> results, Guid knownLangugageId, Guid unknownLanguageId)
        {
            results.ToList().ForEach((wp) =>
            {
                wp.KnownWordTransliterated = wp.KnownWord.Transliterate();
                wp.UnknownWordTransliterated = wp.UnknownWord.Transliterate();
                CalculateCLLD(knownLangugageId, unknownLanguageId, wp);
            });

            return results.Where(wp => wp.Distance < 3);
        }

        public static IEnumerable<WordPair> FilterTransliteratedWordsFromDatabase(this IEnumerable<WordPair> results, TransliteratedWordsDto[] databaseWords)
        {
            var returnedResults = new List<WordPair>();

            foreach (var wp in results)
            {
                if (databaseWords.Any(db => db.KnownWordTransliterated == wp.KnownWordTransliterated && db.UnknownWordTransliterated == wp.UnknownWordTransliterated))
                {
                    continue;
                }

                returnedResults.Add(wp);
            }

            return returnedResults;
        }

        public static IEnumerable<WordPair> FilterTransliteratedDuplicates(this IEnumerable<WordPair> results)
        {
            return results.Where(a => string.Compare(a.UnknownWordTransliterated, a.KnownWordTransliterated, ignoreCase: true) != 0)
                .DistinctBy(a => new { a.UnknownWordTransliterated, a.KnownWordTransliterated });
        }

        /// <summary>
        /// Static predefined dictionary of characters is used only for  German, French and English languages
        /// </summary>
        /// <param name="knownLangugageId"></param>
        /// <param name="unknownLanguageId"></param>
        /// <param name="wp"></param>
        private static void CalculateCLLD(Guid knownLangugageId, Guid unknownLanguageId, WordPair wp)
        {
            Dictionary<string, string> characterMapping = new Dictionary<string, string>();

            if (unknownLanguageId == LanguageSeed.FrenchLanguageId && knownLangugageId == LanguageSeed.EnglishLanguageId)
            {
                characterMapping = FrenchCharacterMapper.FrenchToEnglishMapping;
            }
            else if (unknownLanguageId == LanguageSeed.EnglishLanguageId && knownLangugageId == LanguageSeed.FrenchLanguageId)
            {
                characterMapping = FrenchCharacterMapper.EnglishToFrenchMapping;
            }
            if (unknownLanguageId == LanguageSeed.GermanLanguageId && knownLangugageId == LanguageSeed.EnglishLanguageId)
            {
                characterMapping = GermanCharacterMapper.GermanToEnglishMapping;
            }
            else if (unknownLanguageId == LanguageSeed.EnglishLanguageId && knownLangugageId == LanguageSeed.GermanLanguageId)
            {
                characterMapping = GermanCharacterMapper.EnglishToGermanMapping;
            }

            wp.Distance = CrossLingualLevenshteinDistanceCalculator.Calc(wp.KnownWordTransliterated, wp.UnknownWordTransliterated, characterMapping);
        }
    }
}
