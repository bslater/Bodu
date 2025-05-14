// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="PooledBufferBuilder.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bodu.Buffers
{
	/// <summary>
	/// Provides an efficient way to copy items from a sequence into a pooled buffer, automatically resizing as needed and supporting
	/// collection fast-paths.
	/// </summary>
	/// <typeparam name="T">The type of item.</typeparam>
	public sealed class PooledBufferBuilder<T>
	   : System.IDisposable
	{
		private T[] buffer;
		private int count;
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="PooledBufferBuilder{T}" /> class.
		/// </summary>
		/// <param name="initialCapacity">Optional initial buffer size.</param>
		public PooledBufferBuilder(int initialCapacity = 256)
		{
			buffer = ArrayPool<T>.Shared.Rent(initialCapacity);
			count = 0;
		}

		/// <summary>
		/// Attempts to fast-copy the source if it supports <see cref="ICollection{T}" />.
		/// </summary>
		/// <param name="source">The input sequence.</param>
		/// <returns><c>true</c> if copied using <c>CopyTo</c> otherwise, use <see cref="AppendRange" />.</returns>
		public bool TryCopyFrom(IReadOnlyCollection<T> source)
		{
			ThrowIfDisposed();
			ThrowHelper.ThrowIfNull(source);

			if (source is ICollection<T> col)
			{
				ReturnBufferIfNeeded();
				count = col.Count;
				buffer = ArrayPool<T>.Shared.Rent(count);
				col.CopyTo(buffer, 0);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Appends a single item to the buffer, expanding it if necessary.
		/// </summary>
		/// <param name="item">The item to append.</param>
		public void Append(T item)
		{
			ThrowIfDisposed();

			if (count >= buffer.Length)
				Grow();

			buffer[count++] = item;
		}

		/// <summary>
		/// Appends items from an <see cref="IEnumerable{T}" />, expanding the pooled buffer if needed.
		/// </summary>
		/// <param name="source">The input sequence.</param>
		public void AppendRange(IEnumerable<T> source)
		{
			ThrowIfDisposed();
			ThrowHelper.ThrowIfNull(source);

			foreach (T item in source)
			{
				if (count >= buffer.Length)
					Grow();

				buffer[count++] = item;
			}
		}

		/// <summary>
		/// Returns the buffered elements as a span of valid items.
		/// </summary>
		public Span<T> AsSpan()
		{
			ThrowIfDisposed();
			return buffer.AsSpan(0, count);
		}

		/// <summary>
		/// Returns the buffered elements as a span of valid items.
		/// </summary>
		public T[] AsArray()
		{
			ThrowIfDisposed();
			return buffer;
		}

		/// <summary>
		/// The number of valid elements in the buffer.
		/// </summary>
		public int Count
		{
			get
			{
				ThrowIfDisposed();
				return count;
			}
		}

		/// <summary>
		/// Returns the underlying buffer to the pool and resets internal state.
		/// </summary>
		public void Dispose()
		{
			if (!disposed)
			{
				ReturnBufferIfNeeded();
				buffer = Array.Empty<T>();
				count = 0;
				disposed = true;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfDisposed()
		{
			if (disposed)
				throw new ObjectDisposedException(nameof(PooledBufferBuilder<T>), "Cannot access buffer after it has been disposed.");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Grow()
		{
			T[] newBuffer = ArrayPool<T>.Shared.Rent(buffer.Length * 2);
			Array.Copy(buffer, 0, newBuffer, 0, count);
			ReturnBufferIfNeeded();
			buffer = newBuffer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ReturnBufferIfNeeded()
		{
			if (buffer.Length > 0)
			{
				bool clear = RuntimeHelpers.IsReferenceOrContainsReferences<T>();
				ArrayPool<T>.Shared.Return(buffer, clear);
			}
		}
	}
}