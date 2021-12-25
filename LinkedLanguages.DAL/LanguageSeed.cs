using LinkedLanguages.DAL.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedLanguages.DAL
{
    public static class LanguageSeed
    {
        /// <summary>
        /// Method returns top ten languages used in 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Language> GetStaticLanguages()
        {
           return new List<Language>() { 
                new Language { Id = 1, Code = "lat", Name="Latin" },
                new Language { Id = 2, Code = "eng", Name="English" },
                new Language { Id = 3, Code = "ita", Name="Italian" },
                new Language { Id = 4, Code = "spa", Name="Castilian, Spanish" },
                new Language { Id = 5, Code = "fra", Name="French" },
                new Language { Id = 6, Code = "rus", Name="Russian" },
                new Language { Id = 7, Code = "deu", Name="German" },
                new Language { Id = 8, Code = "por", Name="Portuguese" },
                new Language { Id = 9, Code = "fin", Name="Finnish" },
                new Language { Id = 10,Code = "zho", Name="Chinese" }
            };

        }
    }
}
