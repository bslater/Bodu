// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="FirstDayOfWeekInYear.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the first occurrence of the specified <see cref="DayOfWeek" /> in the calendar year of the specified <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The reference <see cref="DateTime" /> whose <c>Year</c> property defines the search range.</param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> value to locate. For example, <see cref="DayOfWeek.Monday" /> returns the first Monday in the year.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> representing midnight (00:00:00) on the first occurrence of <paramref name="dayOfWeek" /> in the year
		/// of <paramref name="dateTime" />, with the original <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The search begins from January 1 of the given year and advances forward until the first occurrence of the specified
		/// <paramref name="dayOfWeek" /> is found.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a valid value in the <see cref="DayOfWeek" /> enumeration.
		/// </exception>
		public static DateTime FirstDayOfWeekInYear(this DateTime dateTime, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			long ticks = GetTicksForDate(dateTime.Year, 1, 1);
			ticks += GetNextDayOfWeekTicksFrom(ticks, dayOfWeek);
			return new(ticks, dateTime.Kind);
		}
	}
}