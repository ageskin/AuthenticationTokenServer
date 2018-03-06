using Exiger.JWT.Core.Models;
using Exiger.JWT.Core.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;

namespace Exiger.JWT.Core.Data.EF.Identity
{
	public class PasswordStorageUserManager: UserManager<ClientUser, string>
	{
        //private const string PASSWORD_HISTORY_LIMIT = "Exiger.JWT.Core.Utilities.Constants.PasswordHistoryLimit";


        public PasswordStorageUserManager(PasswordStorageUserStore store)
			: base(store)
		{
			this.UserLockoutEnabledByDefault = true;
			this.MaxFailedAccessAttemptsBeforeLockout = ConfigurationHelper.GetInt32Setting(Utilities.Constants.LOGIN_ATTEMPTS);
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(ConfigurationHelper.GetInt32Setting(Utilities.Constants.TIMEOUT_PERIOD_IN_MINUTES));

            // Configure validation logic for passwords
			this.PasswordValidator = new PasswordValidator();

			//Add token provider for providing reset tokens
			MachineKeyProtectionProvider provider = new MachineKeyProtectionProvider();
			this.UserTokenProvider = new DataProtectorTokenProvider<ClientUser, string>(provider.Create("ResetPasswordPurpose"));
		}

		public override async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
		{
			if (await this.IsPreviousPassword(userId, newPassword))
			{
				return await Task.FromResult(IdentityResult.Failed("Cannot reuse any of your previous (12) passwords"));
			}

			IdentityResult result = await base.ChangePasswordAsync(userId, currentPassword, newPassword);

			if (result.Succeeded)
			{
				PasswordStorageUserStore store = this.Store as PasswordStorageUserStore;
				await store.AddToPreviousPasswordsAsync(await this.FindByIdAsync(userId), this.PasswordHasher.HashPassword(newPassword));
			}

			return result;
		}

		public override async Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword)
		{
			if (await this.IsPreviousPassword(userId, newPassword))
			{
                return await Task.FromResult(IdentityResult.Failed("Cannot reuse any of your previous (12) passwords"));
			}

			IdentityResult result = await base.ResetPasswordAsync(userId, token, newPassword);

			if (result.Succeeded)
			{
				PasswordStorageUserStore store = this.Store as PasswordStorageUserStore;
				await store.AddToPreviousPasswordsAsync(await this.FindByIdAsync(userId), this.PasswordHasher.HashPassword(newPassword));
			}

			return result;
		}

		private async Task<bool> IsPreviousPassword(string userId, string newPassword)
		{
            return await Task.FromResult(false);
		}
	}
}
