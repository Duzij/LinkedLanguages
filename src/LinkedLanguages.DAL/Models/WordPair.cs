using System;

namespace LinkedLanguages.DAL.Models
{
    public class WordPair
    {
        public Guid Id { get; set; }

        public string KnownLanguageCode { get; set; }
        public Guid KnownLanguageId { get; set; }
        public string KnownWord { get; set; }
        public string KnownWordUri { get; set; }

        public string UnknownLanguageCode { get; set; }
        public Guid UnknownLanguageId { get; set; }
        public string UnknownWord { get; set; }
        public string UnknownWordUri { get; set; }


    }
}
