using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Exiger.JWT.Core.Utilities;

namespace Exiger.JWT.Core.Data.EF.Identity
{
	internal class PasswordValidator : IIdentityValidator<string>
	{
		private const int MINIMUM_PASSWORD_LENGTH = Utilities.Constants.PasswordMinimumRequiredLength;
		private static readonly Regex PASSWORD_REGEX = new Regex(Utilities.Constants.RegexPassword, RegexOptions.Compiled);

		public Task<IdentityResult> ValidateAsync(string item)
		{
			Guard.AgainstNull(item, "item");

			List<string> results = new List<string>();

			//Check password length
			if (string.IsNullOrWhiteSpace(item) || item.Length < PasswordValidator.MINIMUM_PASSWORD_LENGTH)
			{
				results.Add(string.Format("Password must be at least {0} characters", PasswordValidator.MINIMUM_PASSWORD_LENGTH));
			}

			//Check that item matches the required regular expression
			if (!PasswordValidator.PASSWORD_REGEX.IsMatch(item))
			{
				results.Add(string.Format("The password must be at least {0} characters and contain a lowercase letter [a-z], an upper case letter [A-Z], and at least one digit or punctuation mark", PasswordValidator.MINIMUM_PASSWORD_LENGTH));
			}

			//Success
			if (results.Count == 0)
			{
				return Task.FromResult<IdentityResult>(IdentityResult.Success);
			}

			return Task.FromResult<IdentityResult>(IdentityResult.Failed(results.ToArray()));
		}
	}
}
