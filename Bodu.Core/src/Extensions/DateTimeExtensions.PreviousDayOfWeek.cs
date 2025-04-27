// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.PreviousDayOfWeek.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the most recent occurrence of the specified <see cref="DayOfWeek" /> prior to
		/// the given <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The starting <see cref="DateTime" /> from which to search backward.</param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> to locate. For example, <see cref="DayOfWeek.Friday" /> returns the previous Friday before the
		/// given date.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> with the same time-of-day and <see cref="DateTime.Kind" /> as <paramref name="dateTime" />,
		/// representing the most recent occurrence of <paramref name="dayOfWeek" /> prior to the input.
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a defined value of the <see cref="DayOfWeek" /> enumeration.
		/// </exception>
		/// <remarks>
		/// <para>
		/// If <paramref name="dateTime" /> already falls on the specified <paramref name="dayOfWeek" />, the result will be exactly 7 days
		/// earlier (the previous calendar occurrence).
		/// </para>
		/// <para>The returned <see cref="DateTime" /> preserves the original time-of-day and <see cref="DateTime.Kind" /> values.</para>
		/// </remarks>
		public static DateTime PreviousDayOfWeek(this DateTime dateTime, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			return dateTime.AddTicks(dateTime.DayOfWeek == dayOfWeek ? -DateTimeExtensions.TicksPerWeek : DateTimeExtensions.GetPreviousDayOfWeekAsTicks(dateTime, dayOfWeek));
		}
	}
}
