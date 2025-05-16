// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="OrdinalWeekOfMonth.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the ordinal occurrence of the specified <see cref="DateTime.DayOfWeek" /> within its month.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <returns>
		/// A <see cref="WeekOfMonthOrdinal" /> value indicating which occurrence of the weekday <paramref name="dateTime.DayOfWeek" />
		/// represents within the same calendar month (e.g., the 2nd Monday).
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method calculates how many full 7-day intervals have passed since the first day of the month, based on the
		/// <paramref name="dateTime.Day" /> value. The result indicates whether the current weekday is the first, second, third, fourth, or
		/// fifth occurrence.
		/// </para>
		/// <para><b>Note:</b><see cref="WeekOfMonthOrdinal.Fifth" /> applies only when a month contains five instances of the given weekday.</para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the computed ordinal exceeds the defined values in <see cref="WeekOfMonthOrdinal" />. This typically indicates an
		/// invalid or unsupported weekday occurrence.
		/// </exception>
		public static WeekOfMonthOrdinal OrdinalWeekOfMonth(this DateTime dateTime)
		{
			int ordinal = ((dateTime.Day - 1) / 7) + 1;

			ThrowHelper.ThrowIfEnumValueIsUndefined<WeekOfMonthOrdinal>((WeekOfMonthOrdinal)ordinal);

			return (WeekOfMonthOrdinal)ordinal;
		}
	}
}