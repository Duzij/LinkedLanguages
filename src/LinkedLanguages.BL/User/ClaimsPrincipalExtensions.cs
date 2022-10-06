
using System;
using System.Linq;
using System.Security.Claims;

namespace LinkedLanguages.BL.User
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            var claim = principal.Claims.ToList().FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            return claim == null
                ? throw new InvalidOperationException("UserId claim not found")
                : claim.Value == Guid.Empty.ToString()
                ? throw new InvalidOperationException("UserId claim is empty Guid")
                : Guid.Parse(claim.Value);
        }
    }
}
