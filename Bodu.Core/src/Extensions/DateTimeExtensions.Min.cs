// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.Min.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the earlier of two specified <see cref="DateTime" /> values.
		/// </summary>
		/// <param name="first">The first <see cref="DateTime" /> value to compare.</param>
		/// <param name="second">The second <see cref="DateTime" /> value to compare.</param>
		/// <returns>The earlier of the two <see cref="DateTime" /> values. If both values are equal, <paramref name="first" /> is returned.</returns>
		/// <remarks>
		/// This method compares the two <see cref="DateTime" /> values using the <see cref="DateTime.CompareTo(DateTime)" /> method. The
		/// result preserves the original <see cref="DateTime.Kind" /> property (e.g., <c>Utc</c>, <c>Local</c>, or <c>Unspecified</c>).
		/// </remarks>
		public static DateTime Min(DateTime first, DateTime second) => first <= second ? first : second;

		/// <summary>
		/// Returns the earlier of two specified nullable <see cref="DateTime" /> values.
		/// </summary>
		/// <param name="first">The first nullable <see cref="DateTime" /> value to compare.</param>
		/// <param name="second">The second nullable <see cref="DateTime" /> value to compare.</param>
		/// <returns>The earlier of the two <see cref="DateTime" /> values, or <c>null</c> if both are <c>null</c>.</returns>
		/// <remarks>
		/// If one value is <c>null</c>, the non-null value is returned. If both values are non-null, they are compared using the
		/// <see cref="DateTime.CompareTo(DateTime)" /> method.
		/// </remarks>
		public static DateTime? Min(DateTime? first, DateTime? second) =>
			first.HasValue && second.HasValue ? (first.Value <= second.Value ? first : second) : first ?? second;
	}
}