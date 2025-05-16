// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="FirstDateOfIsoWeek.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the last date (Sunday) of the specified ISO 8601 week and year.
		/// </summary>
		/// <param name="isoYear">
		/// The ISO 8601 year, which corresponds to the calendar year containing the Thursday of the first ISO week (e.g., 2024).
		/// </param>
		/// <param name="isoWeek">
		/// The ISO 8601 week number within the specified year. Valid values range from 1 to 53, depending on the year.
		/// </param>
		/// <returns>A <see cref="DateTime" /> value representing the Sunday that ends the specified ISO 8601 week, with <see cref="DateTimeKind.Unspecified" />.</returns>
		/// <remarks>
		/// <para>In the ISO 8601 standard:
		/// <list type="bullet">
		/// <item>
		/// <description>Weeks begin on Monday and end on Sunday.</description>
		/// </item>
		/// <item>
		/// <description>Week 1 is the first week that contains at least four days in the new year.</description>
		/// </item>
		/// <item>
		/// <description>January 4th is always part of ISO week 1.</description>
		/// </item>
		/// </list>
		/// </para>
		/// <para>
		/// This method calculates the date by anchoring on January 4th of the given year, backtracking to the Monday of ISO week 1, and
		/// offsetting by the number of weeks to reach the specified ISO week. Six additional days are added to yield the Sunday of that week.
		/// </para>
		/// <note type="tip">For the corresponding start of the ISO week, use <see cref="GetFirstDateOfIsoWeek" />.</note>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="isoYear" /> is outside the valid <see cref="DateTime" /> range, or if <paramref name="isoWeek" /> is
		/// less than 1 or greater than the number of ISO weeks in that year.
		/// </exception>
		public static DateTime LastDateOfIsoWeek(int isoYear, int isoWeek)
		{
			ThrowHelper.ThrowIfOutOfRange(isoYear, DateTime.MinValue.Year, DateTime.MaxValue.Year);
			ThrowHelper.ThrowIfOutOfRange(isoWeek, 1, GetIsoWeeksInYear(isoYear));

			long ticks = GetTicksForDate(isoYear, 1, 4);
			ticks += (
				1 - (((int)GetDayOfWeekFromTicks(ticks) + 6) % 7 + 1) +  // Backtrack to Monday
				(isoWeek - 1) * 7 +                                      // Advance to target week
				6                                                        // Advance to Sunday
			) * TicksPerDay;

			return new(ticks, DateTimeKind.Unspecified);
		}
	}
}