// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.ToDateOnly.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
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
		/// <returns>A <see cref="DateOnly" /> representing the date component of the input <paramref name="dateTime" />.</returns>
		/// <remarks>
		/// This method is available on .NET 6.0 or later. The resulting <see cref="DateOnly" /> does not retain the time or
		/// <see cref="DateTime.Kind" /> information.
		/// </remarks>
		public static DateOnly ToDateOnly(this DateTime dateTime) =>
			DateOnly.FromDayNumber(DateOnlyExtensions.GetDayNumber(dateTime));

#endif
	}
}