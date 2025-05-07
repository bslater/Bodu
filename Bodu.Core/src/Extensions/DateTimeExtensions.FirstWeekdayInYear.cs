// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.FirstWeekdayInYear.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the first occurrence of the specified <see cref="DayOfWeek" /> in the calendar year of the given <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose year is used as the search range.</param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> to locate. For example, <see cref="DayOfWeek.Monday" /> will return the first Monday of the year.
		/// </param>
		/// <returns>
		/// A new <see cref="DateTime" /> set to the first occurrence of the specified weekday in the same year as
		/// <paramref name="dateTime" />, with the time component set to 00:00:00 and the <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a defined value of the <see cref="DayOfWeek" /> enumeration.
		/// </exception>
		/// <remarks>
		/// The returned <see cref="DateTime" /> is always within the same calendar year as <paramref name="dateTime" />, starting from
		/// January 1. The time component is normalized to 00:00:00, and the <see cref="DateTime.Kind" /> is retained from the original <paramref name="dateTime" />.
		/// </remarks>
		public static DateTime FirstWeekdayInYear(this DateTime dateTime, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			long ticks = DateTimeExtensions.GetTicksForDate(dateTime.Year, 1, 1);
			ticks += DateTimeExtensions.GetNextDayOfWeekTicksFrom(ticks, dayOfWeek);
			return new DateTime(ticks, dateTime.Kind);
		}
	}
}