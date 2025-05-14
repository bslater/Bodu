// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.Max.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the later of two specified <see cref="DateTime" /> values.
		/// </summary>
		/// <param name="first">The first <see cref="DateTime" /> value to compare.</param>
		/// <param name="second">The second <see cref="DateTime" /> value to compare.</param>
		/// <returns>The later of the two <see cref="DateTime" /> values. If both values are equal, <paramref name="first" /> is returned.</returns>
		/// <remarks>This method compares the two values using <see cref="DateTime.CompareTo(DateTime)" />.</remarks>
		public static DateTime Max(DateTime first, DateTime second) => first >= second ? first : second;

		/// <summary>
		/// Returns the later of two specified nullable <see cref="DateTime" /> values.
		/// </summary>
		/// <param name="first">The first nullable <see cref="DateTime" /> value to compare.</param>
		/// <param name="second">The second nullable <see cref="DateTime" /> value to compare.</param>
		/// <returns>The later of the two <see cref="DateTime" /> values, or <c>null</c> if both are <c>null</c>.</returns>
		/// <remarks>
		/// If both values are non-null, they are compared using <see cref="DateTime.CompareTo(DateTime)" />. If only one value is non-null,
		/// it is returned.
		/// </remarks>
		public static DateTime? Max(DateTime? first, DateTime? second) =>
			first.HasValue && second.HasValue ? (first.Value >= second.Value ? first : second) : first ?? second;
	}
}