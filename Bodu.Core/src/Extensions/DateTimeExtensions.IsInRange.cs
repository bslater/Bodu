// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="IsInRange.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Determines whether the specified <see cref="DateTime" /> value falls within the inclusive range defined by
		/// <paramref name="start" /> and <paramref name="end" />.
		/// </summary>
		/// <param name="value">The <see cref="DateTime" /> value to evaluate.</param>
		/// <param name="start">The start of the range.</param>
		/// <param name="end">The end of the range.</param>
		/// <returns>
		/// <see langword="true" /> if <paramref name="value" /> is greater than or equal to <paramref name="start" /> and less than or
		/// equal to <paramref name="end" />; otherwise, <see langword="false" />.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The range check is inclusive, meaning the result is <see langword="true" /> if <paramref name="value" /> equals either
		/// <paramref name="start" /> or <paramref name="end" />.
		/// </para>
		/// <para>
		/// If <paramref name="start" /> is greater than <paramref name="end" />, the method automatically swaps the values before
		/// performing the comparison.
		/// </para>
		/// </remarks>
		public static bool IsInRange(this DateTime value, DateTime start, DateTime end) =>
			ComparableHelper.IsBetween(value, start, end);

		/// <summary>
		/// Determines whether the specified nullable <see cref="DateTime" /> value falls within the inclusive range defined by
		/// <paramref name="start" /> and <paramref name="end" />.
		/// </summary>
		/// <param name="value">The nullable <see cref="DateTime" /> value to evaluate.</param>
		/// <param name="start">The start of the range.</param>
		/// <param name="end">The end of the range.</param>
		/// <returns>
		/// <see langword="true" /> if <paramref name="value" /> is not <see langword="null" /> and its value is greater than or equal to
		/// <paramref name="start" /> and less than or equal to <paramref name="end" />; otherwise, <see langword="false" />.
		/// </returns>
		/// <remarks>
		/// <para>If <paramref name="value" /> is <see langword="null" />, the result is <see langword="false" />.</para>
		/// <para>The range check is inclusive, meaning the result is <see langword="true" /> if the value equals either boundary.</para>
		/// <para>
		/// If <paramref name="start" /> is greater than <paramref name="end" />, the method automatically swaps the values before
		/// performing the comparison.
		/// </para>
		/// </remarks>
		public static bool IsInRange(this DateTime? value, DateTime start, DateTime end) =>
			value.HasValue && ComparableHelper.IsBetween(value.Value, start, end);
	}
}