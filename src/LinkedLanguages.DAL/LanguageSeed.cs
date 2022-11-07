using LinkedLanguages.DAL.Models;

using System;
using System.Collections.Generic;

namespace LinkedLanguages.DAL
{
    public static class LanguageSeed
    {
        public static Guid EnglishLanguageId = Guid.Parse("bbeb43aa-8b3b-4902-a91c-68b08e396afe");
        public static Guid LatinLanguageId = Guid.Parse("19bc0378-b248-4f72-b827-ebd4e6818b56");
        public static Guid RussianLangaugeId = Guid.Parse("ac332cd9-895a-401f-b115-7e149bdb494b");
        public static Guid FrenchLanguageId = Guid.Parse("955d6d79-e96a-4cdf-b06e-dec4d995d309");
        public static Guid GermanLanguageId = Guid.Parse("af909fb3-f372-42c1-9ca7-8ace513bb8e5");
        public static Guid ItalianLanguageId = Guid.Parse("f4ed4153-e816-462f-b97f-a97c54d9dcc6");
        public static Guid SpanishLanguageId = Guid.Parse("beeab01b-7861-4759-942d-369f74a1f318");
        public static Guid PortugueseLanguageId = Guid.Parse("c3312d0d-5898-4dda-852a-383603c2ce71");
        public static Guid FinnishLanguageId = Guid.Parse("f00723bc-0313-44d7-9fe9-1cf4df2b45a9");
        public static Guid ChineseLanguageId = Guid.Parse("964cbdcb-d421-47d9-93c7-7518b6dca11e");

        /// <summary>
        /// Method returns top ten languages used in 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Language> GetStaticLanguages()
        {

            return new List<Language>() {
                new Language { Id = LatinLanguageId, Code = "lat", Name="Latin" },
                new Language { Id = EnglishLanguageId, Code = "eng", Name="English" },
                new Language { Id = ItalianLanguageId, Code = "ita", Name="Italian" },
                new Language { Id = SpanishLanguageId, Code = "spa", Name="Castilian, Spanish" },
                new Language { Id = FrenchLanguageId, Code = "fra", Name="French" },
                new Language { Id = RussianLangaugeId, Code = "rus", Name="Russian" },
                new Language { Id = GermanLanguageId, Code = "deu", Name="German" },
                new Language { Id = PortugueseLanguageId, Code = "por", Name="Portuguese" },
                new Language { Id = FinnishLanguageId, Code = "fin", Name="Finnish" },
                new Language { Id = ChineseLanguageId, Code = "zho", Name="Chinese" }
            };
        }
    }
}
