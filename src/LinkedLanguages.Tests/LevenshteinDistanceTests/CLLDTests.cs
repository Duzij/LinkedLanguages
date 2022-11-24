using LinkedLanguages.BL.CLLD;

using NUnit.Framework;

using System;

namespace LinkedLanguages.Tests.LevenshteinDistanceTests
{
    public class CLLDTests
    {

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void GermanCharactersMappingTest()
        {
            int distance = CrossLingualLevenshteinDistanceCalculator.Calc("shmidt", "Schmidt", GermanCharacterMapper.EnglishToGermanMapping);
            int distance1 = CrossLingualLevenshteinDistanceCalculator.Calc("smith", "Schmith", GermanCharacterMapper.EnglishToGermanMapping);
            int distance2 = CrossLingualLevenshteinDistanceCalculator.Calc("smith", "Schmid", GermanCharacterMapper.EnglishToGermanMapping);
            Console.WriteLine(distance);
            Console.WriteLine(distance1);
            Console.WriteLine(distance2);
        }

        [Test]
        public void GermanCharactersMappingTestTwo()
        {
            int distance = CrossLingualLevenshteinDistanceCalculator.Calc("Hoffman", "Hoffmann", GermanCharacterMapper.EnglishToGermanMapping);
            Console.WriteLine(distance);
        }
    }
}
