using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Exiger.JWT.Core.Exceptions
{
	[Serializable]
	public class ExigerException : Exception, ISerializable
	{
		private Guid _exceptionId = Guid.NewGuid();

		public Guid ExceptionId
		{
			get { return _exceptionId; }
		}

		public ExigerException(string message)
			: base(message)
		{
		}

		public ExigerException(string message, Exception inner)
			: base(message, inner)
		{
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("ExceptionId", this.ExceptionId);
		}
	}
}
