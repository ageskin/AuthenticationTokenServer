using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Exiger.JWT.Core.Exceptions
{
	[Serializable]
	public class ExigerValidationException : ExigerException, ISerializable
	{
		private List<ValidationResult> _validationResults = new List<ValidationResult>();

		public IEnumerable<ValidationResult> ValidationResults
		{
			get { return _validationResults; }
		}

		public ExigerValidationException(string message)
			: base(message) { }

		public ExigerValidationException(string message, Exception inner)
			: base(message, inner) { }

		public ExigerValidationException(string message, Exception inner, IEnumerable<ValidationResult> validationResults)
			: base(message, inner)
		{
			if (validationResults != null)
			{
				this._validationResults.AddRange(validationResults);
			}
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			foreach (ValidationResult result in this.ValidationResults)
			{
				if (result != null)
				{
					foreach (string memberName in result.MemberNames)
					{
						info.AddValue(memberName, result.ErrorMessage);
					}
				}
			}
		}
	}
}
