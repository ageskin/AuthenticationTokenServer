using Exiger.JWT.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Exiger.JWT.Core.Extensions
{
	public static class IEnumerableExtensions
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Checked by Guard"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Checked by Guard")]
		public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
		{
			Guard.AgainstNull(enumeration);
			Guard.AgainstNull(action);

			foreach (T item in enumeration)
			{
				action(item);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Checked by Guard")]
		public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T, int> action)
		{
			Guard.AgainstNull(action);

			for (int i = 0; i < enumeration.Count(); i++)
			{
				action(enumeration.Skip(i).First(), i);
			}
		}
	}
}
