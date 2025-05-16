// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="GetIso8601WeekOfYear.cs" company="PlaceholderCompany">
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
		/// Returns the ISO 8601 week number for the specified <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value for which to calculate the ISO 8601 week number.</param>
		/// <returns>An integer from 1 to 53 representing the ISO 8601 week number of the year that contains <paramref name="dateTime" />.</returns>
		/// <remarks>
		/// <para>This method follows the ISO 8601 standard, in which:</para>
		/// <list type="bullet">
		/// <item>
		/// <description>Weeks begin on Monday.</description>
		/// </item>
		/// <item>
		/// <description>Week 1 is the first week that contains at least four days of the new year.</description>
		/// </item>
		/// </list>
		/// <para>
		/// To comply with this rule, dates falling on Monday through Wednesday are shifted forward by three days before calling
		/// <see cref="Calendar.GetWeekOfYear" /> with <see cref="CalendarWeekRule.FirstFourDayWeek" /> and <see cref="DayOfWeek.Monday" />.
		/// </para>
		/// <para>This method uses <see cref="CultureInfo.InvariantCulture" /> to ensure consistent week calculations across different locales.</para>
		/// </remarks>
		public static int GetIsoWeekOfYear(this DateTime dateTime) =>
			GetWeekOfYear(GetDateTicks(dateTime), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
	}
}