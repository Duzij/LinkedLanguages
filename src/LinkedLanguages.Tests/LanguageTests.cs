using LinkedLanguages.DAL;

using NUnit.Framework;

using System;
using System.Globalization;
using System.Linq;

namespace LinkedLanguages.Tests
{
    public class LanguageTests
    {
        public string[] EtyTteeLangs { get; }

        public LanguageTests()
        {
            EtyTteeLangs = new string[]
            {
                "eng",
                "lat",
                "en",
                "deu",
                "fin",
                "fra",
                "ita",
                "hun",
                "xcl",
                "nld",
                "gem-pro",
                "rus",
                "non",
                "sla-pro",
                "jpn",
                "spa",
                "grc",
                "gle",
                "ine-pro",
                "hbs",
                "fro",
                "enm",
                "ang",
                "nob",
                "san",
                "ara",
                "swe",
                "por",
                "ces",
                "isl",
                "epo",
                "zho",
                "nno",
                "fas",
                "pol",
                "dan",
                "goh",
                "sga",
                "dum",
                "mul",
                "kor",
                "lav",
                "tur",
                "glv",
                "gla",
                "smi-pro",
                "tel",
                "ron",
                "gmh",
                "fiu-fin-pro",
                "ido",
                "hye",
                "vol",
                "cat",
                "vie",
                "ell",
                "frm",
                "hin",
                "tha",
                "gml",
                "kat",
                "heb",
                "odt",
                "ll",
                "fao",
                "ofs",
                "osx",
                "roa-opt",
                "slk",
                "trk-pro",
                "cel-pro",
                "nci",
                "msa",
                "la-vul",
                "orv",
                "lit",
                "sqi",
                "ceb",
                "nrf",
                "gmq-osw",
                "ota",
                "tgl",
                "cym",
                "est",
                "yid",
                "nav",
                "ltz",
                "poz-pro",
                "egy",
                "ml",
                "glg",
                "urj-pro",
                "dsb",
                "iir-pro",
                "vep",
                "got",
                "cel-bry-pro",
                "ira-pro",
                "frk",
                "pal",
                "itc-pro",
                "ine-bsl-pro",
                "chu",
                "mon",
                "pro",
                "ind",
                "sem-pro",
                "rup",
                "alg-pro",
                "khm",
                "sqj-pro",
                "osp",
                "sme",
                "mga",
                "sit-pro",
                "kur",
                "gmq-bot",
                "tai-pro",
                "ltc",
                "mya",
                "swa",
                "la-new",
                "xgn-pro",
                "poz-mly-pro",
                "sco",
                "bnt-pro",
                "nds-de",
                "pli",
                "bel",
                "map-pro",
                "psu",
                "syc",
                "xno",
                "lao",
                "bod",
                "slv",
                "jbo",
                "poz-pol-pro",
                "nds"
            };
        }

        [Test]
        public void RunAllTests()
        {
            var allLangs = CultureInfo
                    .GetCultures(CultureTypes.NeutralCultures);

            foreach (var item in EtyTteeLangs)
            {
                var lang = allLangs
                    .Where(ci => string.Equals(ci.ThreeLetterISOLanguageName, item, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (lang != null)
                { 
                    Console.WriteLine($"{lang.EnglishName}, {lang.NativeName}");
                }
                else
                {
                    Console.WriteLine($"{item} NOT FOUND");
                }
            }
        }
    }
}
