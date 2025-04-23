// ---------------------------------------------------------------------------------------------------------------
// <copyright file="EvictingDictionary.CacheItem.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Collections;

namespace Bodu.Collections.Generic
{
	public partial class EvictingDictionary<TKey, TValue>
		: System.Collections.ICollection
	{
		[NonSerialized]
		private object? syncRoot;

		/// <inheritdoc />
		object ICollection.SyncRoot
		{
			get
			{
				// Thread-safe lazy initialization
				return syncRoot ?? Interlocked.CompareExchange(ref syncRoot, new object(), null) ?? syncRoot!;
			}
		}

		/// <inheritdoc />
		bool ICollection.IsSynchronized => false;

		/// <inheritdoc />
		void ICollection.CopyTo(Array array, int index)
		{
			ThrowHelper.ThrowIfNull(array);
			ThrowHelper.ThrowIfArrayIsNotSingleDimension(array);
			ThrowHelper.ThrowIfArrayIsNotZeroBased(array);
			ThrowHelper.ThrowIfLessThan(index, 0);
			ThrowHelper.ThrowIfArrayLengthIsInsufficient(array, index, store.Count);

			foreach (var kvp in GetOrderedItems())
			{
				array.SetValue(new DictionaryEntry(kvp.Key, kvp.Value), index++);
			}
		}
	}
}