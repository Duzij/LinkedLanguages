using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedLanguages.DAL.Models
{
    public class KnownLanguageToUser
    {
        public Guid Id { get; set; }
        public Language Language { get; set; }
        public Guid LanguageId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Guid ApplicationUserId { get; set; }
    }
}
