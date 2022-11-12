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
        public void NullCharactersMappingTest()
        {
            int distance = CrossLingualLevenshteinDistanceCalculator.Calc("Goldschmidt", "Schmidt", GermanCharacterMapper.EnglishToGermanMapping);
            Console.WriteLine(distance);
        }

    }
}
