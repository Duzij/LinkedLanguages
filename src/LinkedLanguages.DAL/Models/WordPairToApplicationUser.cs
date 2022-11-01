using System;

namespace LinkedLanguages.DAL.Models
{
    public class WordPairToApplicationUser
    {
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        public Guid WordPairId { get; set; }
        public WordPair WordPair { get; set; }
        public bool Rejected { get; set; }
        public bool Learned { get; set; }
        public int NumberOfFailedSubmissions { get; set; }
    }
}
