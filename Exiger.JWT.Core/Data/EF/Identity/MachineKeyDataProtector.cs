using Exiger.JWT.Core.Utilities;
using Microsoft.Owin.Security.DataProtection;
using System.Web.Security;

namespace Exiger.JWT.Core.Data.EF.Identity
{
	public class MachineKeyDataProtector : IDataProtector
	{
		private readonly string[] _purposes;

		public MachineKeyDataProtector(string[] purposes)
		{
			this._purposes = Guard.AgainstNull(purposes);
		}

		public byte[] Protect(byte[] userData)
		{
			return MachineKey.Protect(userData, this._purposes);
		}

		public byte[] Unprotect(byte[] protectedData)
		{
			return MachineKey.Unprotect(protectedData, this._purposes);
		}
	}
}
