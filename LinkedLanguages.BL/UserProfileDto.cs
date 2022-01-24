
using System;
using System.Collections.Generic;

namespace LinkedLanguages.BL
{
    public class UserProfileDto
    {
        public Guid UserId { get; set; }
        public List<LanguageDto> KnownLanguages { get; set; }
        public List<LanguageDto> UnknownLanguages { get; set; }
    }
}
