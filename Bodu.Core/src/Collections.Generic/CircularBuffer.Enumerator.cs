// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="CircularBuffer.Enumerator.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Collections.Generic
{
	public partial class CircularBuffer<T>
	{
		/// <summary>
		/// Enumerates the elements of a <see cref="CircularBuffer{T}" />.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Use the <see langword="foreach" /> statement to simplify the enumeration process instead
		/// of directly using this enumerator.
		/// </para>
		/// <para>
		/// The enumerator provides read-only access to the collection's elements. Modifying the
		/// underlying collection while enumerating invalidates the enumerator.
		/// </para>
		/// </remarks>
		[Serializable]
		public struct Enumerator
			: System.Collections.Generic.IEnumerator<T>
		{
			private readonly CircularBuffer<T> parent;
			private readonly int version;
			private int currentIndex;
			private T current;
			private int iteratedCount;

			/// <summary>
			/// Initializes a new instance of the <see cref="Enumerator" /> struct.
			/// </summary>
			/// <param name="parent">The buffer to enumerate.</param>
			internal Enumerator(CircularBuffer<T> parent)
			{
				this.parent = parent;
				this.version = parent.version;
				this.currentIndex = parent.head - 1; // start before the first element
				this.current = default!;
				this.iteratedCount = 0;
			}

			/// <inheritdoc />
			public T Current => current;

			/// <inheritdoc />
			object System.Collections.IEnumerator.Current => this.current!;

			/// <inheritdoc />
			public bool MoveNext()
			{
				// Detect changes made to the buffer during enumeration
				if (version != parent.version)
					throw new InvalidOperationException(ResourceStrings.InvalidOperation_CollectionModified);

				if (iteratedCount >= parent.Count)
				{
					// Reached the end of the collection
					current = default!;
					return false;
				}

				// Efficiently increment index without repeated modulo unless necessary
				currentIndex = (currentIndex + 1) % parent.array.Length;
				current = parent.array[currentIndex];
				iteratedCount++;

				return true;
			}

			/// <inheritdoc />
			public void Reset()
			{
				if (version != parent.version)
					throw new InvalidOperationException(ResourceStrings.InvalidOperation_CollectionModified);

				currentIndex = parent.head - 1;
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