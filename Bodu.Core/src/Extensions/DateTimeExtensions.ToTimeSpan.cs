// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.ToTimeSpan.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="TimeSpan" /> representing the time-of-day component of the specified <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose time portion will be extracted.</param>
		/// <returns>A <see cref="TimeSpan" /> containing the hour, minute, second, and fractional second values of <paramref name="dateTime" />.</returns>
		/// <remarks>
		/// This method returns the time elapsed since midnight for the given date. The result is unaffected by the
		/// <see cref="DateTime.Kind" /> property.
		/// </remarks>
		public static TimeSpan ToTimeSpan(this DateTime dateTime)
			=> dateTime.TimeOfDay;
	}
}
