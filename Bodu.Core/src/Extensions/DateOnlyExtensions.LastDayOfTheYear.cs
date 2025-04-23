// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.LastDayOfTheYear.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the last day of the year for the specified <see cref="DateOnly" />.
		/// </summary>
		/// <param name="date">The input <see cref="DateOnly" />.</param>
		/// <returns>A new <see cref="DateOnly" /> set to December 31 of the same year.</returns>
		public static DateOnly LastDayOfTheYear(this DateOnly date)
			=> new DateOnly(DateOnlyExtensions.DateToTicks(dateTime.Year, 12, 31), dateTime.Kind);
	}
}