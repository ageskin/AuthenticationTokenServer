using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Exiger.JWT.Core.Exceptions
{
	[Serializable]
	public class ExigerDbConcurrencyException : ExigerException
	{
		public bool RecordsDeleted { get; set; }

		public ExigerDbConcurrencyException(string message)
			: base(message) { }

		public ExigerDbConcurrencyException(string message, Exception inner)
			: base(message, inner) { }

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("RecordsDeleted", this.RecordsDeleted);
		}
	}
}
