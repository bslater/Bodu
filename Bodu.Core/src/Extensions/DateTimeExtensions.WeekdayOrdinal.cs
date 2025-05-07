// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.WeekdayOrdinal.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the ordinal occurrence of the weekday (e.g., first, second, third) for the specified <see cref="DateTime" /> within its month.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <returns>
		/// A <see cref="WeekOfMonthOrdinal" /> value representing which occurrence of the <see cref="DateTime.DayOfWeek" /> the given date
		/// is, within the same calendar month (e.g., the 2nd Tuesday).
		/// </returns>
		/// <remarks>
		/// The method calculates how many full weeks have passed since the first day of the month to determine the ordinal position of the
		/// weekday for the given <paramref name="dateTime" />.
		/// <para>
		/// <b>Note:</b> The <see cref="WeekOfMonthOrdinal.Fifth" /> value is uncommon and only applies to months that contain five
		/// occurrences of the specified <see cref="DayOfWeek" />.
		/// </para>
		/// </remarks>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if the computed ordinal value exceeds the defined values in <see cref="WeekOfMonthOrdinal" />.
		/// </exception>
		public static WeekOfMonthOrdinal WeekdayOrdinal(this DateTime dateTime)
		{
			int ordinal = (int)((DateTimeExtensions.GetDateTicks(dateTime) - DateTimeExtensions.GetFirstDayOfMonthTicks(dateTime)) / DateTimeExtensions.TicksPerWeek);

			if (!Enum.IsDefined(typeof(WeekOfMonthOrdinal), ordinal))
				throw new ArgumentOutOfRangeException(nameof(dateTime), $"The weekday occurrence at ordinal index {ordinal} is not defined in {nameof(WeekOfMonthOrdinal)}.");

			return (WeekOfMonthOrdinal)ordinal;
		}
	}
}