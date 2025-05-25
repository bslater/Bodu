// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="IEnumerableExtensions.Batch.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

#if !NETSTANDARD2_0

using Bodu.Buffers;
using Bodu.Collections.Generic.Internal;

#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Bodu.Collections.Generic.Extensions
{
	public static partial class IEnumerableExtensions
	{
		/// <summary>
		/// Determines whether the source sequence contains all of the specified items.
		/// </summary>
		/// <typeparam name="T">The type of elements in the sequences.</typeparam>
		/// <param name="source">The source sequence to search.</param>
		/// <param name="items">The items to verify against the source sequence.</param>
		/// <param name="comparer">An optional equality comparer to use; if <see langword="null" />, the default comparer is used.</param>
		/// <returns><see langword="true" /> if all items exist in <paramref name="source" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="source" /> or <paramref name="items" /> is <see langword="null" />.</exception>
		public static bool ContainsAll<T>(
			this IEnumerable<T> source,
			IEnumerable<T> items,
			IEqualityComparer<T>? comparer = null)
		{
			ThrowHelper.ThrowIfNull(source);
			ThrowHelper.ThrowIfNull(items);

			// Avoid unnecessary allocation if possible
			items = SequenceUtility.EnsureMaterialized(items);

			if (!items.Any())
				return true;

			// Materialize the source for fast membership checks
			var sourceSet = new HashSet<T>(source, comparer);
			if (sourceSet.Count == 0)
				return false;

#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions

			// Using a foreach loop instead of LINQ to:
			// - Avoid delegate allocation from predicate-based .Any(...)
			// - Ensure optimal short-circuit performance
			// - Preserve debuggability and allow future extensions with minimal cost
			foreach (var item in items)
			{
				if (!sourceSet.Contains(item))
					return false;
			}
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions

			return true;
		}
	}
}