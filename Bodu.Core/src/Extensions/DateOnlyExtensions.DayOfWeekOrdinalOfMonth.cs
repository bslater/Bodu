// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.GetWeekdayOrdinalInMonth.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the day of week occurrence of the specified date and time for the month.
		/// </summary>
		/// <param name="dateTime">The <see cref="System.DateOnly" />.</param>
		/// <returns>
		/// A <see cref="Bodu.Extensions.MonthOrdinal" /> value that represents the occurrence of
		/// the <see cref="System.DateOnly.DayOfWeek" /> in the month.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The <see cref="Bodu.Extensions.DateOnlyExtensions.GetWeekdayOrdinalInMonth(DateOnly)" />
		/// returns the ordinal number of the <see cref="System.DateOnly.DayOfWeek" /> of
		/// <paramref name="dateTime" /> from the first day of the month.
		/// </para>
		/// </remarks>
		public static MonthOrdinal GetWeekdayOrdinalInMonth(this DateOnly date)
		{
			int ordinal = (int)((DateOnlyExtensions.DateTicks(dateTime) - DateOnlyExtensions.FirstDayOfTheMonthTicks(dateTime)) / DateOnlyExtensions.TicksPerWeek) + 1;
			return (Extensions.MonthOrdinal)ordinal;
		}
	}
}