using Microsoft.Owin.Security.DataProtection;

namespace Exiger.JWT.Core.Data.EF.Identity
{
	public class MachineKeyProtectionProvider : IDataProtectionProvider
	{
		public IDataProtector Create(params string[] purposes)
		{
			return new MachineKeyDataProtector(purposes);
		}
	}
}
