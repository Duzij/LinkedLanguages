using System;

namespace LinkedLanguages.BL.DTO
{
    public readonly record struct LanguageDto
    (
        Guid Value,
        string Label
    );
}
