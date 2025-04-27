// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.NextDayOfWeek.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the next calendar occurrence of the specified <see cref="DayOfWeek" /> after
		/// the given <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The starting <see cref="DateTime" /> from which to search forward.</param>
		/// <param name="dayOfWeek">
		/// The desired <see cref="DayOfWeek" /> to locate. For example, <see cref="DayOfWeek.Monday" /> will return the next Monday.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> with the same time-of-day and <see cref="DateTime.Kind" /> as <paramref name="dateTime" />,
		/// representing the next occurrence of <paramref name="dayOfWeek" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a valid <see cref="DayOfWeek" /> enum value.
		/// </exception>
		/// <remarks>
		/// <para>
		/// If <paramref name="dateTime" /> already falls on the specified <paramref name="dayOfWeek" />, the result will be exactly 7 days
		/// later (the next calendar occurrence).
		/// </para>
		/// <para>The returned value preserves the original time-of-day and <see cref="DateTime.Kind" /> values of <paramref name="dateTime" />.</para>
		/// </remarks>
		public static DateTime NextDayOfWeek(this DateTime dateTime, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			return dateTime.AddTicks(dateTime.DayOfWeek == dayOfWeek ? DateTimeExtensions.TicksPerWeek : DateTimeExtensions.GetNextDayOfWeekAsTicks(dateTime, dayOfWeek));
		}
	}
}
