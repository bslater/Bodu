// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="Add.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the number of ISO 8601 weeks in the specified year (either 52 or 53).
		/// </summary>
		/// <param name="year">The ISO 8601 year to evaluate.</param>
		/// <returns>The number of ISO 8601 weeks in the specified year: either 52 or 53.</returns>
		/// <remarks>
		/// <para>According to ISO 8601:</para>
		/// <list type="bullet">
		/// <item>
		/// <description>A year has 53 weeks if January 1 falls on a Thursday,</description>
		/// </item>
		/// <item>
		/// <description>–or– if December 31 falls on a Thursday.</description>
		/// </item>
		/// <item>
		/// <description>All other years contain 52 weeks.</description>
		/// </item>
		/// </list>
		/// <para>
		/// The calculation is based on the weekday of January 1 and the preceding December 31 to determine whether the year contains an
		/// extra ISO week.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="year" /> is less than 1 or greater than 9999.</exception>
		public static int GetIsoWeeksInYear(int year)
		{
			ThrowHelper.ThrowIfOutOfRange(year, DateTime.MinValue.Year, DateTime.MaxValue.Year);

			return (GetWeekDayOfJanuary1(year) == DayOfWeek.Thursday || GetWeekDayOfJanuary1(year - 1) == DayOfWeek.Wednesday) ? 53 : 52;
		}
	}
}