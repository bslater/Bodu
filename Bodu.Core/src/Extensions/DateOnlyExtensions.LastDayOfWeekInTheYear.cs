// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.GetLastWeekdayInYear.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the last occurrence of a specific <see cref="DayOfWeek" /> in the year of the
		/// specified <see cref="DateOnly" />.
		/// </summary>
		/// <param name="dateTime">The date providing the year.</param>
		/// <param name="dayOfWeek">The day of the week to find.</param>
		/// <returns>A <see cref="DateOnly" /> representing the last occurrence of the day.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a valid enum value.
		/// </exception>
		public static DateOnly GetLastWeekdayInYear(this DateOnly date, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			long ticks = DateOnlyExtensions.DateToTicks(dateTime.Year, 12, 31);
			ticks += DateOnlyExtensions.PreviousDayOfWeekTicks(ticks, dayOfWeek);
			return new DateOnly(ticks, dateTime.Kind);
		}
	}
}