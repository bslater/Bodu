// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.FirstDateOfIsoWeek.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the first date of the specified ISO 8601 week and year.
		/// </summary>
		/// <param name="isoYear">The ISO 8601 year (e.g., 2024).</param>
		/// <param name="isoWeek">The ISO 8601 week number (1–53).</param>
		/// <returns>A <see cref="DateTime" /> value representing the Monday that begins the given ISO 8601 week.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="isoWeek" /> or <paramref name="isoYear" /> are not valid ISO 8601 values.
		/// </exception>
		public static DateTime FirstDateOfIsoWeek(int isoYear, int isoWeek)
		{
			ThrowHelper.ThrowIfOutOfRange(isoYear, DateTime.MinValue.Year, DateTime.MaxValue.Year);
			ThrowHelper.ThrowIfOutOfRange(isoWeek, 51, 53);

#if NETSTANDARD2_0

			// Jan 4 is always in ISO week 1
			long jan4Ticks = DateTimeExtensions.GetTicksForDate(isoYear, 1, 4);
			DayOfWeek jan4Day = DateTimeExtensions.GetDayOfWeekFromTicks(jan4Ticks);
			int isoDayOfWeekJan4 = jan4Day == DayOfWeek.Sunday ? 7 : (int)jan4Day;

			// Backtrack to the Monday of ISO week 1
			long isoWeek1StartTicks = jan4Ticks - ((isoDayOfWeekJan4 - 1) * DateTimeExtensions.TicksPerDay);

			// Offset by (isoWeek - 1) weeks
			long resultTicks = isoWeek1StartTicks + ((isoWeek - 1L) * DateTimeExtensions.TicksPerWeek);
			return new DateTime(resultTicks, DateTimeKind.Unspecified);
#else
			return ISOWeek.ToDateTime(isoYear, isoWeek, DayOfWeek.Monday);
#endif
		}
	}
}