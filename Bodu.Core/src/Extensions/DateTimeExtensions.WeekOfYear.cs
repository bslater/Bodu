// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.WeekOfYear.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the week number (1–53) of the year that contains the specified <see cref="DateTime" />, using the current culture's
		/// calendar and week rule.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <returns>The culture-specific week number of the year that contains <paramref name="dateTime" />.</returns>
		/// <remarks>
		/// This method uses the <see cref="CalendarWeekRule" /> and <see cref="DayOfWeek" /> settings of
		/// <see cref="CultureInfo.CurrentCulture" />. Results may vary between cultures (e.g., the U.S. and ISO week numbers differ).
		/// </remarks>
		public static int WeekOfYear(this DateTime dateTime)
			=> dateTime.WeekOfYear(null!);

		/// <summary>
		/// Returns the week number (1–53) of the year that contains the specified <see cref="DateTime" />, using the given <paramref name="culture" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <param name="culture">
		/// The <see cref="CultureInfo" /> used to determine the calendar system, week rule, and first day of the week. If <c>null</c>,
		/// <see cref="CultureInfo.CurrentCulture" /> is used.
		/// </param>
		/// <returns>The week number of the year according to the specified culture's calendar and formatting rules.</returns>
		/// <remarks>
		/// This method uses <see cref="DateTimeFormatInfo.Calendar" />, <see cref="DateTimeFormatInfo.CalendarWeekRule" />, and
		/// <see cref="DateTimeFormatInfo.FirstDayOfWeek" /> from the specified culture to compute the result.
		/// </remarks>
		public static int WeekOfYear(this DateTime dateTime, CultureInfo? culture)
		{
			CultureInfo effectiveCulture = culture ?? Thread.CurrentThread.CurrentCulture;
			DateTimeFormatInfo info = effectiveCulture.DateTimeFormat;
			return info.Calendar.GetWeekOfYear(dateTime, info.CalendarWeekRule, info.FirstDayOfWeek);
		}
	}
}
