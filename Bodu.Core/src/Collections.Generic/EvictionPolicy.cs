// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="EvictionPolicy.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Collections.Generic
{
	/// <summary>
	/// Specifies the eviction strategy used by a <see cref="EvictingDictionary{TKey, TValue}" /> when its capacity is exceeded.
	/// </summary>
	/// <remarks>
	/// The <see cref="EvictionPolicy" /> enumeration defines how items are selected for removal when the dictionary reaches its maximum
	/// capacity. This allows the dictionary to behave like a queue, cache, or frequency-based store, depending on the selected strategy.
	/// </remarks>
	public enum EvictionPolicy
	{
		/// <summary>
		/// First-In-First-Out: the oldest item added to the dictionary is removed first, regardless of usage.
		/// </summary>
		FirstInFirstOut,

		/// <summary>
		/// Least Recently Used: the item that has not been accessed for the longest time is removed first.
		/// </summary>
		LeastRecentlyUsed,

		/// <summary>
		/// Least Frequently Used: the item with the fewest total accesses is removed first.
		/// </summary>
		LeastFrequentlyUsed,

		/// <summary>
		/// Most Recently Used: the most recently accessed item is evicted first.
		/// </summary>
		MostRecentlyUsed,

		/// <summary>
		/// Random Replacement: a randomly selected item is removed.
		/// </summary>
		RandomReplacement,

		/// <summary>
		/// Second Chance: items are given a second chance before being evicted, based on a reference flag.
		/// </summary>
		SecondChance
	}
}