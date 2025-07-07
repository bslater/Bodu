﻿// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="ComparableHelper.Max.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Bodu.Extensions
{
	public static partial class ComparableHelper
	{
		/// <summary>
		/// Returns the larger of two specified values.
		/// </summary>
		/// <typeparam name="T">The type of the values to compare, which must implement <see cref="IComparable{T}" />.</typeparam>
		/// <param name="first">The first value to compare.</param>
		/// <param name="second">The second value to compare.</param>
		/// <returns>
		/// The larger of the two values. If one value is <see langword="null" />, the non-null value is returned. If both values are
		/// <c>null</c>, <c>null</c> is returned.
		/// </returns>
		/// <remarks>This method compares the two values using the <see cref="IComparable{T}.CompareTo(T)" /> method.</remarks>
		public static T? Max<T>(T? first, T? second) where T : IComparable<T> =>
			first is null ? second : (second is null ? first : (first.CompareTo(second) >= 0 ? first : second));

		/// <summary>
		/// Returns the larger of two specified values using a custom <see cref="IComparer{T}" />.
		/// </summary>
		/// <typeparam name="T">The type of the values to compare.</typeparam>
		/// <param name="first">The first value to compare.</param>
		/// <param name="second">The second value to compare.</param>
		/// <param name="comparer">The comparer to use for comparing the values.</param>
		/// <returns>
		/// The larger of the two values based on the specified comparer. If one value is <see langword="null" />, the non-null value is
		/// returned. If both values are <c>null</c>, <c>null</c> is returned.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="comparer" /> is <see langword="null" />.</exception>
		/// <remarks>Use this overload to apply custom comparison logic, such as ordinal or case-insensitive string comparisons.</remarks>
		public static T? Max<T>(T? first, T? second, IComparer<T> comparer) =>
			comparer is null
				? throw new ArgumentNullException(nameof(comparer))
				: (first is null ? second : (second is null ? first : (comparer.Compare(first, second) >= 0 ? first : second)));
	}
}