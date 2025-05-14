// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="EvictingDictionaryDebugView.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bodu.Collections.Generic
{
	/// <summary>
	/// Internal debug view that shows key-value pairs directly in the debugger.
	/// </summary>
	internal sealed class EvictingDictionaryDebugView<TKey, TValue>
		where TKey : notnull, IComparable<TKey>
	{
		private readonly EvictingDictionary<TKey, TValue> dictionary;

		public EvictingDictionaryDebugView(EvictingDictionary<TKey, TValue> dictionary)
		{
			ThrowHelper.ThrowIfNull(dictionary);

			this.dictionary = dictionary;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public KeyValuePair<TKey, TValue>[] Items => dictionary.ToArray();
	}
}