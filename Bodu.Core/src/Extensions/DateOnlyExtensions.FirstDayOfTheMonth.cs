// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.FirstDayOfTheMonth.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the first day of the month for the given <see cref="DateOnly" />.
		/// </summary>
		/// <param name="date">The input <see cref="DateOnly" />.</param>
		/// <returns>A new <see cref="DateOnly" /> set to the first day of the month.</returns>
		public static DateOnly FirstDayOfTheMonth(this DateOnly date)
			=> new DateOnly(DateOnlyExtensions.FirstDayOfTheMonthTicks(dateTime), dateTime.Kind);
	}
}