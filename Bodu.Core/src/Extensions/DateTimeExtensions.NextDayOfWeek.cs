// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="NextDayOfWeek.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the next calendar occurrence of the specified <see cref="DayOfWeek" /> after
		/// the given <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">
		/// The starting <see cref="DateTime" /> from which to search forward. The result will preserve the same time-of-day and <see cref="DateTime.Kind" />.
		/// </param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> to locate. For example, <see cref="DayOfWeek.Monday" /> returns the next Monday.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> with the same time-of-day and <see cref="DateTime.Kind" /> as <paramref name="dateTime" />,
		/// representing the next occurrence of <paramref name="dayOfWeek" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a defined value in the <see cref="DayOfWeek" /> enumeration.
		/// </exception>
		/// <remarks>
		/// <para>
		/// If <paramref name="dateTime" /> already falls on the specified <paramref name="dayOfWeek" />, the result is exactly 7 days later.
		/// </para>
		/// <para>The returned value always occurs after <paramref name="dateTime" />, maintaining the original time-of-day and <see cref="DateTime.Kind" />.</para>
		/// </remarks>
		public static DateTime NextDayOfWeek(this DateTime dateTime, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			return dateTime.AddTicks(
				dateTime.DayOfWeek == dayOfWeek
					? TicksPerWeek
					: GetNextDayOfWeekAsTicks(dateTime, dayOfWeek));
		}
	}
}