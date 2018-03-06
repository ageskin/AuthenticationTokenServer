using System;

namespace Exiger.JWT.Core.Extensions
{
	public static class TypeExtensions
	{
		/// <summary>
		/// Determine whether a type is simple (String, Decimal, DateTime, etc) 
		/// or complex (i.e. custom class with public properties and methods).
		/// </summary>
		/// <see cref="http://stackoverflow.com/questions/2442534/how-to-test-if-type-is-primitive"/>
		public static bool IsSimpleType(
			this Type type)
		{
			return (type == typeof(object) 
				|| Type.GetTypeCode(type) != TypeCode.Object);
		}
	}
}
