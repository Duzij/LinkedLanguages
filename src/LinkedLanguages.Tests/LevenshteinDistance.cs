using NUnit.Framework;
using System;
using System.Linq;

namespace LinkedLanguages.Tests
{

    public static class LevenshteinDistance
    {
        public static int Distance(string a, string b)
        {
            Console.WriteLine($"{a}, {b}");
            return string.IsNullOrWhiteSpace(a)
                ? b.Length
                : string.IsNullOrWhiteSpace(b)
                ? a.Length
                : a[0] == b[0]
                ? Distance(RemoveFirstChar(a), RemoveFirstChar(b))
                : 1 + new[] {
                    Distance(RemoveFirstChar(a), b),
                    Distance(a, RemoveFirstChar(b)),
                    Distance(RemoveFirstChar(a), RemoveFirstChar(b))
                }.Min();
        }

        private static string RemoveFirstChar(string b)
        {
            return b[1..];
        }
    }

    public class LevenshteinTests
    {
        [Test]
        public void Test1()
        {
            int distance = LevenshteinDistance.Distance("kitten", "sitting");
            Assert.That(distance, Is.EqualTo(3));
        }
    }
}
