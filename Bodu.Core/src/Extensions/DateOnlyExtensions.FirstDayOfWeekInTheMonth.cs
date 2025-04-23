// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.GetFirstWeekdayInMonth.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the first occurrence of the specified <see cref="DayOfWeek" /> in the month of
		/// the given <see cref="DateOnly" />.
		/// </summary>
		/// <param name="date">The input <see cref="DateOnly" />.</param>
		/// <param name="dayOfWeek">The day of the week to find.</param>
		/// <returns>
		/// A new <see cref="DateOnly" /> representing the first occurrence of the specified day in
		/// the month.
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a valid enum value.
		/// </exception>
		public static DateOnly GetFirstWeekdayInMonth(this DateOnly date, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			long ticks = FirstDayOfWeekInTheMonthTicks(dateTime, dayOfWeek);
			return new DateOnly(ticks, dateTime.Kind);
		}
	}
}