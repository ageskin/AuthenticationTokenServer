using Microsoft.AspNet.Identity;

namespace Exiger.JWT.Core.Data.EF.Identity
{
    public static class IdentityExtensions
    {
        public static AppIdentityResult ToAppIdentityResult(this IdentityResult identityResult)
        {
            return identityResult == null ? null : new AppIdentityResult(identityResult.Errors, identityResult.Succeeded);
        }
    }
}
