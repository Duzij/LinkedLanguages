using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedLanguages.DAL.Models
{
    public class UnknownLanguageToUser
    {
        public int Id { get; set; }
        public Language Language { get; set; }
        public int LanguageId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int ApplicationUserId { get; set; }
    }
}
