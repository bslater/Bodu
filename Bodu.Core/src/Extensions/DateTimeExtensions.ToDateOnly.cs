// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="ToDateOnly.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
#if NET6_0_OR_GREATER // Uses DateOnly for .NET 6 or greater

		/// <summary>
		/// Converts the specified <see cref="DateTime" /> to a <see cref="DateOnly" />, preserving only the year, month, and day components.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> instance to convert.</param>
		/// <returns>
		/// A <see cref="DateOnly" /> representing the calendar date of the specified <paramref name="dateTime" />, without any time-of-day
		/// or <see cref="DateTime.Kind" /> information.
		/// </returns>
		/// <remarks>
		/// This method is supported in .NET 6.0 or later. The returned <see cref="DateOnly" /> is constructed from the
		/// <see cref="DateTime.Year" />, <see cref="DateTime.Month" />, and <see cref="DateTime.Day" /> components of the input.
		/// </remarks>
		public static DateOnly ToDateOnly(this DateTime dateTime) =>
			DateOnly.FromDayNumber(GetDayNumber(dateTime));

#endif
	}
}