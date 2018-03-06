using System;
using System.Security.Claims;
using System.Web;

namespace Exiger.JWT.Core.Utilities
{
    public static class WebContextHelper
    {
        public static int CurrentLoggedInUserId()
        {
            ClaimsIdentity claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
            Claim claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                return Convert.ToInt32(claim.Value);
            }
            return 0;
        }
    }
}
