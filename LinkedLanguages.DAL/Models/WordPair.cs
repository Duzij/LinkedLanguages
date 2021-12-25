using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedLanguages.DAL.Models
{
    public class WordPair
    {
        public string ForeignWord { get; set; }

        public Language ForeignLanguage { get; set; }
        public string KnownWord { get; set; }
        public Language KnownLanguage { get; set; }
    }
}
