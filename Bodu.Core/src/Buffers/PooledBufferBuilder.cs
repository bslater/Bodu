// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="PooledBufferBuilder.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
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
		private int _count;
		private bool _disposed;
		private T[] _internalBuffer;

		/// <summary>
		/// Initializes a new instance of the <see cref="PooledBufferBuilder{T}" /> class.
		/// </summary>
		/// <param name="initialCapacity">Optional initial buffer size.</param>
		public PooledBufferBuilder(int initialCapacity = 256)
		{
			_internalBuffer = ArrayPool<T>.Shared.Rent(initialCapacity);
			_count = 0;
		}

		/// <summary>
		/// The number of valid elements in the buffer.
		/// </summary>
		public int Count
		{
			get
			{
				ThrowIfDisposed();
				return _count;
			}
		}

		/// <summary>
		/// Appends a single item to the buffer, expanding it if necessary.
		/// </summary>
		/// <param name="item">The item to append.</param>
		public void Append(T item)
		{
			ThrowIfDisposed();

			if (_count >= _internalBuffer.Length)
				Grow();

			_internalBuffer[_count++] = item;
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
				if (_count >= _internalBuffer.Length)
					Grow();

				_internalBuffer[_count++] = item;
			}
		}

		/// <summary>
		/// Returns the buffered elements as a span of valid items.
		/// </summary>
		public T[] AsArray()
		{
			ThrowIfDisposed();
			return _internalBuffer;
		}

		/// <summary>
		/// Returns the buffered elements as a span of valid items.
		/// </summary>
		public Span<T> AsSpan()
		{
			ThrowIfDisposed();
			return _internalBuffer.AsSpan(0, _count);
		}

		/// <summary>
		/// Returns the underlying buffer to the pool and resets internal state.
		/// </summary>
		public void Dispose()
		{
			if (!_disposed)
			{
				ReturnBufferIfNeeded();
				_internalBuffer = Array.Empty<T>();
				_count = 0;
				_disposed = true;
			}
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
				_count = col.Count;
				_internalBuffer = ArrayPool<T>.Shared.Rent(_count);
				col.CopyTo(_internalBuffer, 0);
				return true;
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Grow()
		{
			T[] newBuffer = ArrayPool<T>.Shared.Rent(_internalBuffer.Length * 2);
			Array.Copy(_internalBuffer, 0, newBuffer, 0, _count);
			ReturnBufferIfNeeded();
			_internalBuffer = newBuffer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ReturnBufferIfNeeded()
		{
			if (_internalBuffer.Length > 0)
			{
				bool clear = RuntimeHelpers.IsReferenceOrContainsReferences<T>();
				ArrayPool<T>.Shared.Return(_internalBuffer, clear);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfDisposed()
		{
			if (_disposed)
				throw new ObjectDisposedException(nameof(PooledBufferBuilder<T>), "Cannot access _internalBuffer after it has been _disposed.");
		}
	}
}