using System;

namespace LinkedLanguages.BL.DTO
{
    public readonly record struct WordPairDto
    (
        Guid Id,
        string UnknownWord,
        string KnownWord
    );
}
