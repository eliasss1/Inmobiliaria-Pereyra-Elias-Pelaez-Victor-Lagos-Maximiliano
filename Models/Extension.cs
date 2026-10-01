using System.Security.Claims;
using System.Security.Principal;

namespace Inmobiliaria.Extension
{
    public static class IdentityExtensions
    {
        public static int GetUserId(this IPrincipal user)
        {
            if (user == null || !user.Identity.IsAuthenticated)
                return 0;

            var claimsIdentity = user.Identity as ClaimsIdentity;
            
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null && int.TryParse(claim.Value, out int id))
            {
                return id;
            }

            return 0;
        }
    }
}