// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.NthDayOfWeekInMonth.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> that represents the Nth occurrence of the specified day of the week in the month.
		/// </summary>
		/// <param name="dateTime">The date providing the month and year context (day component is ignored).</param>
		/// <param name="dayOfWeek">The day of the week to locate.</param>
		/// <param name="ordinal">
		/// The ordinal occurrence to locate (e.g., <see cref="WeekOfMonthOrdinal.First" />, <see cref="WeekOfMonthOrdinal.Second" />, <see cref="WeekOfMonthOrdinal.Last" />).
		/// <para>
		/// <b>Note:</b><see cref="WeekOfMonthOrdinal.Fifth" /> is relatively rare and only occurs in months where five instances of the
		/// specified weekday exist.
		/// </para>
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the specified occurrence of the weekday within the same month and year as <paramref name="dateTime" />.
		/// </returns>
		/// <remarks>
		/// For <see cref="WeekOfMonthOrdinal.Last" />, the last matching weekday in the month is returned. For all others, the method
		/// computes the Nth occurrence starting from the first of the month.
		/// </remarks>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> or <paramref name="ordinal" /> is not a defined enum value, or if the specified ordinal
		/// does not exist in the given month.
		/// </exception>
		public static DateTime NthDayOfWeekInMonth(this DateTime dateTime, DayOfWeek dayOfWeek, WeekOfMonthOrdinal ordinal)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);
			ThrowHelper.ThrowIfEnumValueIsUndefined(ordinal);

			switch (ordinal)
			{
				case Extensions.WeekOfMonthOrdinal.First:
					return new DateTime(DateTimeExtensions.GetFirstDayOfWeekInMonthTicks(dateTime, dayOfWeek), dateTime.Kind);

				case Extensions.WeekOfMonthOrdinal.Last:
					return dateTime.LastWeekdayInMonth(dayOfWeek);

				default:
					var result = new DateTime(DateTimeExtensions.GetFirstDayOfWeekInMonthTicks(dateTime, dayOfWeek) + ((int)ordinal * DateTimeExtensions.TicksPerWeek), dateTime.Kind);

					if (result.Month != dateTime.Month)
						throw new ArgumentOutOfRangeException(nameof(ordinal),
							string.Format(ResourceStrings.Arg_Invalid_OrdinalDoesNotExistForMonth, ordinal, dayOfWeek, dateTime.ToString("MMMM yyyy")));

					return result;
			}
		}

		/// <summary>
		/// Calculates the date of the Nth occurrence of a specified day of the week within a given month and year.
		/// </summary>
		/// <param name="year">
		/// The year for which to calculate the date. Must be between <see cref="DateTimeExtensions.MinYear" /> and <see cref="DateTimeExtensions.MaxYear" />.
		/// </param>
		/// <param name="month">The month (1–12) for which to calculate the date.</param>
		/// <param name="dayOfWeek">The <see cref="DayOfWeek" /> to find within the specified month.</param>
		/// <param name="ordinal">
		/// The ordinal occurrence of the specified day of the week to return (e.g. First, Second, Third, Fourth, Last). If the specified
		/// ordinal does not occur in the given month (e.g., a fifth Monday in February), an <see cref="ArgumentOutOfRangeException" /> is thrown.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the Nth occurrence of the specified <paramref name="dayOfWeek" /> in the given
		/// <paramref name="month" /> and <paramref name="year" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the specified <paramref name="ordinal" /> does not correspond to a valid date in the given month.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the <paramref name="dayOfWeek" /> or <paramref name="ordinal" /> is not a defined enumeration value.
		/// </exception>
		/// <remarks>
		/// For <see cref="WeekOfMonthOrdinal.Last" />, the last matching day of the week in the month is returned. For other ordinal
		/// values, the method starts from the first matching day and adds full weeks to locate the Nth occurrence.
		/// </remarks>
		internal static DateTime NthDayOfWeekInMonth(int year, int month, DayOfWeek dayOfWeek, WeekOfMonthOrdinal ordinal)
		{
			ThrowHelper.ThrowIfOutOfRange(year, DateTimeExtensions.MinYear, DateTimeExtensions.MaxYear);
			ThrowHelper.ThrowIfOutOfRange(month, 1, 12);
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);
			ThrowHelper.ThrowIfEnumValueIsUndefined(ordinal);

			switch (ordinal)
			{
				case Extensions.WeekOfMonthOrdinal.First:
					return new DateTime(DateTimeExtensions.GetFirstDayOfWeekInMonthTicks(year, month, dayOfWeek), DateTimeKind.Unspecified);

				case Extensions.WeekOfMonthOrdinal.Last:
					return new DateTime(DateTimeExtensions.GetLastDayOfWeekInMonthAsTicks(year, month, dayOfWeek), DateTimeKind.Unspecified);

				default:
					var result = new DateTime(DateTimeExtensions.GetFirstDayOfWeekInMonthTicks(year, month, dayOfWeek) + ((int)ordinal * DateTimeExtensions.TicksPerWeek), DateTimeKind.Unspecified);

					if (result.Month != month)
						throw new ArgumentOutOfRangeException(nameof(ordinal),
							string.Format(ResourceStrings.Arg_Invalid_OrdinalDoesNotExistForMonth, ordinal, dayOfWeek, new DateTime(year, month, 1).ToString("MMMM yyyy")));

					return result;
			}
		}
	}
}
