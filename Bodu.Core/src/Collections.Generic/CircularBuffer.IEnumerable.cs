// ---------------------------------------------------------------------------------------------------------------
// <copyright file="CircularBuffer.IEnumerable.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Collections;

namespace Bodu.Collections.Generic
{
	public partial class CircularBuffer<T>
		: System.Collections.Generic.IEnumerable<T>
	{
		/// <summary>
		/// Returns an enumerator that iterates through the <see cref="CircularBuffer{T}" />.
		/// </summary>
		/// <returns>An <see cref="Enumerator" /> that can be used to iterate through the buffer.</returns>
		/// <remarks>
		/// <para>
		/// The <see langword="foreach" /> statement in C# ( <c>For Each</c> in Visual Basic) hides
		/// the complexity of using enumerators. Therefore, using <see langword="foreach" /> is
		/// recommended over directly manipulating the enumerator.
		/// </para>
		/// <para>
		/// The enumerator provides read-only access to the collection. It cannot be used to modify
		/// the underlying buffer.
		/// </para>
		/// <para>
		/// Initially, the enumerator is positioned before the first element. At this position,
		/// <see cref="Enumerator.Current" /> is undefined. You must call
		/// <see cref="Enumerator.MoveNext" /> to advance to the first element before reading <see cref="Enumerator.Current" />.
		/// </para>
		/// <para>
		/// <see cref="Enumerator.Current" /> returns the same value until
		/// <see cref="Enumerator.MoveNext" /> is called. Each call to
		/// <see cref="Enumerator.MoveNext" /> advances the enumerator to the next element.
		/// </para>
		/// <para>
		/// If <see cref="Enumerator.MoveNext" /> passes the end of the buffer, the enumerator is
		/// positioned after the last element and returns <see langword="false" />. At this point,
		/// <see cref="Enumerator.Current" /> is undefined, and you must create a new enumerator
		/// instance to iterate again.
		/// </para>
		/// <para>
		/// The enumerator is invalidated if the buffer is modified. Subsequent calls to
		/// <see cref="Enumerator.MoveNext" /> or
		/// <see cref="System.Collections.IEnumerator.Reset" /> will throw an <see cref="System.InvalidOperationException" />.
		/// </para>
		/// <para>
		/// Enumeration is not thread-safe. To guarantee thread safety, lock the buffer during
		/// enumeration. <see cref="CircularBuffer{T}" /> is not synchronized by default.
		/// </para>
		/// </remarks>
		public Enumerator GetEnumerator()
			=> new CircularBuffer<T>.Enumerator(this);

		/// <inheritdoc />
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
			=> new Enumerator(this);

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
			=> new Enumerator(this);
	}
}