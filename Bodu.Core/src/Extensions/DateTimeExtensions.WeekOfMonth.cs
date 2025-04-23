// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.Add.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the 1-based week number of the month for the specified <see cref="DateTime" />, using the current culture's calendar
		/// week rule and first day of the week.
		/// </summary>
		/// <param name="date">The date to evaluate.</param>
		/// <returns>The 1-based week number of the month for the specified date.</returns>
		/// <remarks>
		/// This method uses the <see cref="CultureInfo.CurrentCulture" /> settings to determine the calendar week rule and the first day of
		/// the week. The calculation is based on the difference between the week number of the target date and the week number of the first
		/// day of the month, plus one.
		/// </remarks>
		public static int WeekOfMonth(this DateTime date)
		{
			var culture = CultureInfo.CurrentCulture;
			return date.WeekOfMonth(culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
		}

		/// <summary>
		/// Returns the 1-based week number of the month for the specified <see cref="DateTime" />, using the calendar week rule and first
		/// day of the week defined by the specified <see cref="CultureInfo" />.
		/// </summary>
		/// <param name="date">The date to evaluate.</param>
		/// <param name="culture">
		/// The culture providing the calendar rule and first day of the week. If <c>null</c>, the current thread culture is used.
		/// </param>
		/// <returns>The 1-based week number of the month for the specified date.</returns>
		/// <remarks>
		/// This method uses the specified culture's <see cref="CalendarWeekRule" /> and <see cref="DateTimeFormatInfo.FirstDayOfWeek" /> to
		/// compute the week number of the date relative to the first day of the same month.
		/// </remarks>
		public static int WeekOfMonth(this DateTime date, CultureInfo culture)
		{
			culture ??= Thread.CurrentThread.CurrentCulture;
			return date.WeekOfMonth(culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
		}

		/// <summary>
		/// Returns the 1-based week number of the month for the specified <see cref="DateTime" />, using the specified
		/// <see cref="CalendarWeekRule" /> and <see cref="DayOfWeek" />.
		/// </summary>
		/// <param name="date">The date to evaluate.</param>
		/// <param name="weekRule">The calendar week rule used to determine the first week of the month.</param>
		/// <param name="weekStart">The day considered the start of the week.</param>
		/// <returns>The 1-based week number of the month for the specified date.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekRule" /> or <paramref name="weekStart" /> is not a defined enum value.
		/// </exception>
		/// <remarks>
		/// The week number is calculated as the difference between the week of year for the given date and the week of year for the first
		/// day of the same month, plus one. This ensures that the result is always 1-based and reflects the position of the week within the
		/// calendar month.
		/// </remarks>
		public static int WeekOfMonth(this DateTime date, CalendarWeekRule weekRule, DayOfWeek weekStart)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekRule);
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekStart);

			var calendar = CultureInfo.InvariantCulture.Calendar; // Use invariant for consistent week computation
			var weekOfYear = calendar.GetWeekOfYear(date, weekRule, weekStart);
			var firstOfMonth = new DateTime(DateTimeExtensions.GetTicksForDate(date.Year, date.Month, 1), date.Kind);
			var firstWeek = calendar.GetWeekOfYear(firstOfMonth, weekRule, weekStart);

			return weekOfYear - firstWeek + 1;
		}
	}
}
