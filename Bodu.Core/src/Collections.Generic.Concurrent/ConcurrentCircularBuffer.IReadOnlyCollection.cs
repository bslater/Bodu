// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="ConcurrentCircularBuffer.IReadOnlyCollection.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Bodu.Collections.Generic.Concurrent
{
	public sealed partial class ConcurrentCircularBuffer<T>
		: System.Collections.Generic.IReadOnlyCollection<T>
	{
		/// <summary>
		/// Gets the number of elements currently contained in the <see cref="ConcurrentCircularBuffer{T}" />.
		/// </summary>
		/// <value>The number of elements in the buffer. This value is always less than or equal to <see cref="Capacity" />.</value>
		/// <remarks>
		/// <para>
		/// The value is retrieved using a thread-safe read operation via <see cref="!:Volatile.Read" />, ensuring accurate and consistent
		/// visibility across threads.
		/// </para>
		/// <para>This property may change immediately after being read if other threads modify the buffer concurrently.</para>
		/// </remarks>
		public int Count
		{
			get
			{
				int head = Volatile.Read(ref this.head);
				int tail = Volatile.Read(ref this.tail);
				int capacity = Volatile.Read(ref this.capacity);
				return (tail - head + capacity) % capacity;
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates over a snapshot of the buffer’s contents.
		/// </summary>
		/// <returns>An <see cref="Enumerator" /> that iterates through the buffer in the order from oldest to newest.</returns>
		/// <remarks>
		/// <para>
		/// The enumerator operates over a one-time snapshot of the buffer taken at the moment of enumeration. This ensures that enumeration
		/// is safe and consistent even if the buffer is modified concurrently by other threads.
		/// </para>
		/// <para>Modifications to the buffer after the enumerator is created will not be reflected in the enumerated results.</para>
		/// </remarks>
		public Enumerator GetEnumerator() => new(this);

		/// <inheritdoc />
		IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);
	}
}