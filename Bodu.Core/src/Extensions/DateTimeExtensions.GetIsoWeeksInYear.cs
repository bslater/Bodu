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
		/// <returns>The number of weeks (52 or 53) in the specified year.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="year" /> is outside the valid range of the <c>Year</c> property, as defined by
		/// <see cref="DateTime.MinValue" /> and <see cref="DateTime.MaxValue" />.
		/// </exception>
		/// <remarks>
		/// <para>According to ISO 8601:
		/// <list type="bullet">
		/// <item>
		/// <description>A year has 53 weeks if January 1st falls on a Thursday, or if December 31st falls on a Thursday.</description>
		/// </item>
		/// <item>
		/// <description>All other years have 52 weeks.</description>
		/// </item>
		/// </list>
		/// </para>
		/// </remarks>
		public static int GetIsoWeeksInYear(int year)
		{
			ThrowHelper.ThrowIfOutOfRange(year, DateTime.MinValue.Year, DateTime.MaxValue.Year);

			return (GetWeekDayOfJanuary1(year) == DayOfWeek.Thursday || GetWeekDayOfJanuary1(year - 1) == DayOfWeek.Wednesday) ? 53 : 52;
		}
	}
}