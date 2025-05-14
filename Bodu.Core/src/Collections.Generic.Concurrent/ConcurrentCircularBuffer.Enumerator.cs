// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="ConcurrentCircularBuffer.Enumerator.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;

namespace Bodu.Collections.Generic.Concurrent
{
	public sealed partial class ConcurrentCircularBuffer<T>
		: System.Collections.Generic.IReadOnlyCollection<T>
	{
		/// <summary>
		/// Provides a snapshot-based enumerator for <see cref="ConcurrentCircularBuffer{T}" /> that supports allocation-free enumeration in
		/// foreach loops.
		/// </summary>
		[Serializable]
		public struct Enumerator
			: System.Collections.Generic.IEnumerator<T>
		{
			private readonly T[] snapshot;
			private int index;
			private T current;

			/// <summary>
			/// Initializes a new instance of the <see cref="Enumerator" /> struct using a snapshot of the buffer's contents at the time of enumeration.
			/// </summary>
			/// <param name="circularBuffer">The <see cref="ConcurrentCircularBuffer{T}" /> instance to enumerate. Must not be <see langword="null" />.</param>
			/// <remarks>
			/// <para>
			/// This constructor captures a snapshot of the buffer's elements via <see cref="ConcurrentCircularBuffer{T}.ToArray" />. It
			/// allows safe iteration even if the buffer is concurrently modified during enumeration.
			/// </para>
			/// <para>The enumerator operates over a static copy and does not reflect subsequent changes made to the buffer.</para>
			/// </remarks>
			/// <exception cref="ArgumentNullException">Thrown if <paramref name="circularBuffer" /> is <see langword="null" />.</exception>
			internal Enumerator(ConcurrentCircularBuffer<T> circularBuffer)
			{
				ThrowHelper.ThrowIfNull(circularBuffer);

				snapshot = circularBuffer.ToArray();
				index = -1;
				current = default!;
			}

			/// <inheritdoc />
			public T Current => current;

			/// <inheritdoc />
			object IEnumerator.Current => current!;

			/// <inheritdoc />
			public bool MoveNext()
			{
				if (++index < snapshot.Length)
				{
					current = snapshot[index];
					return true;
				}

				current = default!;
				return false;
			}

			/// <inheritdoc />
			public void Reset()
			{
				index = -1;
				current = default!;
			}

			/// <inheritdoc />
			public void Dispose()
			{
				// No resources to dispose.
			}
		}
	}
}