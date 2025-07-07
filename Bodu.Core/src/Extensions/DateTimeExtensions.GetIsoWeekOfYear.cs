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
		/// <param name="dateTime">The <see cref="DateTime" /> value to evaluate.</param>
		/// <returns>The ISO 8601 week number of the year that contains <paramref name="dateTime" />, ranging from 1 to 53.</returns>
		/// <remarks>
		/// <para>This method uses the ISO 8601 standard for week numbering, where:</para>
		/// <list type="bullet">
		/// <item>
		/// <description>Weeks begin on Monday.</description>
		/// </item>
		/// <item>
		/// <description>Week 1 is the first week that contains at least four days in the new year.</description>
		/// </item>
		/// </list>
		/// <para>
		/// To comply with this rule, dates falling on Monday through Wednesday are internally adjusted forward by three days before
		/// computing the week number using <see cref="Calendar.GetWeekOfYear" /> with <see cref="CalendarWeekRule.FirstFourDayWeek" /> and <see cref="DayOfWeek.Monday" />.
		/// </para>
		/// <para><see cref="CultureInfo.InvariantCulture" /> is used to ensure consistent behavior across cultures.</para>
		/// </remarks>
		public static int GetIsoWeekOfYear(this DateTime dateTime) =>
			GetWeekOfYear(GetDateTicks(dateTime), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
	}
}