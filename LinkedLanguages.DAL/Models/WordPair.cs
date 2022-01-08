using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedLanguages.DAL.Models
{
    public class WordPair
    {
        public Guid Id { get; set; }

        public string KnownLanguage { get; set; }
        public Guid KnownLanguageId { get; set; }
        public string KnownWord { get; set; }

        public string UnknownLanguageCode { get; set; }
        public Guid UnknownLanguageId { get; set; }
        public string UnknownWord { get; set; }

    }
}
