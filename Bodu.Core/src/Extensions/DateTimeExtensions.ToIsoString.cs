// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.ToIsoString.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Converts the specified <see cref="DateTime" /> to an ISO 8601 formatted string, respecting the <see cref="DateTime.Kind" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to convert.</param>
		/// <returns>
		/// A string representation of <paramref name="dateTime" /> in ISO 8601 format:
		/// <list type="bullet">
		/// <item>
		/// <description>If <see cref="DateTime.Kind" /> is <see cref="DateTimeKind.Utc" />, the result ends with 'Z'.</description>
		/// </item>
		/// <item>
		/// <description>
		/// If <see cref="DateTime.Kind" /> is <see cref="DateTimeKind.Local" />, the result includes the local time zone offset.
		/// </description>
		/// </item>
		/// <item>
		/// <description>
		/// If <see cref="DateTime.Kind" /> is <see cref="DateTimeKind.Unspecified" />, the result omits any time zone information.
		/// </description>
		/// </item>
		/// </list>
		/// </returns>
		/// <remarks>
		/// Uses the "o" (round-trip) format for Local and Unspecified kinds, and "yyyy-MM-ddTHH:mm:ss.fffffffZ" for Utc to enforce ISO 8601
		/// 'Z' suffix.
		/// </remarks>
		public static string ToIsoString(this DateTime dateTime)
		{
			return dateTime.Kind switch
			{
				DateTimeKind.Utc => dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture),
				DateTimeKind.Local => dateTime.ToString("o", CultureInfo.InvariantCulture),
				_ => DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified).ToString("o", CultureInfo.InvariantCulture),
			};
		}

		/// <summary>
		/// Converts the specified <see cref="DateTime" /> to an ISO 8601 formatted string, allowing fractional seconds to be omitted.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to convert.</param>
		/// <param name="includeFractionalSeconds">Whether to include fractional seconds (7 digits) in the result.</param>
		/// <returns>A string representation of <paramref name="dateTime" /> in ISO 8601 format, with or without fractional seconds.</returns>
		/// <remarks>
		/// When <paramref name="includeFractionalSeconds" /> is <c>true</c>, uses the "o" (round-trip) format; otherwise, uses "yyyy-MM-ddTHH:mm:ss".
		/// </remarks>
		public static string ToIsoString(this DateTime dateTime, bool includeFractionalSeconds)
		{
			string format = includeFractionalSeconds ? "o" : "yyyy-MM-ddTHH:mm:ss";
			return dateTime.ToString(format, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts the specified <see cref="DateTime" /> to an ISO 8601 formatted string, using an explicit <see cref="DateTimeKind" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to convert.</param>
		/// <param name="kind">The <see cref="DateTimeKind" /> to apply before formatting.</param>
		/// <returns>
		/// A string representation of the input in ISO 8601 format, formatted as:
		/// <list type="bullet">
		/// <item>
		/// <description><see cref="DateTimeKind.Utc" /> → UTC with trailing 'Z'.</description>
		/// </item>
		/// <item>
		/// <description><see cref="DateTimeKind.Local" /> → Local time with offset.</description>
		/// </item>
		/// <item>
		/// <description><see cref="DateTimeKind.Unspecified" /> → No time zone indicator.</description>
		/// </item>
		/// </list>
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="kind" /> is not a valid <see cref="DateTimeKind" /> value.
		/// </exception>
		public static string ToIsoString(this DateTime dateTime, DateTimeKind kind)
		{
			return kind switch
			{
				DateTimeKind.Utc => dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture),
				DateTimeKind.Local => dateTime.ToLocalTime().ToString("o", CultureInfo.InvariantCulture),
				DateTimeKind.Unspecified => DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified).ToString("o", CultureInfo.InvariantCulture),
				_ => throw new ArgumentOutOfRangeException(nameof(kind), "Invalid DateTimeKind selectedDays.")
			};
		}

		/// <summary>
		/// Converts the specified <see cref="DateTime" /> to a formatted string using a custom format and optional culture.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to format.</param>
		/// <param name="format">A valid custom format string (e.g., "yyyy-MM-ddTHH:mm:ss").</param>
		/// <param name="culture">
		/// An optional <see cref="CultureInfo" /> to apply. If <c>null</c>, <see cref="CultureInfo.InvariantCulture" /> is used.
		/// </param>
		/// <returns>A string formatted using the specified pattern and culture.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="format" /> is null or empty.</exception>
		public static string ToIsoString(this DateTime dateTime, string format, CultureInfo? culture = null)
		{
			if (string.IsNullOrWhiteSpace(format))
				throw new ArgumentNullException(nameof(format), "Format string must be specified.");

			return dateTime.ToString(format, culture ?? CultureInfo.InvariantCulture);
		}
	}
}
