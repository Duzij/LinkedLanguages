using System.Collections.Generic;
using System.Linq;

namespace LinkedLanguages.BL.CLLD
{
    public static class CrossLingualLevenshteinDistanceCalculator
    {
        public static int[,] GetMatrix(string wordA, string wordB, Dictionary<string, string> charactersMapping)
        {
            wordA = wordA.ToLower();
            wordB = wordB.ToLower();

            int lengthA = wordA.Length;
            int lengthB = wordB.Length;
            int[,] distances = new int[lengthA + 1, lengthB + 1];

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
                    distances[i, j] = new int[3]
                    {
                        WordADeletion(wordA, wordB, i, j, charactersMapping, distances),
                        WordBDeletion(wordA, wordB, i, j, charactersMapping, distances),
                        Mismatch(wordA, wordB, i, j, charactersMapping, distances)
                    }.Min();
                }
            }
            return distances;
        }

        public static int Calc(string wordA, string wordB, Dictionary<string, string> charactersMapping)
        {
            var lengthA = wordA.Length;
            var lengthB = wordB.Length;
            return GetMatrix(wordA, wordB, charactersMapping)[lengthA, lengthB];
        }

        private static int WordADeletion(string wordA, string wordB, int wordAIndex, int wordBIndex, Dictionary<string, string> charactersMapping, int[,] distances)
        {
            var substringA = wordA[..(wordAIndex - 1)];
            var substringB = wordB[..wordBIndex];

            var cost = 1;
            if (!string.IsNullOrEmpty(substringA) && !string.IsNullOrEmpty(substringB))
            {
                var sourceNullMappings = charactersMapping.Where(a => a.Value == CharacterMapperConstant.NULL);
                foreach (KeyValuePair<string, string> item in sourceNullMappings)
                {
                    string[] mappings = item.Value.Split('\u002C');
                    foreach (string mappingA in mappings)
                    {
                        if (substringA.Contains(mappingA))
                        {
                            var isAtTheEndA = substringA.Substring(substringA.Length - item.Key.Length, item.Key.Length);
                            if (isAtTheEndA == mappingA)
                            {
                                cost = 0;
                                break;
                            }
                        }
                    }
                }
            }

            return distances[wordAIndex - 1, wordBIndex] + cost;
        }

        private static int WordBDeletion(string wordA, string wordB, int wordAIndex, int wordBIndex, Dictionary<string, string> charactersMapping, int[,] distances)
        {
            var substringA = wordA[..wordAIndex];
            var substringB = wordB[..(wordBIndex - 1)];

            var cost = 1;
            if (!string.IsNullOrEmpty(substringA) && !string.IsNullOrEmpty(substringB))
            {
                var nullMappings = charactersMapping.Where(a => a.Key == CharacterMapperConstant.NULL);
                foreach (KeyValuePair<string, string> item in nullMappings)
                {
                    string[] mappings = item.Value.Split('\u002C');

                    foreach (string mappingB in mappings)
                    {
                        if (substringB.Contains(mappingB))
                        {
                            var isAtTheEndB = substringB.Substring(substringB.Length - mappingB.Length, mappingB.Length);
                            if (isAtTheEndB == mappingB)
                            {
                                cost = 0;
                                break;
                            }
                        }
                    }
                }
            }

            return distances[wordAIndex, wordBIndex - 1] + cost;
        }

        public static int Mismatch(string wordA, string wordB, int wordAIndex, int wordBIndex, Dictionary<string, string> charactersMapping, int[,] distances)
        {
            var charB = wordB[wordBIndex - 1];
            var charA = wordA[wordAIndex - 1];
            var substringA = wordA[..(wordAIndex - 1)];
            var substringB = wordB[..(wordBIndex - 1)];

            if (!string.IsNullOrEmpty(substringA) && !string.IsNullOrEmpty(substringB))
            {
                foreach (KeyValuePair<string, string> item in charactersMapping)
                {
                    if (substringA.Contains(item.Key))
                    {
                        var isAtTheEndA = substringA.Substring(substringA.Length - item.Key.Length, item.Key.Length);
                        if (isAtTheEndA == item.Key)
                        {
                            string[] mappings = item.Value.Split('\u002C');

                            foreach (string mappingB in mappings)
                            {
                                if (substringB.Contains(mappingB))
                                {
                                    var isAtTheEndB = substringB.Substring(substringB.Length - mappingB.Length, mappingB.Length);
                                    if (isAtTheEndB == mappingB)
                                    {
                                        return distances[wordAIndex - item.Key.Length, wordBIndex - mappingB.Length];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var cost = charA == charB ? 0 : 1;
            return distances[wordAIndex - 1, wordBIndex - 1] + cost;
        }

    }
}
