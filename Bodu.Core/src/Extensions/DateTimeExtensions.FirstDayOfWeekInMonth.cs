// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.FirstDayOfWeekInMonth.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the first occurrence of the specified <see cref="DayOfWeek" /> within the same month and year as the provided <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose month and year define the search range.</param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> to locate. For example, <see cref="DayOfWeek.Friday" /> will return the first Friday in the month.
		/// </param>
		/// <returns>
		/// A new <see cref="DateTime" /> representing the first occurrence of the specified <paramref name="dayOfWeek" /> in the month of
		/// <paramref name="dateTime" />. The result has its time set to 00:00:00 and preserves the <see cref="DateTime.Kind" /> of the input.
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a defined value of the <see cref="DayOfWeek" /> enumeration.
		/// </exception>
		/// <remarks>
		/// The search begins from the 1st day of the month, and the result will always fall within the same month and year as
		/// <paramref name="dateTime" />. The time component is reset to midnight (00:00:00), and the <see cref="DateTime.Kind" /> is preserved.
		/// </remarks>
		public static DateTime FirstDayOfWeekInMonth(this DateTime dateTime, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			long ticks = GetFirstDayOfWeekInMonthTicks(dateTime, dayOfWeek);
			return new DateTime(ticks, dateTime.Kind);
		}
	}
}