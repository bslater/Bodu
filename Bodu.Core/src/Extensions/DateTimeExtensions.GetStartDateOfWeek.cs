// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.GetFirstDateOfWeek.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the <see cref="DateTime" /> representing the first day of the specified week number in a given year, using the
		/// <see cref="CalendarWeekRule" /> and <see cref="DayOfWeek" /> settings from the specified <see cref="CultureInfo" />.
		/// </summary>
		/// <param name="year">The target year (e.g., 2024).</param>
		/// <param name="week">The 1-based week number (1–53).</param>
		/// <param name="culture">
		/// An optional <see cref="CultureInfo" /> used to determine the start of the week and the applicable week rule. If <c>null</c>,
		/// <see cref="CultureInfo.CurrentCulture" /> is used.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00.000) on the first day of the specified week, with
		/// <see cref="DateTimeKind.Unspecified" /> as its kind.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="week" /> does not correspond to a valid week number for <paramref name="year" /> using the week rule
		/// and starting day of the specified or current culture.
		/// </exception>
		/// <remarks>
		/// This method aligns with the culture-specific week numbering scheme as defined by <paramref name="culture" />. The first day of
		/// the year is adjusted backward to the nearest instance of the defined start-of-week day, and weeks are counted forward from that
		/// anchor. The result is validated using <see cref="Calendar.GetWeekOfYear" />.
		/// </remarks>
		public static DateTime GetStartDateOfWeek(int year, int week, CultureInfo? culture = null)
		{
			culture ??= CultureInfo.CurrentCulture;
			DateTimeFormatInfo dfi = culture.DateTimeFormat;
			Calendar calendar = dfi.Calendar;

			DateTime firstDayOfYear = new(year, 1, 1);
			int daysOffset = dfi.FirstDayOfWeek - firstDayOfYear.DayOfWeek;
			if (daysOffset > 0) daysOffset -= 7;

			DateTime firstWeekStart = firstDayOfYear.AddDays(daysOffset);
			DateTime result = firstWeekStart.AddDays((week - 1) * 7);

			// Validate that result belongs to the correct year and week number
			int resultWeek = calendar.GetWeekOfYear(result, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
			if (resultWeek != week)
				throw new ArgumentOutOfRangeException(nameof(week),
					$"Week {week} is invalid for the year {year} using culture {culture.Name}.");

			return result.Date;
		}
	}
}