using LinkedLanguages.DAL.Models;
using System.Linq;

namespace LinkedLanguages.BL.Query
{
    public class ApprovedWordPairsQuery
    {
        private readonly WordPairsUserQuery wordPairsUserQuery;

        public ApprovedWordPairsQuery(WordPairsUserQuery wordPairsUserQuery)
        {
            this.wordPairsUserQuery = wordPairsUserQuery;
        }

        public IQueryable<WordPairToApplicationUser> GetQueryable(string knownLanguageCode, string unknownLanguageCode)
        {
            return wordPairsUserQuery
                .GetQueryable(knownLanguageCode, unknownLanguageCode)
                .Where(a => !a.Rejected);
        }
    }
}
