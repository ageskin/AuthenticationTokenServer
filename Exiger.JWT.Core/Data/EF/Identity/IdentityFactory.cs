using Exiger.JWT.Core.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace Exiger.JWT.Core.Data.EF.Identity
{
    public static class IdentityFactory
    {
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public static UserManager<ClientUser, string> CreateUserManager(DbContext context)
        {
            var manager = new PasswordStorageUserManager(new PasswordStorageUserStore(context));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ClientUser, string>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };
            return manager;
        }
    }
}
