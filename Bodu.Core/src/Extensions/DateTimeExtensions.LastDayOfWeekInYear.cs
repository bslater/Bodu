// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="LastDayOfWeekInYear.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the last occurrence of the specified <see cref="DayOfWeek" /> within the same calendar year as the given <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">
		/// The <see cref="DateTime" /> whose calendar year is used as the search range. The result preserves the original <see cref="DateTime.Kind" />.
		/// </param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> value to locate. For example, <see cref="DayOfWeek.Sunday" /> returns the last Sunday in the year.
		/// </param>
		/// <returns>
		/// A new <see cref="DateTime" /> instance set to midnight (00:00:00) on the final occurrence of the specified
		/// <paramref name="dayOfWeek" /> in the same year as <paramref name="dateTime" />, with the original <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a defined value of the <see cref="DayOfWeek" /> enumeration.
		/// </exception>
		/// <remarks>
		/// <para>
		/// The search begins on December 31 of the specified year and moves backward until the desired <paramref name="dayOfWeek" /> is found.
		/// </para>
		/// <para>
		/// The returned <see cref="DateTime" /> is normalized to midnight and guaranteed to fall within the same calendar year as <paramref name="dateTime" />.
		/// </para>
		/// </remarks>
		public static DateTime LastDayOfWeekInYear(this DateTime dateTime, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			long ticks = GetTicksForDate(dateTime.Year, 12, 31);
			ticks += GetPreviousDayOfWeekTicksFrom(ticks, dayOfWeek);
			return new(ticks, dateTime.Kind);
		}
	}
}