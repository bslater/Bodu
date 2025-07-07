// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="LastDayOfYear.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the last day of the same calendar year as the specified instance.
		/// </summary>
		/// <param name="dateTime">The date and time value whose year is used to determine the result.</param>
		/// <returns>An object whose value is set to midnight (00:00:00) on December 31 of the same calendar year as <paramref name="dateTime" />.</returns>
		/// <remarks>
		/// <para>
		/// This method resets the date component to January 1 and the time component to midnight (00:00:00), preserving the original
		/// calendar year from <paramref name="dateTime" />.
		/// </para>
		/// <para>The <see cref="DateTime.Kind" /> property of the returned instance matches that of the original <paramref name="dateTime" />.</para>
		/// </remarks>
		public static DateTime LastDayOfYear(this DateTime dateTime) =>
			new(GetTicksForDate(dateTime.Year, 12, 31), dateTime.Kind);
	}
}