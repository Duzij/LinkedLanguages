using System.Collections.Generic;

namespace LinkedLanguages.BL.CLLD
{
    public static class FrenchCharacterMapper
    {
        public static readonly Dictionary<string, string> EnglishToFrenchMapping = new Dictionary<string, string>()
            {
                {"a","â" },
                {"c",  "ch" },
                {"k",  "que"},
                {"hy",  "é, e"},
                {"y",  "ie"},
                {"u",  "o, ou"},
                {"st",  "t"}
            };

        public static readonly Dictionary<string, string> FrenchToEnglishMapping = new Dictionary<string, string>()
            {
               { "â",  "a" },
               { "ch",  "c"},
               { "que",  "k"},
               { "é, e", "hy"},
               { "ie", "y"},
               { "o, ou", "u"},
               { "t", "st"}
            };

    }

    public static class GermanCharacterMapper
    {
        public static readonly Dictionary<string, string> EnglishToGermanMapping = new Dictionary<string, string>()
            {
                {"u",  "ü" },
                {"o",  "ö"},
                {"a",  "ä"},
                {"th",  "d" },
                {"d",  "t"},
                {"c",  "k"},
                {"f",  "b,v"},
                {"p",  "f"},
                {"pp",  "p"},
                {"x",  "chs"},
                {"sh",  "sch"},
                {"s",  "sch"},
                {"v",  "b"},
                {"t",  "ss"},
                {"n",  "nn"}
            };

        public static readonly Dictionary<string, string> GermanToEnglishMapping = new Dictionary<string, string>()
            {
                { "ü" ,"u" },
                { "ö","o"},
                { "ä","a"},
                { "d" ,"th"},
                { "t","d"},
                { "k","c"},
                { "b,v","f"},
                { "f","p"},
                { "p","pp"},
                { "chs","x"},
                { "sch","s, sh"},
                { "b","v"},
                { "ss","t"},
                { "nn","n"}
            };
    }
}
