using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkedLanguages.BL.CLLD
{
    public static class CrossLingualLevenshteinDistanceCalculator
    {
        public static int Calc(string a, string b, Dictionary<string, string> charactersMapping)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
            {
                return 0;
            }
            if (string.IsNullOrEmpty(a))
            {
                return b.Length;
            }
            if (string.IsNullOrEmpty(b))
            {
                return a.Length;
            }
            int lengthA = a.Length;
            int lengthB = b.Length;
            int[,] distances = new int[lengthA + 1, lengthB + 1];

            for (int i = 1; i <= lengthA; i++)
            {
                for (int j = 1; j <= lengthB; j++)
                {
                    bool isMatch = AreCharactersEqual(b[j - 1].ToString(), a[i - 1].ToString(), charactersMapping);
                    int cost = isMatch ? 0 : 1;
                    distances[i, j] = Math.Min
                    (
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                    );
                }
            }

            return distances[lengthA, lengthB];
        }

        private static bool AreCharactersEqual(string v1, string v2, Dictionary<string, string> characterMapping)
        {
            if (v1 == v2)
            {
                return true;
            }
            else
            {
                if (characterMapping is not null)
                {
                    if (characterMapping.TryGetValue(v1, value: out string languageCharactersEquivalent))
                    {
                        string[] mappings = languageCharactersEquivalent.Split('\u002C');
                        if (mappings.ToList().Contains(v2))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }
    }
}
