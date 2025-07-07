// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="EvictingDictionary.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bodu.Collections.Generic
{
	/// <summary>
	/// Represents a fixed-capacity dictionary that automatically removes entries based on a chosen eviction policy, such as
	/// First-In-First-Out (FirstInFirstOut), Least Recently Used (LeastRecentlyUsed), or Least Frequently Used (LeastFrequentlyUsed).
	/// </summary>
	/// <typeparam name="TKey">Specifies the type of keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">Specifies the type of values in the dictionary.</typeparam>
	/// <remarks>
	/// <para>
	/// <see cref="EvictingDictionary{TKey, TValue}" /> maintains a maximum number of key-value pairs and automatically evicts items when
	/// capacity is exceeded. Eviction is determined by a specified <see cref="EvictingDictionaryPolicy" />, allowing this dictionary to behave like a
	/// queue, an access-order cache, or a frequency-based cache.
	/// </para>
	/// <para>
	/// <see cref="EvictingDictionary{TKey, TValue}" /> allows <see langword="null" /> keys and values (for reference types) and supports
	/// custom key equality via <see cref="System.Collections.Generic.IEqualityComparer{T}" />.
	/// </para>
	/// <example>
	/// <code language="csharp">
	/// <![CDATA[
	/// // Create an evicting dictionary with capacity for 2 items
	/// var cache = new EvictingDictionary<string, in>(capacity: 2)
	/// {
	///     Policy = EvictionPolicy.LeastRecentlyUsed
	/// };
	///
	/// // Add two entries
	/// cache["A"] = 1;
	/// cache["B"] = 2;
	///
	/// // Touch "A" to mark it as recently used
	/// cache.Touch("A");
	///
	/// // Add a third entry; "B" is now the least recently used and will be evicted
	/// cache["C"] = 3;
	///
	/// // Dictionary now contains: { "A": 1, "C": 3 }
	///
	/// // Display current entries
	/// foreach (var kvp in cache)
	///     Console.WriteLine($"{kvp.Key} = {kvp.Value}");
	///
	/// // Output:
	/// // A = 1
	/// // C = 3
	///]]>
	/// </code>
	/// </example>
	/// </remarks>
	[DebuggerDisplay("Count: {Count}, Capacity: {_capacity}, Policy: {_evictingPolicy}")]
	[DebuggerTypeProxy(typeof(EvictingDictionaryDebugView<,>))]
	[Serializable]
	public partial class EvictingDictionary<TKey, TValue>
		where TKey : notnull
	{
		private const int DefaultCapacity = 16;
		private const EvictingDictionaryPolicy DefaultPolicy = EvictingDictionaryPolicy.LeastRecentlyUsed;

		private readonly int _capacity;
		private readonly IEqualityComparer<TKey> _comparer;
		private readonly EvictingDictionaryPolicy _evictingPolicy;
		private readonly SortedDictionary<int, LinkedList<TKey>> _frequencyList = null!;
		private readonly Dictionary<TKey, CacheItem> store;
		private long _evictionCount;
		private LinkedList<TKey> _order = null!;
		private long _totalTouches;

		/// <summary>
		/// Initializes a new empty <see cref="EvictingDictionary{TKey, TValue}" /> with the default capacity and eviction policy.
		/// </summary>
		/// <remarks>The default capacity is 16, and the default eviction policy is <see cref="EvictingDictionaryPolicy.LeastRecentlyUsed" />.</remarks>
		public EvictingDictionary()
			: this(DefaultCapacity, DefaultPolicy, null) { }

		/// <summary>
		/// Initializes a new empty <see cref="EvictingDictionary{TKey, TValue}" /> with the specified capacity and the default eviction policy.
		/// </summary>
		/// <param name="capacity">The maximum number of key-value pairs the dictionary can contain. Must be positive.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than or equal to zero.</exception>
		/// <remarks>The default eviction policy is <see cref="EvictingDictionaryPolicy.LeastRecentlyUsed" />.</remarks>
		public EvictingDictionary(int capacity)
			: this(capacity, DefaultPolicy, null) { }

		/// <summary>
		/// Initializes a new empty <see cref="EvictingDictionary{TKey, TValue}" /> with the specified capacity and eviction policy.
		/// </summary>
		/// <param name="capacity">The maximum number of key-value pairs the dictionary can contain. Must be positive.</param>
		/// <param name="policy">The eviction policy to use when the dictionary exceeds its capacity.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than or equal to zero.</exception>
		public EvictingDictionary(int capacity, EvictingDictionaryPolicy policy)
					: this(capacity, policy, null) { }

		/// <summary>
		/// Initializes a new empty <see cref="EvictingDictionary{TKey, TValue}" /> with the specified capacity and key comparer.
		/// </summary>
		/// <param name="capacity">The maximum number of key-value pairs the dictionary can contain. Must be positive.</param>
		/// <param name="comparer">An equality comparer to use for comparing keys, or <see langword="null" /> to use the default comparer.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than or equal to zero.</exception>
		/// <remarks>The default eviction policy is <see cref="EvictingDictionaryPolicy.LeastRecentlyUsed" />.</remarks>
		public EvictingDictionary(int capacity, IEqualityComparer<TKey>? comparer)
			: this(capacity, DefaultPolicy, comparer) { }

		/// <summary>
		/// Initializes a new empty <see cref="EvictingDictionary{TKey, TValue}" /> with the specified capacity, eviction policy, and key comparer.
		/// </summary>
		/// <param name="capacity">The maximum number of key-value pairs the dictionary can contain. Must be positive.</param>
		/// <param name="policy">The eviction policy to use when the dictionary exceeds its capacity.</param>
		/// <param name="comparer">An equality comparer to use for comparing keys, or <see langword="null" /> to use the default comparer.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="capacity" /> is less than or equal to zero. A non-zero capacity is required to allow storing items.
		/// </exception>
		public EvictingDictionary(int capacity, EvictingDictionaryPolicy policy, IEqualityComparer<TKey>? comparer)
		{
			ThrowHelper.ThrowIfZeroOrNegative(capacity);

			_capacity = capacity;
			_evictingPolicy = policy;
			_comparer = comparer ?? EqualityComparer<TKey>.Default;

			store = new Dictionary<TKey, CacheItem>(_comparer);

			switch (_evictingPolicy)
			{
				case EvictingDictionaryPolicy.FirstInFirstOut:
				case EvictingDictionaryPolicy.LeastRecentlyUsed:
				case EvictingDictionaryPolicy.MostRecentlyUsed:
				case EvictingDictionaryPolicy.SecondChance:
					_order = new LinkedList<TKey>();
					break;

				case EvictingDictionaryPolicy.LeastFrequentlyUsed:
					_frequencyList = new SortedDictionary<int, LinkedList<TKey>>();
					break;

				case EvictingDictionaryPolicy.RandomReplacement:

					// No additional structure required
					break;
			}
		}

		/// <summary>
		/// Initializes a new <see cref="EvictingDictionary{TKey, TValue}" /> by copying entries from the specified dictionary, using
		/// default capacity and eviction policy.
		/// </summary>
		/// <param name="source">The enumerable collection of key-value pairs to copy. Must not be null.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="source" /> is <see langword="null" />.</exception>
		/// <remarks>
		/// If the number of entries in <paramref name="source" /> exceeds the default capacity (16), only the most recent entries are
		/// retained according to the default eviction policy ( <see cref="EvictingDictionaryPolicy.LeastRecentlyUsed" />).
		/// </remarks>
		public EvictingDictionary(IEnumerable<KeyValuePair<TKey, TValue>> source)
			: this(DefaultCapacity, source, DefaultPolicy, null) { }

		/// <summary>
		/// Initializes a new <see cref="EvictingDictionary{TKey, TValue}" /> by copying entries from the specified enumerable source, using
		/// the specified capacity and the default eviction policy ( <see cref="EvictingDictionaryPolicy.LeastRecentlyUsed" />).
		/// </summary>
		/// <param name="capacity">The maximum number of key-value pairs the dictionary can contain. Must be a positive integer.</param>
		/// <param name="source">The enumerable collection of key-value pairs to copy into the dictionary. Must not be <see langword="null" />.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">/// Thrown when <paramref name="capacity" /> is less than or equal to zero.</exception>
		/// <remarks>
		/// If the number of elements in <paramref name="source" /> exceeds <paramref name="capacity" />, only the most recently added
		/// entries will be retained based on the default eviction policy ( <see cref="EvictingDictionaryPolicy.LeastRecentlyUsed" />).
		/// </remarks>
		public EvictingDictionary(int capacity, IEnumerable<KeyValuePair<TKey, TValue>> source)
			: this(capacity, source, DefaultPolicy, null) { }

		/// <summary>
		/// Initializes a new <see cref="EvictingDictionary{TKey, TValue}" /> by copying entries from the specified dictionary, using the
		/// specified eviction policy and default capacity.
		/// </summary>
		/// <param name="source">The enumerable collection of key-value pairs to copy. Must not be null.</param>
		/// <param name="policy">The eviction policy to use when the dictionary exceeds its capacity.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="source" /> is <see langword="null" />.</exception>
		public EvictingDictionary(IEnumerable<KeyValuePair<TKey, TValue>> source, EvictingDictionaryPolicy policy)
			: this(DefaultCapacity, source, policy, null) { }

		/// <summary>
		/// Initializes a new <see cref="EvictingDictionary{TKey, TValue}" /> by copying entries from the specified dictionary, using the
		/// specified capacity and eviction policy.
		/// </summary>
		/// <param name="capacity">The maximum number of key-value pairs the dictionary can contain. Must be positive.</param>
		/// <param name="source">The enumerable collection of key-value pairs to copy. Must not be null.</param>
		/// <param name="policy">The eviction policy to use when the dictionary exceeds its capacity.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than or equal to zero.</exception>
		public EvictingDictionary(int capacity, IEnumerable<KeyValuePair<TKey, TValue>> source, EvictingDictionaryPolicy policy)
			: this(capacity, source, policy, null) { }

		/// <summary>
		/// Initializes a new <see cref="EvictingDictionary{TKey, TValue}" /> by copying entries from the specified dictionary, using the
		/// specified capacity, eviction policy, and key comparer.
		/// </summary>
		/// <param name="capacity">The maximum number of key-value pairs the dictionary can contain. Must be positive.</param>
		/// <param name="source">The enumerable collection of key-value pairs to copy. Must not be null.</param>
		/// <param name="policy">The eviction policy to use when the dictionary exceeds its capacity.</param>
		/// <param name="comparer">An equality comparer to use for comparing keys, or <see langword="null" /> to use the default comparer.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than or equal to zero.</exception>
		public EvictingDictionary(int capacity, IEnumerable<KeyValuePair<TKey, TValue>> source, EvictingDictionaryPolicy policy, IEqualityComparer<TKey>? comparer)
			: this(capacity, policy, comparer)
		{
			ThrowHelper.ThrowIfNull(source);

			foreach (var kvp in source)
				Add(kvp.Key, kvp.Value);
		}

		/// <summary>
		/// Occurs immediately <b>after</b> an item is evicted from the
		/// <see cref="EvictingDictionary{TKey, TValue}" /> due to capacity limits.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This event is raised after the item has been removed from the collection, based on the
		/// configured <see cref="EvictingDictionaryPolicy" /> (e.g., FirstInFirstOut, LeastRecentlyUsed, or LeastFrequentlyUsed).
		/// </para>
		/// <para>
		/// Consumers can use this event to record historical data, notify observers, or synchronize
		/// external caches. The key and value provided are no longer present in the dictionary.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code language="csharp"><![CDATA[
		/// var cache = new EvictingDictionary<string, int>(capacity: 2, policy: EvictionPolicy.FirstInFirstOut);
		/// cache.ItemEvicted += (key, value) =>
		/// {
		///     Console.WriteLine($""[AfterEvict] {key} = {value}"");
		/// };
		///
		/// cache.Add(""A"", 1);
		/// cache.Add(""B"", 2);
		/// cache.Add(""C"", 3); // Triggers ItemEvicted for ""A""
		///]]>
		/// </code>
		/// </example>
		public event Action<TKey, TValue>? ItemEvicted;

		/// <summary>
		/// Occurs immediately <b>before</b> an item is evicted from the
		/// <see cref="EvictingDictionary{TKey, TValue}" /> due to capacity limits.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This event is raised before the item is removed from the collection, allowing consumers
		/// to inspect the key and value before eviction occurs.
		/// </para>
		/// <para>
		/// Common use cases include diagnostics, logging, cache warm-up, or state mirroring. This
		/// event is informational and cannot cancel or delay eviction.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code language="csharp"><![CDATA[
		/// var cache = new EvictingDictionary<string, int>(capacity: 2, policy: EvictionPolicy.FirstInFirstOut);
		/// cache.ItemEvicting += (key, value) =>
		/// {
		///     Console.WriteLine($""[BeforeEvict] {key} = {value}"");
		/// };
		///
		/// cache.Add(""A"", 1);
		/// cache.Add(""B"", 2);
		/// cache.Add(""C"", 3); // Triggers ItemEvicting for ""A""
		///]]>
		/// </code>
		/// </example>
		public event Action<TKey, TValue>? ItemEvicting;

		// FirstInFirstOut/LeastRecentlyUsed LeastFrequentlyUsed}
		/// <summary>
		/// Gets the maximum number of items that can be stored in the dictionary before eviction occurs.
		/// </summary>
		public int Capacity => _capacity;

		/// <summary>
		/// Gets the total number of items evicted from the dictionary since creation.
		/// </summary>
		public long EvictionCount => _evictionCount;

		/// <summary>
		/// Gets the eviction policy configured for this dictionary.
		/// </summary>
		public EvictingDictionaryPolicy Policy => _evictingPolicy;

		/// <summary>
		/// Gets the total number of times any key has been accessed or touched.
		/// </summary>
		public long TotalTouches => _totalTouches;

		/// <summary>
		/// Returns the key that would be evicted next based on the current eviction policy and internal state.
		/// </summary>
		/// <remarks>
		/// The eviction candidate depends on the selected <see cref="EvictingDictionaryPolicy" />:
		/// <list type="bullet">
		/// <item>
		/// <description><b>FirstInFirstOut</b>: returns the oldest inserted key.</description>
		/// </item>
		/// <item>
		/// <description><b>LeastRecentlyUsed</b>: returns the least recently accessed key.</description>
		/// </item>
		/// <item>
		/// <description><b>MostRecentlyUsed</b>: returns the most recently accessed key.</description>
		/// </item>
		/// <item>
		/// <description><b>LeastFrequentlyUsed</b>: returns the key with the fewest total accesses.</description>
		/// </item>
		/// <item>
		/// <description><b>RandomReplacement</b>: returns an arbitrary key from the dictionary.</description>
		/// </item>
		/// <item>
		/// <description>
		/// <b>SecondChance</b>: returns the first key that has not been accessed recently; falls back to FIFO if all have second chances.
		/// </description>
		/// </item>
		/// </list>
		/// </remarks>
		/// <returns>The key that is next in line for eviction.</returns>
		public TKey? PeekEvictionCandidate()
		{
			if ((_evictingPolicy == EvictingDictionaryPolicy.FirstInFirstOut || _evictingPolicy == EvictingDictionaryPolicy.LeastRecentlyUsed) &&
				_order.First is not null)
			{
				return _order.First.Value;
			}
			else if (_evictingPolicy == EvictingDictionaryPolicy.MostRecentlyUsed && _order.Last is not null)
			{
				return _order.Last.Value;
			}
			else if (_evictingPolicy == EvictingDictionaryPolicy.LeastFrequentlyUsed &&
					 _frequencyList.Count > 0)
			{
				var bucket = _frequencyList.First().Value;
				if (bucket.First is not null)
					return bucket.First.Value;
			}
			else if (_evictingPolicy == EvictingDictionaryPolicy.RandomReplacement && store.Count > 0)
			{
				return store.Keys.First(); // Consider using a true RNG if needed
			}
			else if (_evictingPolicy == EvictingDictionaryPolicy.SecondChance && _order is not null)
			{
				foreach (var key in _order)
				{
					if (store.TryGetValue(key, out var item) && !item.SecondChance)
						return key;
				}

				if (_order.First is not null)
					return _order.First.Value;
			}

			return default!;
		}

		/// <summary>
		/// Marks the specified key as recently accessed without retrieving its value. If the eviction policy involves usage tracking, this
		/// updates the internal usage metadata.
		/// </summary>
		/// <param name="key">The key to touch.</param>
		/// <returns><see langword="true" /> if the key exists and was marked as accessed; otherwise, <see langword="false" />.</returns>
		public bool Touch(TKey key)
		{
			if (store.TryGetValue(key, out var item))
			{
				TouchInternal(key, item);
				_totalTouches++;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Marks the specified key as recently accessed without retrieving its value, and throws an exception if the key does not exist in
		/// the dictionary.
		/// </summary>
		/// <param name="key">The key to touch.</param>
		/// <exception cref="KeyNotFoundException">Thrown when the specified key does not exist in the dictionary.</exception>
		/// <remarks>
		/// If the eviction policy is <see cref="EvictingDictionaryPolicy.LeastRecentlyUsed" /> or
		/// <see cref="EvictingDictionaryPolicy.LeastFrequentlyUsed" />, this updates the internal usage metadata.
		/// </remarks>
		public void TouchOrThrow(TKey key)
		{
			if (!Touch(key))
				throw new KeyNotFoundException($"The key '{key}' was not found in the dictionary.");
		}

		/// <summary>
		/// Adds the specified key to the LeastFrequentlyUsed frequency bucket for the given frequency.
		/// </summary>
		/// <param name="frequency">The new frequency count.</param>
		/// <param name="key">The key to add.</param>
		private void AddToFrequencyList(int frequency, TKey key)
		{
			if (!_frequencyList.TryGetValue(frequency, out var list))
			{
				list = new LinkedList<TKey>();
				_frequencyList[frequency] = list;
			}

			list.AddLast(key);
		}

		/// <summary>
		/// Removes the next item to be evicted based on the current eviction policy. Raises the <see cref="ItemEvicted" /> event and
		/// updates eviction metrics.
		/// </summary>
		private void EvictOne()
		{
			TKey? keyToRemove = _evictingPolicy switch
			{
				EvictingDictionaryPolicy.FirstInFirstOut or EvictingDictionaryPolicy.LeastRecentlyUsed
					when _order?.First is not null => _order.First.Value,

				EvictingDictionaryPolicy.MostRecentlyUsed
					when _order?.Last is not null => _order.Last.Value,

				EvictingDictionaryPolicy.LeastFrequentlyUsed
					when _frequencyList?.Count > 0 &&
						 _frequencyList.First().Value?.First is LinkedListNode<TKey> node => node.Value,

				EvictingDictionaryPolicy.RandomReplacement
					when store.Count > 0 => store.Keys.First(), // (Consider random selection here)

				EvictingDictionaryPolicy.SecondChance when _order is not null =>
					GetSecondChanceCandidate(),

				_ => default
			};

			if (keyToRemove is not null && store.TryGetValue(keyToRemove, out var item))
			{
				ItemEvicting?.Invoke(keyToRemove, item.Value);
				_evictionCount++;
				Remove(keyToRemove); // Cleans up internal structures
				ItemEvicted?.Invoke(keyToRemove, item.Value);
			}
		}

		/// <summary>
		/// Returns an ordered enumeration of key-value pairs based on the current eviction policy and internal tracking state.
		/// </summary>
		/// <remarks>
		/// This method is used primarily for diagnostics, testing, or enumeration purposes, and reflects the internal priority used for
		/// eviction, not insertion order.
		/// </remarks>
		/// <returns>
		/// An <see cref="IEnumerable{T}" /> of <see cref="KeyValuePair{TKey, TValue}" /> in the order determined by the current eviction policy.
		/// </returns>
		/// <exception cref="InvalidOperationException">Thrown if the eviction policy is unrecognized or unsupported for ordering.</exception>
		private IEnumerable<KeyValuePair<TKey, TValue>> GetOrderedItems()
		{
			switch (_evictingPolicy)
			{
				case EvictingDictionaryPolicy.FirstInFirstOut:
				case EvictingDictionaryPolicy.LeastRecentlyUsed:
				case EvictingDictionaryPolicy.SecondChance:
					if (_order is null)
						yield break;

					foreach (var key in _order)
					{
						if (store.TryGetValue(key, out var item))
							yield return new KeyValuePair<TKey, TValue>(key, item.Value);
					}
					break;

				case EvictingDictionaryPolicy.MostRecentlyUsed:
					if (_order is null)
						yield break;

					// MRU: iterate from most recently used (tail) to least (head)
					for (var node = _order.Last; node is not null; node = node.Previous)
					{
						if (store.TryGetValue(node.Value, out var item))
							yield return new KeyValuePair<TKey, TValue>(node.Value, item.Value);
					}
					break;

				case EvictingDictionaryPolicy.LeastFrequentlyUsed:
					if (_frequencyList is null)
						yield break;

					foreach (var freq in _frequencyList.OrderBy(pair => pair.Key))
					{
						foreach (var key in freq.Value)
						{
							if (store.TryGetValue(key, out var item))
								yield return new KeyValuePair<TKey, TValue>(key, item.Value);
						}
					}
					break;

				case EvictingDictionaryPolicy.RandomReplacement:
#if NETSTANDARD2_0
					foreach (var pair in store)
						yield return new KeyValuePair<TKey, TValue>(pair.Key, pair.Value.Value);
#else
					foreach (var (key, item) in store)
						yield return new KeyValuePair<TKey, TValue>(key, item.Value);
#endif
					break;

				default:
					throw new InvalidOperationException($"Unknown eviction _evictingPolicy: {_evictingPolicy}");
			}
		}

		/// <summary>
		/// Finds the next candidate for eviction using the Second-Chance algorithm. Items with their second-chance flag set are moved to
		/// the end of the list and cleared. If no eligible item is found, the oldest item is returned.
		/// </summary>
		/// <returns>The key to evict according to the Second-Chance strategy.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the internal order list is empty.</exception>
		private TKey GetSecondChanceCandidate()
		{
			if (_order is null || _order.Count == 0)
				throw new InvalidOperationException("No eviction candidate available: the _order list is empty.");

			foreach (var key in _order.ToList()) // ToList allows safe modification during iteration
			{
				if (store.TryGetValue(key, out var item))
				{
					if (!item.SecondChance)
						return key;

					// Give second chance: clear flag and cycle to end
					item.SecondChance = false;
					_order.Remove(key);
					_order.AddLast(key);
				}
			}

			if (_order.First is not null)
				return _order.First.Value;

			throw new InvalidOperationException("No eviction candidate found after second chance evaluation.");
		}

		/// <summary>
		/// Removes the specified key from the LeastFrequentlyUsed frequency bucket for the given frequency. Cleans up the bucket if it
		/// becomes empty.
		/// </summary>
		/// <param name="frequency">The current frequency count of the key.</param>
		/// <param name="key">The key to remove.</param>
		private void RemoveFromFrequencyList(int frequency, TKey key)
		{
			if (_frequencyList.TryGetValue(frequency, out var list))
			{
				list.Remove(key);
				if (list.Count == 0)
					_frequencyList.Remove(frequency);
			}
		}

		/// <summary>
		/// Handles internal usage tracking logic based on the current eviction policy.
		/// </summary>
		/// <param name="key">The key that was accessed.</param>
		/// <param name="item">The associated cache item for the key.</param>
		private void TouchInternal(TKey key, CacheItem item)
		{
			switch (_evictingPolicy)
			{
				case EvictingDictionaryPolicy.LeastRecentlyUsed:
				case EvictingDictionaryPolicy.MostRecentlyUsed:
					if (_order is not null)
					{
						if (item.Node is not null)
							_order.Remove(item.Node);
						item.Node = _order.AddLast(key);
					}
					break;

				case EvictingDictionaryPolicy.LeastFrequentlyUsed:
					RemoveFromFrequencyList(item.Frequency, key);
					item.Frequency++;
					AddToFrequencyList(item.Frequency, key);
					break;

				case EvictingDictionaryPolicy.SecondChance:
					item.SecondChance = true;
					break;
			}
		}
	}
}