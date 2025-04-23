// ---------------------------------------------------------------------------------------------------------------
// <copyright file="EvictionPolicy.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Collections.Generic
{
	/// <summary>
	/// Specifies the eviction strategy used by a <see cref="EvictingDictionary{TKey, TValue}" />
	/// when its capacity is exceeded.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="EvictionPolicy" /> enumeration defines how items are selected for removal
	/// when the dictionary reaches its maximum capacity. This allows the dictionary to behave like
	/// a queue, cache, or frequency-based store, depending on the selected strategy.
	/// </para>
	/// <para>The following eviction strategies are available:</para>
	/// <list type="bullet">
	/// <item>
	/// <description>
	/// <see cref="FirstInFirstOut" /> - removes the oldest inserted item first, regardless of
	/// access pattern.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <see cref="LeastRecentlyUsed" /> - removes the item that has not been accessed for the
	/// longest period of time.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <see cref="LeastFrequentlyUsed" /> - removes the item with the lowest number of access
	/// operations over its lifetime.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <see cref="MostRecentlyUsed" /> - removes the most recently accessed item first.
	/// </description>
	/// </item>
	/// <item>
	/// <description><see cref="RandomReplacement" /> - evicts an item chosen at random.</description>
	/// </item>
	/// <item>
	/// <description>
	/// <see cref="SecondChance" /> - gives items a second chance before eviction, based on a
	/// reference bit.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	public enum EvictionPolicy
	{
		/// <summary>
		/// First-In-First-Out: the oldest item added to the dictionary is removed first, regardless
		/// of usage.
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
		/// Second Chance: items are given a second chance before being evicted, based on a
		/// reference flag.
		/// </summary>
		SecondChance
	}
}