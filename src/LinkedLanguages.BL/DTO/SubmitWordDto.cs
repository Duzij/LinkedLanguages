using System;

namespace LinkedLanguages.BL.DTO
{
    public record struct SubmitWordDto(Guid WordPairId, string SubmitedWord);
}
