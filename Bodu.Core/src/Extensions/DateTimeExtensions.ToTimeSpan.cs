// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="ToTimeSpan.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a <see cref="TimeSpan" /> representing the time-of-day portion of the specified <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> from which to extract the time component.</param>
		/// <returns>
		/// A <see cref="TimeSpan" /> containing the hour, minute, second, and fractional seconds elapsed since midnight on the day of <paramref name="dateTime" />.
		/// </returns>
		/// <remarks>The result represents the duration since midnight and is unaffected by the <see cref="DateTime.Kind" />.</remarks>
		public static TimeSpan ToTimeSpan(this DateTime dateTime) =>
			dateTime.TimeOfDay;
	}
}