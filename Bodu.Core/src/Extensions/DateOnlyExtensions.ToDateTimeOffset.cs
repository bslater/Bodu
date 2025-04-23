// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.ToDateOnlyOffset.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Converts the specified <see cref="DateOnly" /> to a <see cref="DateOnlyOffset" /> based
		/// on its kind.
		/// </summary>
		/// <param name="date">The input <see cref="DateOnly" />.</param>
		/// <returns>A new <see cref="DateOnlyOffset" /> representing the same point in time.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if the resulting UTC time is outside valid range.
		/// </exception>
		/// <remarks>This method respects the <see cref="DateOnly.Kind" /> property of the input.</remarks>
		public static DateTimeOffset ToDateOnlyOffset(this DateOnly date)
			=> new DateTimeOffset(date);

		// <summary>
		// Converts the specified <see cref="DateOnly" /> to a <see cref="DateOnlyOffset" /> with
		// the specified UTC offset.
		// </summary>
		/// <param name="date">The input <see cref="DateOnly" />.</param>
		/// <param name="offset">The offset to apply from UTC.</param>
		/// <returns>A <see cref="DateOnlyOffset" /> with the given offset.</returns>
		/// <exception cref="System.ArgumentException">
		/// Thrown if <paramref name="offset" /> is inconsistent with
		/// <paramref name="dateTime.Kind" /> or invalid.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if the resulting value is outside the allowable range.
		/// </exception>
		public static DateTimeOffset ToDateOnlyOffset(this DateOnly date, TimeSpan offset)
			=> new DateTimeOffset(date, offset);
	}
}