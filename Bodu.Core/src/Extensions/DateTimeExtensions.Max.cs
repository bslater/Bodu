// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="Max.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
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
		/// <remarks>
		/// This method compares the two values using the greater-than-or-equal-to ( <c>&gt;=</c>) operator, which is equivalent to <see cref="DateTime.CompareTo(DateTime)" />.
		/// </remarks>
		public static DateTime Max(DateTime first, DateTime second) =>
			first >= second ? first : second;

		/// <summary>
		/// Returns the later of two specified nullable <see cref="DateTime" /> values.
		/// </summary>
		/// <param name="first">The first nullable <see cref="DateTime" /> value to compare.</param>
		/// <param name="second">The second nullable <see cref="DateTime" /> value to compare.</param>
		/// <returns>
		/// The later of the two <see cref="DateTime" /> values, or <c>null</c> if both <paramref name="first" /> and
		/// <paramref name="second" /> are <c>null</c>.
		/// </returns>
		/// <remarks>
		/// <para>If both values are non-null, they are compared using the greater-than-or-equal-to ( <c>&gt;=</c>) operator.</para>
		/// <para>If only one value is non-null, that value is returned. If both are <c>null</c>, the result is <c>null</c>.</para>
		/// </remarks>
		public static DateTime? Max(DateTime? first, DateTime? second) =>
			first.HasValue && second.HasValue ? (first.Value >= second.Value ? first : second) : first ?? second;
	}
}