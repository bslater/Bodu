// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="FirstDayOfWeekInMonth.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the first occurrence of the specified <see cref="DayOfWeek" /> within the same month and year as the given <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The reference <see cref="DateTime" /> whose month and year are used to determine the search range.</param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> to locate. For example, <see cref="DayOfWeek.Monday" /> returns the first Monday in the month.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> representing midnight (00:00:00) on the first occurrence of <paramref name="dayOfWeek" /> in the month
		/// of <paramref name="dateTime" />, with the original <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The search begins on the 1st day of the month and continues forward to locate the first occurrence of the specified weekday. The
		/// result is guaranteed to fall within the same month and year as <paramref name="dateTime" />.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a valid value in the <see cref="DayOfWeek" /> enumeration.
		/// </exception>
		public static DateTime FirstDayOfWeekInMonth(this DateTime dateTime, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			return new(GetFirstDayOfWeekInMonthTicks(dateTime, dayOfWeek), dateTime.Kind);
		}
	}
}