
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedLanguages.BL.User
{
    public interface IAppUserProvider
    {
        public Guid GetUserId();
        string GetUserKnownLanguage();
    }
}
