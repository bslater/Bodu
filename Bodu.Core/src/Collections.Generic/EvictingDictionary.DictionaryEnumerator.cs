// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="EvictingDictionary.DictionaryEnumerator.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System.Collections;

namespace Bodu.Collections.Generic
{
	public partial class EvictingDictionary<TKey, TValue>
	{
		/// <summary>
		/// Enumerates the elements of a <see cref="EvictingDictionary{TKey, TValue}" />.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Use the <see langword="foreach" /> statement to simplify the enumeration process instead
		/// of directly using this enumerator.
		/// </para>
		/// <para>
		/// The enumerator provides read-only access to the dictionary's elements. Modifying the
		/// underlying dictionary while enumerating invalidates the enumerator.
		/// </para>
		/// </remarks>
		public struct DictionaryEnumerator : IDictionaryEnumerator
		{
			private readonly EvictingDictionary<TKey, TValue> dictionary;
			private IEnumerator<KeyValuePair<TKey, TValue>> inner;

			/// <summary>
			/// Initializes a new instance of the <see cref="DictionaryEnumerator" /> struct.
			/// </summary>
			/// <param name="dictionary">The dictionary to enumerate.</param>
			public DictionaryEnumerator(EvictingDictionary<TKey, TValue> dictionary)
			{
				this.dictionary = dictionary;
				this.inner = dictionary.GetEnumerator(); // Assume internal method to access the entry sequence
			}

			/// <inheritdoc />
			public DictionaryEntry Entry => new(inner.Current.Key!, inner.Current.Value);

			/// <inheritdoc />
			public object Key => inner.Current.Key!;

			/// <inheritdoc />
			public object? Value => inner.Current.Value;

			/// <inheritdoc />
			public object Current => Entry;

			/// <inheritdoc />
			public bool MoveNext() => inner.MoveNext();

			/// <inheritdoc />
			public void Reset()
			{
				inner.Dispose();
				inner = dictionary.GetEnumerator(); // Refreshes the enumerator from current state
			}
		}
	}
}