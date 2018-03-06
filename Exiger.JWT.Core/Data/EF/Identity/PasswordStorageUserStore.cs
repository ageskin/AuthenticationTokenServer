using Exiger.JWT.Core.Models;
using Exiger.JWT.Core.Utilities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Exiger.JWT.Core.Data.EF.Identity
{
    public class PasswordStorageUserStore : UserStore<ClientUser, ApplicationIdentityRole, string, ApplicationIdentityUserLogin, ApplicationIdentityUserRole, ApplicationIdentityUserClaim>
	{
        public PasswordStorageUserStore(DbContext context)
			: base(context)
		{
		}

		public override async Task CreateAsync(ClientUser user)
		{
			await base.CreateAsync(user);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Checked by Guard")]
		public Task AddToPreviousPasswordsAsync(ClientUser user, string password)
		{
			Guard.AgainstNull(user);
			return this.UpdateAsync(user);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Checked by Guard")]
		public override Task SetLockoutEndDateAsync(ClientUser user, DateTimeOffset lockoutEnd)
		{
			Guard.AgainstNull(user);
			return base.SetLockoutEndDateAsync(user, lockoutEnd);
		}
	}
}
