using LinkedLanguages.BL.Languages;
using LinkedLanguages.Resources;
using NUnit.Framework;

using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace LinkedLanguages.Tests
{
    public class LanguageGeneratorTests
    {
        public string[] EtyTteeLangs { get; }

        public LanguageGeneratorTests()
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

        [Ignore("")]
        public void ReadFromFileTest()
        {
            CultureInfo
                    .GetCultures(CultureTypes.NeutralCultures);

            foreach (string item in EtyTteeLangs)
            {
                const string separator = "|";

                Stream stream = typeof(Resource).GetTypeInfo().Assembly.GetManifestResourceStream("LinkedLanguages.Resources.ISO-639-2_utf-8.txt");
                using StreamReader reader = new StreamReader(stream);
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    //two-letter ISO code is in the third column, i.e. after the second separator character
                    string threeLetterISOCode = line[..3];

                    if (item == threeLetterISOCode)
                    {
                        string[] thirdSeparator = line.Split(separator);
                        Console.WriteLine(thirdSeparator[3]);
                    }
                }
            }
        }

        [Ignore("")]
        public void UseCodeGenerator()
        {
            foreach (string item in EtyTteeLangs)
            {
                if (Enum.TryParse(item, out ISOLanguages _))
                {
                    //Console.WriteLine($"{item}:{result.GetDisplayName()}");
                }
                else
                {
                    Console.WriteLine($"{item}: NOT FOUND");
                }
            }
        }

    }
}
