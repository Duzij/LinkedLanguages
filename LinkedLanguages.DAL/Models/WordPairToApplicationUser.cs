using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedLanguages.DAL.Models
{
    public class WordPairToApplicationUser
    {
        public Guid Id { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public Guid ApplicationUserId { get; set; }

        public Guid WordPairId { get; set; }
        public WordPair WordPair { get; set; }
    }
}
