// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="NthDayOfWeekInMonth.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the specified ordinal occurrence of a <see cref="DayOfWeek" /> within the
		/// same month and year as the given <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The reference <see cref="DateTime" />. Only its month and year are used; the day component is ignored.</param>
		/// <param name="dayOfWeek">The <see cref="DayOfWeek" /> value to locate within the month.</param>
		/// <param name="ordinal">
		/// The ordinal instance to retrieve (e.g., <see cref="WeekOfMonthOrdinal.First" />, <see cref="WeekOfMonthOrdinal.Second" />, <see cref="WeekOfMonthOrdinal.Last" />).
		/// <para>
		/// <b>Note:</b><see cref="WeekOfMonthOrdinal.Fifth" /> occurs only in months where five instances of the specified
		/// <paramref name="dayOfWeek" /> exist.
		/// </para>
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the requested occurrence of <paramref name="dayOfWeek" /> in the same month and year as
		/// <paramref name="dateTime" />. The time is set to midnight and the <see cref="DateTime.Kind" /> is preserved.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> or <paramref name="ordinal" /> is not a defined enumeration value, or if the ordinal
		/// does not occur in the target month.
		/// </exception>
		/// <remarks>
		/// For <see cref="WeekOfMonthOrdinal.Last" />, the method returns the final matching weekday of the month. For other values, the
		/// result is computed by offsetting from the first occurrence of <paramref name="dayOfWeek" />.
		/// </remarks>
		public static DateTime NthDayOfWeekInMonth(this DateTime dateTime, DayOfWeek dayOfWeek, WeekOfMonthOrdinal ordinal)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);
			ThrowHelper.ThrowIfEnumValueIsUndefined(ordinal);

			switch (ordinal)
			{
				case Extensions.WeekOfMonthOrdinal.First:
					return new(GetFirstDayOfWeekInMonthTicks(dateTime, dayOfWeek), dateTime.Kind);

				case Extensions.WeekOfMonthOrdinal.Last:
					return dateTime.LastDayOfWeekInMonth(dayOfWeek);

				default:
					var result = new DateTime(GetFirstDayOfWeekInMonthTicks(dateTime, dayOfWeek) + (((int)ordinal - 1) * TicksPerWeek), dateTime.Kind);

					if (result.Month != dateTime.Month)
						throw new ArgumentOutOfRangeException(nameof(ordinal),
							string.Format(ResourceStrings.Arg_Invalid_OrdinalDoesNotExistForMonth, ordinal, dayOfWeek, dateTime.ToString("MMMM yyyy")));

					return result;
			}
		}

		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the specified ordinal occurrence of a <see cref="DayOfWeek" /> within a given
		/// calendar month and year.
		/// </summary>
		/// <param name="year">
		/// The calendar year to evaluate. Must be between the <c>Year</c> property values of <see cref="DateTime.MinValue" /> and
		/// <see cref="DateTime.MaxValue" />, inclusive.
		/// </param>
		/// <param name="month">
		/// The calendar month to evaluate. Must be an integer between 1 and 12, inclusive, where 1 represents January and 12 represents December.
		/// </param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> value to locate within the specified month. For example, <see cref="DayOfWeek.Monday" /> will
		/// return the nth Monday of the month as determined by <paramref name="ordinal" />.
		/// </param>
		/// <param name="ordinal">
		/// The ordinal instance to retrieve within the month. Supported values include <see cref="WeekOfMonthOrdinal.First" />,
		/// <see cref="WeekOfMonthOrdinal.Second" />, <see cref="WeekOfMonthOrdinal.Third" />, <see cref="WeekOfMonthOrdinal.Fourth" />,
		/// <see cref="WeekOfMonthOrdinal.Fifth" />, and <see cref="WeekOfMonthOrdinal.Last" />.
		/// <para>
		/// <b>Note:</b><see cref="WeekOfMonthOrdinal.Fifth" /> is only valid in months where five occurrences of the specified day exist.
		/// </para>
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the requested occurrence of <paramref name="dayOfWeek" /> in the specified
		/// <paramref name="month" /> and <paramref name="year" />. The returned time is set to midnight (00:00:00) with <see cref="DateTimeKind.Unspecified" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if:
		/// <list type="bullet">
		/// <item><paramref name="year" /> is less than the <c>Year</c> of <see cref="DateTime.MinValue" /> or greater than that of <see cref="DateTime.MaxValue" />.</item>
		/// <item><paramref name="month" /> is not between 1 and 12.</item>
		/// <item><paramref name="dayOfWeek" /> is not a defined value of the <see cref="DayOfWeek" /> enumeration.</item>
		/// <item><paramref name="ordinal" /> is not a defined value of the <see cref="WeekOfMonthOrdinal" /> enumeration.</item>
		/// <item>
		/// <paramref name="ordinal" /> refers to an occurrence that does not exist in the specified month (e.g., the fifth Tuesday in February).
		/// </item>
		/// </list>
		/// </exception>
		/// <remarks>
		/// <para>
		/// For <see cref="WeekOfMonthOrdinal.Last" />, the method finds the final occurrence of the specified <paramref name="dayOfWeek" />
		/// in the month by counting backwards from the last day.
		/// </para>
		/// <para>
		/// For all other ordinal values, the method finds the first occurrence of <paramref name="dayOfWeek" /> in the month, then offsets
		/// by full 7-day weeks to compute the requested instance.
		/// </para>
		/// </remarks>
		public static DateTime GetNthDayOfWeekInMonth(int year, int month, DayOfWeek dayOfWeek, WeekOfMonthOrdinal ordinal)
		{
			ThrowHelper.ThrowIfOutOfRange(year, MinYear, MaxYear);
			ThrowHelper.ThrowIfOutOfRange(month, 1, 12);
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);
			ThrowHelper.ThrowIfEnumValueIsUndefined(ordinal);

			switch (ordinal)
			{
				case Extensions.WeekOfMonthOrdinal.First:
					return new(GetFirstDayOfWeekInMonthTicks(year, month, dayOfWeek), DateTimeKind.Unspecified);

				case Extensions.WeekOfMonthOrdinal.Last:
					return new(GetLastDayOfWeekInMonthAsTicks(year, month, dayOfWeek), DateTimeKind.Unspecified);

				default:
					var result = new DateTime(GetFirstDayOfWeekInMonthTicks(year, month, dayOfWeek) + (((int)ordinal - 1) * TicksPerWeek), DateTimeKind.Unspecified);

					if (result.Month != month)
						throw new ArgumentOutOfRangeException(nameof(ordinal),
							string.Format(ResourceStrings.Arg_Invalid_OrdinalDoesNotExistForMonth, ordinal, dayOfWeek, $"{GetMonthName(month)} {year:0000}"));

					return result;
			}
		}
	}
}