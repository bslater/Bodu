// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.LastDayOfTheMonth.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the last day of the month for the given <see cref="DateOnly" />.
		/// </summary>
		/// <param name="date">The input <see cref="DateOnly" />.</param>
		/// <returns>A new <see cref="DateOnly" /> at midnight on the last day of the month.</returns>
		/// <remarks>
		/// Preserves the original <see cref="DateOnly.Kind" /> and uses Gregorian calendar logic.
		/// </remarks>
		public static DateOnly LastDayOfTheMonth(this DateOnly date)
			=> new DateOnly(DateOnlyExtensions.LastDayOfTheMonthTicks(dateTime), dateTime.Kind);
	}
}