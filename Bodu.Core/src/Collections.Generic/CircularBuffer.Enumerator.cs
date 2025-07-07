// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="CircularBuffer.Enumerator.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Collections.Generic
{
	public partial class CircularBuffer<T>
	{
		/// <summary>
		/// Enumerates the elements of a <see cref="CircularBuffer{T}" />.
		/// </summary>
		/// <remarks>
		/// <para>Use the <see langword="foreach" /> statement to simplify the enumeration process instead of directly using this enumerator.</para>
		/// <para>
		/// The enumerator provides read-only access to the collection's elements. Modifying the underlying collection while enumerating
		/// invalidates the enumerator.
		/// </para>
		/// </remarks>
		[Serializable]
		public struct Enumerator
			: System.Collections.Generic.IEnumerator<T>
		{
			private readonly CircularBuffer<T> circularBuffer;
			private readonly int version;
			private int currentIndex;
			private T current;
			private int iteratedCount;

			/// <summary>
			/// Initializes a new instance of the <see cref="Enumerator" /> struct.
			/// </summary>
			/// <param name="circularBuffer">The buffer to enumerate.</param>
			internal Enumerator(CircularBuffer<T> circularBuffer)
			{
				this.circularBuffer = circularBuffer;
				version = circularBuffer._version;
				currentIndex = -1;
				current = default!;
				iteratedCount = 0;
			}

			/// <inheritdoc />
			public T Current =>
				currentIndex == -1
					? throw new InvalidOperationException(ResourceStrings.InvalidOperation_EnumeratorNotOnElement)
					: current;

			/// <inheritdoc />
			object System.Collections.IEnumerator.Current => Current!;

			/// <inheritdoc />
			public bool MoveNext()
			{
				if (version != circularBuffer._version)
					throw new InvalidOperationException(ResourceStrings.InvalidOperation_CollectionModified);

				if (iteratedCount >= circularBuffer._count)
				{
					current = default!;
					currentIndex = -1; // Ended
					return false;
				}

				currentIndex = (circularBuffer._head + iteratedCount) % circularBuffer._capacity;
				current = circularBuffer._internalBuffer[currentIndex];
				iteratedCount++;

				return true;
			}

			/// <inheritdoc />
			public void Reset()
			{
				if (version != circularBuffer._version)
					throw new InvalidOperationException(ResourceStrings.InvalidOperation_CollectionModified);

				currentIndex = -1;
				current = default!;
				iteratedCount = 0;
			}

			/// <inheritdoc />
			public void Dispose()
			{
				// No unmanaged resources; method provided for interface completeness.
			}
		}
	}
}