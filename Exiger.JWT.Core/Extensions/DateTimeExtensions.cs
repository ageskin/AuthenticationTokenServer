using System;
using System.Globalization;

namespace Exiger.JWT.Core.Extensions
{
    public static class DateTimeExtensions
	{
		public const string EASTERN_TIME_ZONE_ID = "Eastern Standard Time";
		public static readonly TimeZoneInfo EasternTimeZone = TimeZoneInfo.FindSystemTimeZoneById(DateTimeExtensions.EASTERN_TIME_ZONE_ID);

		public const string ENGLISH_US_CULTURE_NAME = "en-US";
		public static readonly CultureInfo EnglishUSCulture = CultureInfo.GetCultureInfo(DateTimeExtensions.ENGLISH_US_CULTURE_NAME);

		/// <summary>
		/// Converts an Eastern DateTime to a UTC DateTime.
		/// </summary>
		/// <param name="easternDateTime">A DateTime object in Eastern time</param>
		/// <returns>a DateTime object in UTC</returns>
		public static DateTime FromEasternToUtcTime(this DateTime easternDateTime)
		{
			return easternDateTime.ToUtc();
		}

		/// <summary>
		/// Converts a UTC DateTime to Eastern DateTime.
		/// </summary>
		/// <param name="utcDateTime">A DateTime object in UTC</param>
		/// <returns>A DateTime object in Eastern time</returns>
		/// <remarks>This should only be used for presentaion puproses.</remarks>
		public static DateTime FromUtcToEasternTime(this DateTime utcDateTime)
		{
			return utcDateTime.FromUtc();
		}

		public static DateTime FromUtc(this DateTime utcDateTime, int utcOffsetMinutes)
		{
            var localTime = new DateTime(utcDateTime.AddMinutes(utcOffsetMinutes).Ticks, DateTimeKind.Local);
			return localTime;
		}

		public static DateTime FromUtc(this DateTime utcDateTime, TimeZoneInfo destinationTimeZone = null)
		{
			destinationTimeZone = destinationTimeZone ?? DateTimeExtensions.EasternTimeZone;

			return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, destinationTimeZone);
		}

		public static DateTime ToUtc(this DateTime dateTime, TimeZoneInfo sourceTimeZone = null)
		{
			sourceTimeZone = sourceTimeZone ?? DateTimeExtensions.EasternTimeZone;

			return TimeZoneInfo.ConvertTimeToUtc(dateTime, sourceTimeZone);
		}

		public static string ToString(this DateTime dateTime, CultureInfo info = null)
		{
			info = info ?? DateTimeExtensions.EnglishUSCulture;

			return dateTime.ToString(info);
		}

        public static DateTime AddWeeks(this DateTime dateTime, int numberOfWeeks)
        {
            return dateTime.AddDays(numberOfWeeks * 7);
        }
    }
}
