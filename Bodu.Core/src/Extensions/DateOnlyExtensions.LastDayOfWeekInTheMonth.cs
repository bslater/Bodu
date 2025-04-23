// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.GetLastWeekdayInMonth.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the last occurrence of a specific <see cref="DayOfWeek" /> in the month of the
		/// specified <see cref="DateOnly" />.
		/// </summary>
		/// <param name="dateTime">The date providing the month and year.</param>
		/// <param name="dayOfWeek">The day of the week to find.</param>
		/// <returns>A <see cref="DateOnly" /> representing the last occurrence of the day.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a valid enum value.
		/// </exception>
		public static DateOnly GetLastWeekdayInMonth(this DateOnly date, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			long ticks = DateOnlyExtensions.LastDayOfWeekInTheMonthTicks(dateTime, dayOfWeek);
			return new DateOnly(ticks, dateTime.Kind);
		}
	}
}