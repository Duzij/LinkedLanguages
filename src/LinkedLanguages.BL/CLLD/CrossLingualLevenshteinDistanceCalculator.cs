using System;
using System.Collections.Generic;

namespace LinkedLanguages.BL.CLLD
{
    public static class CrossLingualLevenshteinDistanceCalculator
    {
        public static int Calc(string wordA, string wordB, Dictionary<string, string> charactersMapping)
        {
            if (string.IsNullOrEmpty(wordA) && string.IsNullOrEmpty(wordB))
            {
                return 0;
            }
            if (string.IsNullOrEmpty(wordA))
            {
                return wordB.Length;
            }
            if (string.IsNullOrEmpty(wordB))
            {
                return wordA.Length;
            }
            wordA = wordA.ToLower();
            wordB = wordB.ToLower();

            foreach (KeyValuePair<string, string> item in charactersMapping)
            {
                if (wordA.Contains(item.Key))
                {
                    int index = wordA.IndexOf(item.Key);

                    string[] mappings = item.Value.Split('\u002C');

                    foreach (string mappingB in mappings)
                    {
                        int index2 = wordB.IndexOf(mappingB);
                        if (wordB.Contains(mappingB) && index2 == index)
                        {
                            wordA = wordA.Replace(item.Key, "");
                            wordB = wordB.Replace(mappingB, "");
                        }
                    }
                }
            }

            int lengthA = wordA.Length;
            int lengthB = wordB.Length;
            int[,] distances = new int[lengthA + 1, lengthB + 1];

            // Step 2
            for (int i = 0; i <= lengthA; distances[i, 0] = i++)
            {
            }

            for (int j = 0; j <= lengthB; distances[0, j] = j++)
            {
            }

            for (int i = 1; i <= lengthA; i++)
            {
                for (int j = 1; j <= lengthB; j++)
                {
                    bool isMatch = wordB[j - 1] == wordA[i - 1];
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

    }
}
