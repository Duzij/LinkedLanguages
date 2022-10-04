using System;

namespace LinkedLanguages.DAL.Models
{
    public class UnknownLanguageToUser
    {
        public Guid Id { get; set; }
        public Language Language { get; set; }
        public Guid LanguageId { get; set; }
        public Guid ApplicationUserId { get; set; }
    }
}
