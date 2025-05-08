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

		/// <summary>
		/// Returns the start date of the specified week in the given year, based on the inferred week structure from the provided <see cref="CalendarWeekendDefinition" />.
		/// </summary>
		/// <param name="year">The target year (e.g., 2024).</param>
		/// <param name="week">The 1-based week number (1–53).</param>
		/// <param name="weekend">A <see cref="CalendarWeekendDefinition" /> value used to infer the first day of the week.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00.000) on the first day of the specified week, with
		/// <see cref="DateTimeKind.Unspecified" /> as its kind.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="week" /> is less than 1, or if <paramref name="weekend" /> is not a supported value.
		/// </exception>
		/// <remarks>
		/// This method does not follow ISO 8601 or locale-based week rules. It assumes a simple week numbering model, where weeks are
		/// counted from the first occurrence of the inferred start-of-week day in the target year. The result is constructed using
		/// ticks-based arithmetic and does not allocate intermediate <see cref="DateTime" /> objects.
		/// </remarks>
		public static DateTime GetStartDateOfWeek(int year, int week, CalendarWeekendDefinition weekend)
		{
			ThrowHelper.ThrowIfLessThan(week, 1, nameof(week));
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekend);

			DayOfWeek firstDayOfWeek = GetWeekStartDay(weekend);

			// Get ticks at midnight on Jan 1st of the year
			long jan1Ticks = DateTimeExtensions.GetTicksForDate(year, 1, 1);
			DayOfWeek jan1DayOfWeek = DateTimeExtensions.GetDayOfWeekFromTicks(jan1Ticks);

			// Calculate tick offset to the first occurrence of the target week start
			int offsetDays = ((int)firstDayOfWeek - (int)jan1DayOfWeek + 7) % 7;
			if (offsetDays > 0)
				offsetDays -= 7;

			long targetTicks = jan1Ticks + ((offsetDays + ((week - 1) * 7)) * DateTimeExtensions.TicksPerDay);

			if (targetTicks < DateTimeExtensions.MinTicks || targetTicks > DateTimeExtensions.MaxTicks)
				throw new ArgumentOutOfRangeException(nameof(week),
					string.Format(ResourceStrings.Arg_OutOfRange_ResultingValueOutOfRangeForType, nameof(DateTime)));

			return new DateTime(targetTicks, DateTimeKind.Unspecified);
		}
	}
}