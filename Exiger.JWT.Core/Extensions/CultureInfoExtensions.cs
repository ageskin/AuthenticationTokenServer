using System.Globalization;

namespace Exiger.JWT.Core.Extensions
{
	public static class CultureInfoExtensions
	{
		public static string GetDateTimeStandardFormat(this CultureInfo info)
		{
			return info.DateTimeFormat.ShortDatePattern + ' ' + info.DateTimeFormat.LongTimePattern;
		}
	}
}
