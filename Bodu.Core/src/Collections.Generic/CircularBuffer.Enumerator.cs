// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="CircularBuffer.Enumerator.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

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
				this.version = circularBuffer.version;
				this.currentIndex = -1;
				this.current = default!;
				this.iteratedCount = 0;
			}

			/// <inheritdoc />
			public T Current =>
				this.currentIndex == -1
					? throw new InvalidOperationException(ResourceStrings.InvalidOperation_EnumeratorNotOnElement)
					: this.current;

			/// <inheritdoc />
			object System.Collections.IEnumerator.Current => this.Current!;

			/// <inheritdoc />
			public bool MoveNext()
			{
				if (this.version != this.circularBuffer.version)
					throw new InvalidOperationException(ResourceStrings.InvalidOperation_CollectionModified);

				if (this.iteratedCount >= this.circularBuffer.count)
				{
					this.current = default!;
					this.currentIndex = -1; // Ended
					return false;
				}

				this.currentIndex = (this.circularBuffer.head + this.iteratedCount) % this.circularBuffer.capacity;
				this.current = this.circularBuffer.array[this.currentIndex];
				this.iteratedCount++;

				return true;
			}

			/// <inheritdoc />
			public void Reset()
			{
				if (this.version != this.circularBuffer.version)
					throw new InvalidOperationException(ResourceStrings.InvalidOperation_CollectionModified);

				this.currentIndex = -1;
				this.current = default!;
				this.iteratedCount = 0;
			}

			/// <inheritdoc />
			public void Dispose()
			{
				// No unmanaged resources; method provided for interface completeness.
			}
		}
	}
}