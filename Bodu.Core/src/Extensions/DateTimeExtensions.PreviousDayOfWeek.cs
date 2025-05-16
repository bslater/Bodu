// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="PreviousDayOfWeek.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the most recent occurrence of the specified <see cref="DayOfWeek" /> prior to
		/// the given <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">
		/// The starting <see cref="DateTime" /> from which to search backward. The time-of-day and <see cref="DateTime.Kind" /> are preserved.
		/// </param>
		/// <param name="dayOfWeek">
		/// The target <see cref="DayOfWeek" /> to locate. For example, <see cref="DayOfWeek.Friday" /> returns the previous Friday before <paramref name="dateTime" />.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> with the same time-of-day and <see cref="DateTime.Kind" /> as <paramref name="dateTime" />,
		/// representing the most recent occurrence of <paramref name="dayOfWeek" /> before the given date.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If <paramref name="dateTime" /> already falls on the specified <paramref name="dayOfWeek" />, the result will be exactly 7 days earlier.
		/// </para>
		/// <para>This method is useful for calculating recurring schedules or navigating week-based date boundaries.</para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a valid member of the <see cref="DayOfWeek" /> enumeration.
		/// </exception>
		public static DateTime PreviousDayOfWeek(this DateTime dateTime, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			return dateTime.AddTicks(
				dateTime.DayOfWeek == dayOfWeek
					? -TicksPerWeek
					: GetPreviousDayOfWeekAsTicks(dateTime, dayOfWeek));
		}
	}
}