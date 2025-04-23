// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ByteBuffer.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Runtime.CompilerServices;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Represents a fixed-capacity byte buffer for sequential writes and zero-copy retrieval.
	/// </summary>
	internal sealed class ByteBuffer
	{
		private const int EmptyIndex = -1;
		private readonly byte[] buffer;
		private int index;

		/// <summary>
		/// Initializes a new instance of the <see cref="ByteBuffer" /> class that is empty and has
		/// the specified capacity.
		/// </summary>
		/// <param name="capacity">The number of bytes that the new buffer can initially store.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="capacity" /> is less than 0.
		/// </exception>
		public ByteBuffer(int capacity)
		{
			ThrowHelper.ThrowIfLessThan(capacity, 0);
			this.buffer = new byte[capacity];
			this.Initialize();
		}

		/// <summary>
		/// Gets a value indicating whether the buffer is currently empty.
		/// </summary>
		public bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return this.index == EmptyIndex; }
		}

		/// <summary>
		/// Gets a value indicating whether the buffer is full and no more bytes can be added.
		/// </summary>
		public bool IsFull
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return this.Count == this.buffer.Length; }
		}

		/// <summary>
		/// Gets the maximum number of bytes that the buffer can contain.
		/// </summary>
		public int Capacity => this.buffer.Length;

		/// <summary>
		/// Gets the number of bytes currently written to the buffer.
		/// </summary>
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return this.index + 1; }
		}

		/// <summary>
		/// Copies a range of bytes from a one-dimensional array into the buffer.
		/// </summary>
		/// <param name="array">The source array that contains the bytes to copy.</param>
		/// <param name="index">
		/// The zero-based index in <paramref name="array" /> at which copying begins.
		/// </param>
		/// <param name="count">The number of bytes to copy.</param>
		/// <returns>
		/// <see langword="true" /> if the buffer is full after the operation; otherwise, <see langword="false" />.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index" /> or <paramref name="count" /> is less than 0.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="index" /> and <paramref name="count" /> specify an invalid range in
		/// <paramref name="array" />. <br />
		/// -or- <br /><paramref name="count" /> exceeds the remaining capacity of the buffer.
		/// </exception>
		public bool Add(byte[] array, int index, int count)
		{
			EnsureAddIsValid(array, index, count);
			Buffer.BlockCopy(array, index, this.buffer, this.Count, count);
			this.index += count;
			return this.IsFull;
		}

		/// <summary>
		/// Adds bytes from a read-only span to the buffer without allocating or copying the input.
		/// </summary>
		/// <param name="span">The source span of bytes to copy.</param>
		/// <returns>
		/// <see langword="true" /> if the buffer is full after the operation; otherwise, <see langword="false" />.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// The input span exceeds the remaining capacity of the buffer.
		/// </exception>
		public bool Add(ReadOnlySpan<byte> span)
		{
			int currentCount = this.Count;
			ThrowHelper.ThrowIfGreaterThan(span.Length, this.buffer.Length - currentCount, nameof(span));
			span.CopyTo(new Span<byte>(this.buffer, currentCount, span.Length));
			this.index += span.Length;
			return this.IsFull;
		}

		/// <summary>
		/// Returns the contents of the buffer as an array of bytes.
		/// </summary>
		/// <returns>An array of bytes representing the full buffer contents.</returns>
		/// <exception cref="InvalidOperationException">The buffer is not yet full.</exception>
		public byte[] GetBytes()
		{
			if (!this.IsFull)
				throw new InvalidOperationException(ResourceStrings.InvalidOperation_BufferNotFull);

			this.index = EmptyIndex;
			return this.buffer;
		}

		/// <summary>
		/// Returns the contents of the buffer as an array of bytes, zero-padding the unused portion.
		/// </summary>
		/// <returns>An array of bytes representing the buffer, with remaining capacity cleared.</returns>
		public byte[] GetBytesZeroPadded()
		{
			int count = this.Count;
			Array.Clear(this.buffer, count, this.buffer.Length - count);
			this.index = EmptyIndex;
			return this.buffer;
		}

		/// <summary>
		/// Resets the buffer by optionally clearing its contents and marking it as empty.
		/// </summary>
		/// <param name="clear">
		/// If <see langword="true" />, the buffer contents will be cleared. Otherwise, only the
		/// state is reset.
		/// </param>
		public void Initialize(bool clear = true)
		{
			if (clear)
				Array.Clear(this.buffer, 0, this.buffer.Length);

			this.index = EmptyIndex;
		}

		/// <summary>
		/// Validates parameters before performing a write to the buffer.
		/// </summary>
		/// <param name="array">The array to copy from.</param>
		/// <param name="index">The start index in the array.</param>
		/// <param name="count">The number of bytes to copy.</param>
		/// <exception cref="ArgumentNullException"><paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index" /> or <paramref name="count" /> is less than 0.
		/// </exception>
		/// <exception cref="ArgumentException">NextWhile or capacity validation fails.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureAddIsValid(byte[] array, int index, int count)
		{
			ThrowHelper.ThrowIfNull(array);
			ThrowHelper.ThrowIfLessThan(index, 0);
			ThrowHelper.ThrowIfLessThan(count, 0);
			ThrowHelper.ThrowIfGreaterThan(count, array.Length - index, nameof(count));
			ThrowHelper.ThrowIfGreaterThan(this.Count + count, this.buffer.Length, nameof(count));
		}
	}
}