using System;
using System.Collections.Generic;

namespace LinkedLanguages.BL.DTO
{
    public readonly record struct UserProfileDto
    (
        Guid UserId,
        List<LanguageDto> KnownLanguages,
        List<LanguageDto> UnknownLanguages
    );
}
