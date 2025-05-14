// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="CircularBuffer.IEnumerable.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace Bodu.Collections.Generic
{
	public partial class CircularBuffer<T>
		: System.Collections.Generic.IEnumerable<T>
	{
		/// <summary>
		/// Returns an enumerator that iterates over a snapshot of the buffer’s contents.
		/// </summary>
		/// <returns>An enumerator that provides forward-only, read-only access to the buffer's contents.</returns>
		/// <remarks>
		/// <para>
		/// This method captures a snapshot of the buffer using <see cref="ToArray" />. The enumerator operates over that snapshot, ensuring
		/// that enumeration is not affected by modifications made to the buffer after enumeration begins.
		/// </para>
		/// <para>For performance and safety, the enumerator should be preferred over manual index iteration in concurrent or volatile scenarios.</para>
		/// <para><b>Note:</b> This type is not thread-safe. If thread safety is required, consider using <see cref="Bodu.Collections.Generic.Concurrent.ConcurrentCircularBuffer{T}" />.</para>
		/// </remarks>
		public Enumerator GetEnumerator()
			=> new(this);

		/// <inheritdoc />
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
			=> new Enumerator(this);

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
			=> new Enumerator(this);
	}
}