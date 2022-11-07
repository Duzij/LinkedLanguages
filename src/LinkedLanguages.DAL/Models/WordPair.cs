using Microsoft.EntityFrameworkCore;
using System;

namespace LinkedLanguages.DAL.Models
{
    [Index(nameof(KnownWordTransliterated), nameof(UnknownWordTransliterated))]
    public class WordPair
    {
        public Guid Id { get; set; }
        public string KnownLanguageCode { get; set; }
        public Language KnownLanguage { get; set; }
        public Guid KnownLanguageId { get; set; }
        public string KnownWord { get; set; }
        public string KnownWordUri { get; set; }
        public string KnownWordTransliterated { get; set; }
        public string UnknownWordTransliterated { get; set; }
        public string UnknownLanguageCode { get; set; }
        public Guid UnknownLanguageId { get; set; }
        public Language UnknownLanguage { get; set; }
        public string UnknownWord { get; set; }
        public string UnknownWordUri { get; set; }
        public int Distance { get; set; }
    }
}
