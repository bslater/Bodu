// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.Age.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Calculates the age in years from the specified <see cref="DateOnly" /> to today.
		/// </summary>
		/// <param name="date">The date of birth or past date.</param>
		/// <returns>The number of whole years elapsed between the input and today.</returns>
		/// <remarks>
		/// If the date is February 29 in a leap year, the age is calculated based on February 28 in
		/// non-leap years.
		/// </remarks>
		public static int Age(this DateOnly date)
			=> date.Age(DateTime.Today.ToDateOnly());

		/// <summary>
		/// Calculates the age in years from one <see cref="DateOnly" /> to another.
		/// </summary>
		/// <param name="firstDate">The earlier date.</param>
		/// <param name="secondDate">The later date to compare against.</param>
		/// <returns>The number of full years between the two dates.</returns>
		/// <remarks>
		/// If <paramref name="firstDate" /> is February 29 in a leap year and
		/// <paramref name="secondDate" /> is not in a leap year, February 28 is used as the
		/// comparison base.
		/// </remarks>
		public static int Age(this DateOnly firstDate, DateOnly secondDate)
		{
			// if the first date is a leap day and the second date is not a leap year, compute age
			// based on 28-Feb
			if (firstDate.IsLeapYear() && firstDate.Month == 2 && firstDate.Day == 29 && !secondDate.IsLeapYear())
				firstDate = firstDate.AddDays(-1);

			int age = secondDate.Year - firstDate.Year;

			// compensate for partial years
			if (age > 0 && secondDate.AddYears(-age) < firstDate) age--;
			if (age < 0 && secondDate.AddYears(-age) > firstDate) age++;
			return age;
		}
	}
}