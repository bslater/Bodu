// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.GetIso8601WeekOfYear.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Gets the ISO 8601 week number of the year.
		/// </summary>
		public static int GetIso8601WeekOfYear(this DateTime dateTime)
		{
			// ISO 8601 weeks start on Monday, and the first week of the year has at least 4 days.
			DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dateTime);
			if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
				dateTime = dateTime.AddDays(3);
			return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
				dateTime,
				CalendarWeekRule.FirstFourDayWeek,
				DayOfWeek.Monday);
		}
	}
}