// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateOnlyExtensions.IsInRange.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Determines whether the specified <see cref="DateOnly" /> value falls within the range defined by <paramref name="start" /> and
		/// <paramref name="end" />, inclusive.
		/// </summary>
		/// <param name="value">The <see cref="DateOnly" /> value to evaluate.</param>
		/// <param name="start">The start of the range.</param>
		/// <param name="end">The end of the range.</param>
		/// <returns>
		/// <c>true</c> if <paramref name="value" /> is greater than or equal to <paramref name="start" /> and less than or equal to
		/// <paramref name="end" />; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method automatically handles reversed ranges by swapping <paramref name="start" /> and <paramref name="end" /> if necessary.
		/// </para>
		/// <para>The range is inclusive; if <paramref name="value" /> equals either boundary, the result is <c>true</c>.</para>
		/// </remarks>
		public static bool IsInRange(this DateOnly value, DateOnly start, DateOnly end) =>
			ComparableHelper.IsBetween(value, start, end);

		/// <summary>
		/// Determines whether the specified nullable <see cref="DateOnly" /> value falls within the range defined by
		/// <paramref name="start" /> and <paramref name="end" />, inclusive.
		/// </summary>
		/// <param name="value">The nullable <see cref="DateOnly" /> value to evaluate.</param>
		/// <param name="start">The start of the range.</param>
		/// <param name="end">The end of the range.</param>
		/// <returns>
		/// <c>true</c> if <paramref name="value" /> has a value and that value is greater than or equal to <paramref name="start" /> and
		/// less than or equal to <paramref name="end" />; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>
		/// <para>If <paramref name="value" /> is <c>null</c>, this method returns <c>false</c>.</para>
		/// <para>
		/// This method automatically handles reversed ranges by swapping <paramref name="start" /> and <paramref name="end" /> if necessary.
		/// </para>
		/// <para>The range is inclusive; if the underlying value equals either boundary, the result is <c>true</c>.</para>
		/// </remarks>
		public static bool IsInRange(this DateOnly? value, DateOnly start, DateOnly end) =>
			value.HasValue && ComparableHelper.IsBetween(value.Value, start, end);
	}
}