using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedLanguages.DAL.Models
{
    public class WordPairToApplicationUser
    {
        public int Id { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public int ApplicationUserId { get; set; }

        public int WordPairId { get; set; }
        public WordPair WordPair { get; set; }
    }
}
