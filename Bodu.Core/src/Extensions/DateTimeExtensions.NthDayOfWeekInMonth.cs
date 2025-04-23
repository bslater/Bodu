// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.NthDayOfWeekInMonth.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

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
		/// <b>Note:</b><see cref="WeekOfMonthOrdinal.Fifth" /> is relatively rare and only occurs in months where five instances of the specified
		/// weekday exist.
		/// </para>
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the specified occurrence of the weekday within the same month and year as <paramref name="dateTime" />.
		/// </returns>
		/// <remarks>
		/// For <see cref="WeekOfMonthOrdinal.Last" />, the last matching weekday in the month is returned. For all others, the method computes
		/// the Nth occurrence starting from the first of the month.
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
	}
}
