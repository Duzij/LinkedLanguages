
using System;

namespace LinkedLanguages.BL.DTO
{
    public class WordPairDto
    {
        public Guid Id { get; set; }
        public string UnknownWord { get; set; }
        public string KnownWord { get; set; }
    }
}
