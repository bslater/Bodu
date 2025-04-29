// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="ComparableHelper.IsBetween.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	/// <summary>
	/// Provides helper methods for comparing values.
	/// </summary>
	public static partial class ComparableHelper
	{
		/// <summary>
		/// Determines whether a value falls inclusively between two specified boundaries.
		/// </summary>
		/// <typeparam name="T">The type of the value to compare, which must implement <see cref="IComparable{T}" />.</typeparam>
		/// <param name="value">The value to test.</param>
		/// <param name="value1">The first boundary.</param>
		/// <param name="value2">The second boundary.</param>
		/// <returns>
		/// <c>true</c> if <paramref name="value" /> falls between <paramref name="value1" /> and <paramref name="value2" /> inclusively;
		/// otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>
		/// <para>If any of the parameters are <c>null</c>, the method returns <c>false</c>.</para>
		/// <para>The order of <paramref name="value1" /> and <paramref name="value2" /> does not matter.</para>
		/// </remarks>
		public static bool IsBetween<T>(T? value, T? value1, T? value2) where T : IComparable<T>
		{
			if (value is null || value1 is null || value2 is null)
				return false;

			return value1.CompareTo(value2) > 0
				? value.CompareTo(value2) >= 0 && value.CompareTo(value1) <= 0
				: value.CompareTo(value1) >= 0 && value.CompareTo(value2) <= 0;
		}

		/// <summary>
		/// Determines whether a value falls inclusively between two specified boundaries using a custom <see cref="IComparer{T}" />.
		/// </summary>
		/// <typeparam name="T">The type of the value to compare.</typeparam>
		/// <param name="value">The value to test.</param>
		/// <param name="value1">The first boundary.</param>
		/// <param name="value2">The second boundary.</param>
		/// <param name="comparer">The comparer to use for comparing values.</param>
		/// <returns>
		/// <c>true</c> if <paramref name="value" /> falls between <paramref name="value1" /> and <paramref name="value2" /> inclusively
		/// based on the specified comparer; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="comparer" /> is <c>null</c>.</exception>
		/// <remarks>
		/// <para>If any of the parameters are <c>null</c>, the method returns <c>false</c>.</para>
		/// <para>The order of <paramref name="value1" /> and <paramref name="value2" /> does not matter.</para>
		/// </remarks>
		public static bool IsBetween<T>(T? value, T? value1, T? value2, IComparer<T> comparer)
		{
			if (comparer is null)
				throw new ArgumentNullException(nameof(comparer));

			if (value is null || value1 is null || value2 is null)
				return false;

			return comparer.Compare(value1, value2) > 0
				? comparer.Compare(value, value2) >= 0 && comparer.Compare(value, value1) <= 0
				: comparer.Compare(value, value1) >= 0 && comparer.Compare(value, value2) <= 0;
		}
	}
}