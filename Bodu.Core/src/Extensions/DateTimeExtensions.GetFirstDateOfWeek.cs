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
		/// <param name="week">The 1-based week number (e.g., 1–53).</param>
		/// <param name="culture">
		/// Optional <see cref="CultureInfo" /> used to determine the calendar week rule and start of the week. If <c>null</c>, the current
		/// thread's culture is used.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> value representing the first day of the specified week, with the time component set to 00:00:00.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the <paramref name="week" /> does not correspond to a valid week number for the specified year and culture.
		/// </exception>
		public static DateTime GetFirstDateOfWeek(int year, int week, CultureInfo? culture = null)
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

		/// <summary>
		/// Returns the first day of the specified week in a given year, using the <see cref="CalendarWeekendDefinition" /> definition to
		/// infer the start of the week.
		/// </summary>
		/// <param name="year">The target year (e.g., 2024).</param>
		/// <param name="week">The 1-based week number (e.g., 1–53).</param>
		/// <param name="weekend">The <see cref="CalendarWeekendDefinition" /> that determines how the week is structured.</param>
		/// <returns>
		/// A <see cref="DateTime" /> value representing the first day of the specified week, with the time component set to 00:00:00.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekend" /> is not a defined <see cref="CalendarWeekendDefinition" /> value.
		/// </exception>
		/// <remarks>
		/// This method does not validate ISO 8601 compliance or locale-specific week rules. It is intended for systems using custom week logic.
		/// </remarks>
		public static DateTime GetFirstDateOfWeek(int year, int week, CalendarWeekendDefinition weekend)
		{
			DayOfWeek firstDay = GetWeekStartDay(weekend);
			Calendar calendar = CultureInfo.InvariantCulture.Calendar;

			DateTime firstDayOfYear = new(year, 1, 1);
			int offset = firstDay - firstDayOfYear.DayOfWeek;
			if (offset > 0) offset -= 7;

			DateTime firstWeekStart = firstDayOfYear.AddDays(offset);
			return firstWeekStart.AddDays((week - 1) * 7);
		}

		/// <summary>
		/// Returns the day of the week considered the start of the week for a given <see cref="CalendarWeekendDefinition" /> definition.
		/// </summary>
		/// <param name="weekend">The weekend configuration to evaluate.</param>
		/// <returns>The inferred <see cref="DayOfWeek" /> that begins the week.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the provided <paramref name="weekend" /> is not supported.</exception>
		private static DayOfWeek GetWeekStartDay(CalendarWeekendDefinition weekend) => weekend switch
		{
			CalendarWeekendDefinition.SaturdaySunday => DayOfWeek.Monday,     // ISO-8601
			CalendarWeekendDefinition.FridaySaturday => DayOfWeek.Sunday,     // Workweek = Sun–Thu
			CalendarWeekendDefinition.SundayOnly => DayOfWeek.Monday,
			CalendarWeekendDefinition.FridayOnly => DayOfWeek.Saturday,
			_ => throw new ArgumentOutOfRangeException(nameof(weekend),
				$"Unsupported {nameof(CalendarWeekendDefinition)} selectedDays: {weekend}")
		};
	}
}