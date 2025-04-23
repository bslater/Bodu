// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.GetNthDayOfWeekInMonth.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateOnly" /> that represents the Nth occurrence of the
		/// specified day of the week in the month.
		/// </summary>
		/// <param name="date">The date providing the month and year.</param>
		/// <param name="dayOfWeek">The day of the week to locate.</param>
		/// <param name="ordinal">The ordinal occurrence (e.g., First, Second, Last).</param>
		/// <returns>A <see cref="DateOnly" /> representing the specified occurrence.</returns>
		/// <remarks>
		/// Returns the first occurrence by default. For "Last", it returns the last matching day
		/// within the month.
		/// </remarks>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="ordinal" /> is not a defined enum value.
		/// </exception>
		public static DateOnly GetNthDayOfWeekInMonth(this DateOnly date, DayOfWeek dayOfWeek, MonthOrdinal ordinal)
		{
			switch (ordinal)
			{
				case MonthOrdinal.First:
					return new DateOnly(DateOnlyExtensions.FirstDayOfWeekInTheMonthTicks(date, dayOfWeek), date.Kind);

				case MonthOrdinal.Last:
					return date.GetLastWeekdayInMonth(dayOfWeek);

				default:
					return new DateOnly(DateOnlyExtensions.FirstDayOfWeekInTheMonthTicks(date, dayOfWeek) + (((int)ordinal - 1) * DateOnlyExtensions.TicksPerWeek), date.Kind);
			}
		}
	}
}