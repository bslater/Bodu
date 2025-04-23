// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.FirstDayOfTheYear.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the first day of the year for the specified <see cref="DateOnly" />.
		/// </summary>
		/// <param name="date">The input <see cref="DateOnly" />.</param>
		/// <returns>A new <see cref="DateOnly" /> at the start of the year (January 1).</returns>
		public static DateOnly FirstDayOfTheYear(this DateOnly date)
			=> new DateOnly(DateOnlyExtensions.DateToTicks(dateTime.Year, 1, 1), dateTime.Kind);
	}
}