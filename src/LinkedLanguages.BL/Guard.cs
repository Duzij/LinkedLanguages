using LinkedLanguages.DAL;
using System;
using System.Linq;

namespace LinkedLanguages.BL
{
    public static class Guard
    {
        public static void ThrowExceptionIfNotExists(ApplicationDbContext dbContext, Guid wordPairId)
        {
            if (!dbContext.WordPairs.Any(wp => wp.Id == wordPairId))
            {
                throw new InvalidOperationException($"Word pair with id {wordPairId} not found");
            }
        }

    }
}
