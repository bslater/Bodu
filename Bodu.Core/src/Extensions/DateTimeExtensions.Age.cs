// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.Age.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Calculates the age in full years between the specified <see cref="DateTime" /> and today.
		/// </summary>
		/// <param name="dateTime">The date to calculate the age from, typically a date of birth or historical reference.</param>
		/// <returns>The number of full calendar years between <paramref name="dateTime" /> and <see cref="DateTime.Today" />.</returns>
		/// <remarks>
		/// If <paramref name="dateTime" /> is February 29 in a leap year and the current year is not a leap year, February 28 is used as
		/// the comparison point.
		/// </remarks>
		public static int Age(this DateTime dateTime)
			=> dateTime.Age(DateTime.Today);

		/// <summary>
		/// Calculates the age in full years between two <see cref="DateTime" /> values.
		/// </summary>
		/// <param name="firstDate">The earlier date to calculate from, typically a date of birth or historical reference.</param>
		/// <param name="secondDate">The later date to calculate to. This is usually the current date or another reference point.</param>
		/// <returns>The number of full calendar years that have elapsed between <paramref name="firstDate" /> and <paramref name="secondDate" />.</returns>
		/// <remarks>
		/// If <paramref name="firstDate" /> is February 29 in a leap year and <paramref name="secondDate" /> occurs in a non-leap year, the
		/// age is calculated based on February 28.
		/// </remarks>
		public static int Age(this DateTime firstDate, DateTime secondDate)
		{
			// if the first date is a leap day and the second date is not a leap year compute age based on 28-Feb
			if (firstDate.IsLeapYear() && firstDate.Month == 2 && firstDate.Day == 29 && !secondDate.IsLeapYear()) firstDate = firstDate.AddDays(-1);

			int age = secondDate.Year - firstDate.Year;

			// compensate for partial years
			if (age > 0 && secondDate.AddYears(-age) < firstDate) age--;
			if (age < 0 && secondDate.AddYears(-age) > firstDate) age++;
			return age;
		}
	}
}