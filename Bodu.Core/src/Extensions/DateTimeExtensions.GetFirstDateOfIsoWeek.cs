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
		/// Returns the first day (Monday) of the specified ISO 8601 week and year.
		/// </summary>
		/// <param name="isoYear">
		/// The ISO 8601 year, defined as the year containing the Thursday of the first ISO week. This may differ from the calendar year for
		/// dates near the start or end of a year.
		/// </param>
		/// <param name="isoWeek">
		/// The ISO 8601 week number, ranging from 1 to 53 depending on the year. Week 1 is the first week with a Thursday in the given ISO year.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on the Monday that begins the specified ISO 8601 week, with <see cref="DateTimeKind.Unspecified" />.
		/// </returns>
		/// <remarks>
		/// <para>This method calculates the start of an ISO week based on the ISO 8601 standard:</para>
		/// <list type="bullet">
		/// <item>
		/// <description>Weeks start on Monday.</description>
		/// </item>
		/// <item>
		/// <description>Week 1 is the first week with a Thursday in the year.</description>
		/// </item>
		/// <item>
		/// <description>Years may contain either 52 or 53 weeks.</description>
		/// </item>
		/// </list>
		/// <para>
		/// For .NET Standard 2.0, the method is computed manually by anchoring on January 4 (which always falls in ISO week 1) and
		/// backtracking to the preceding Monday.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="isoYear" /> is outside the valid range supported by <see cref="DateTime" />, or if
		/// <paramref name="isoWeek" /> is less than 1 or exceeds the number of ISO weeks in the given year.
		/// </exception>
		public static DateTime GetFirstDateOfIsoWeek(int isoYear, int isoWeek)
		{
			ThrowHelper.ThrowIfOutOfRange(isoYear, DateTime.MinValue.Year, DateTime.MaxValue.Year);
			ThrowHelper.ThrowIfOutOfRange(isoWeek, 1, GetIsoWeeksInYear(isoYear));

			long ticks = GetTicksForDate(isoYear, 1, 4);
			ticks += (
				1 - (((int)GetDayOfWeekFromTicks(ticks) + 6) % 7 + 1) +  // Backtrack to Monday
				(isoWeek - 1) * 7                                        // Advance to target week
			) * TicksPerDay;

			return new(ticks, DateTimeKind.Unspecified);
		}
	}
}