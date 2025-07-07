// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="IEnumerableExtensions.Batch.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

#if !NETSTANDARD2_0

using Bodu.Buffers;

#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Bodu.Collections.Generic.Extensions
{
	public static partial class IEnumerableExtensions
	{
		/// <summary>
		/// Batches the source sequence into subsequences of the specified size.
		/// </summary>
		/// <typeparam name="TSource">The type of elements in the source sequence.</typeparam>
		/// <param name="source">The source sequence to batch.</param>
		/// <param name="size">The size of each batch. Must be greater than 0.</param>
		/// <returns>
		/// An <see cref="IEnumerable{T}" /> where each inner <see cref="IEnumerable{T}" /> contains up to <paramref name="size" /> elements
		/// from the source sequence. The final batch may contain fewer than <paramref name="size" /> elements.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="source" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="size" /> is less than or equal to 0.</exception>
		/// <remarks>
		/// This method uses deferred execution. Enumeration of the source sequence and batches occurs only when the result is enumerated.
		/// </remarks>
		public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> source, int size) =>
			source.Batch(size, (item, _) => item);

		/// <summary>
		/// Projects each element of a sequence and batches the transformed elements into subsequences of the specified size.
		/// </summary>
		/// <typeparam name="TSource">The type of elements in the source sequence.</typeparam>
		/// <typeparam name="TResult">The type of result elements.</typeparam>
		/// <param name="source">The source sequence to batch.</param>
		/// <param name="size">The size of each batch. Must be greater than 0.</param>
		/// <param name="selector">A projection function to apply to each element.</param>
		/// <returns>
		/// An <see cref="IEnumerable{T}" /> where each inner <see cref="IEnumerable{T}" /> contains up to <paramref name="size" />
		/// transformed elements.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="size" /> is less than or equal to 0.</exception>
		/// <remarks>This method uses deferred execution. The transformation and batching occur only during enumeration.</remarks>
		public static IEnumerable<IEnumerable<TResult>> Batch<TSource, TResult>(this IEnumerable<TSource> source, int size, Func<TSource, TResult> selector)
		{
			ThrowHelper.ThrowIfNull(selector);
			return source.Batch(size, (item, _) => selector(item));
		}

		/// <summary>
		/// Projects each element of a sequence into a new form using its index, and batches the transformed elements into subsequences of
		/// the specified size.
		/// </summary>
		/// <typeparam name="TSource">The type of elements in the source sequence.</typeparam>
		/// <typeparam name="TResult">The type of result elements.</typeparam>
		/// <param name="source">The source sequence to batch.</param>
		/// <param name="size">The size of each batch. Must be greater than 0.</param>
		/// <param name="selector">A projection function that receives the item and its index.</param>
		/// <returns>
		/// An <see cref="IEnumerable{T}" /> where each inner <see cref="IEnumerable{T}" /> contains up to <paramref name="size" />
		/// transformed elements.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="size" /> is less than or equal to 0.</exception>
		/// <remarks>This method uses deferred execution. The projection and batching occur only during enumeration.</remarks>
		public static IEnumerable<IEnumerable<TResult>> Batch<TSource, TResult>(
			this IEnumerable<TSource> source,
			int size,
			Func<TSource, int, TResult> selector)
		{
			ThrowHelper.ThrowIfNull(source);
			ThrowHelper.ThrowIfNull(selector);
			ThrowHelper.ThrowIfOutOfRange(size, 1, int.MaxValue);

			// Use a local iterator to enable deferred execution. This ensures the source is not evaluated until enumeration begins.
			return BatchIterator();

			IEnumerable<IEnumerable<TResult>> BatchIterator()
			{
				// Create an enumerator to read the source sequence lazily
				using var enumerator = source.GetEnumerator();

				int index = 0; // Tracks the global index across all batches

				// Continue looping while source has elements
				while (enumerator.MoveNext())
				{
					// Allocate a new array for the current batch. We use a fixed-size array here for performance since we know the batch size.
					TResult[] batch = new TResult[size];
					int count = 0;

					// Inner loop: fill up the batch up to the specified size
					do
					{
						// Project the current source element using the selector, passing the current index
						batch[count++] = selector(enumerator.Current, index++);
					} while (count < size && enumerator.MoveNext());

					// If the final batch is not full (i.e., count < size), resize the array to avoid yielding unused slots. This preserves
					// accurate memory usage and avoids exposing default(T) values.
					if (count < size)
						Array.Resize(ref batch, count);

					// Yield the completed batch
					yield return batch;
				}
			}
		}

#if !NETSTANDARD2_0

		/// <summary>
		/// Batches and transforms a sequence using a pooled array to reduce allocations.
		/// </summary>
		/// <typeparam name="TSource">The type of elements in the source sequence.</typeparam>
		/// <typeparam name="TResult">The type of transformed result.</typeparam>
		/// <param name="source">The source sequence to batch.</param>
		/// <param name="size">The maximum number of items per batch.</param>
		/// <param name="selector">A projection function that receives the source item and its index.</param>
		/// <returns>
		/// An <see cref="IEnumerable{T}" /> of <see cref="ReadOnlyMemory{TResult}" /> batches, reusing memory via a pooled buffer.
		/// </returns>
		/// <remarks>
		/// <para>
		/// Each batch shares memory from a pooled buffer that is reused across iterations to reduce allocations.
		/// If you need to retain a batch after enumeration, make a defensive copy using <c>.ToArray()</c>.
		/// </para>
		/// <para>
		/// This method should be consumed via <c>foreach</c>. Repeated enumeration creates new pooled buffers per call.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code language="csharp">
		/// <![CDATA[
		/// Input:  Enumerable.Range(1, 10)
		/// Batch size: 4
		/// Selector: (x, i) => $"Item {i}: {x}"
		///
		/// Expected output:
		/// Batch 1: "Item 0: 1", "Item 1: 2", "Item 2: 3", "Item 3: 4"
		/// Batch 2: "Item 4: 5", "Item 5: 6", "Item 6: 7", "Item 7: 8"
		/// Batch 3: "Item 8: 9", "Item 9: 10"
		/// var source = Enumerable.Range(1, 10);
		/// foreach (var batch in source.BatchPooled(4, (x, i) => $"Item {i}: {x}"))
		/// {
		///     Console.WriteLine($"[{string.Join(", ", batch.ToArray())}]");
		/// }
		///]]>
		/// </code>
		/// </example>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="size" /> is less than or equal to 0.</exception>
		public static IEnumerable<ReadOnlyMemory<TResult>> BatchPooled<TSource, TResult>(
				this IEnumerable<TSource> source,
				int size,
				Func<TSource, int, TResult> selector)
		{
			ThrowHelper.ThrowIfNull(source);
			ThrowHelper.ThrowIfNull(selector);
			ThrowHelper.ThrowIfOutOfRange(size, 1, int.MaxValue);

			return BatchIterator();

#if NET5_0_OR_GREATER
			IEnumerable<ReadOnlyMemory<TResult>> BatchIterator()
			{
				using var enumerator = source.GetEnumerator();
				var buffer = new PooledBufferBuilder<TResult>(size);

				int index = 0;

				try
				{
					while (enumerator.MoveNext())
					{
						buffer.Append(selector(enumerator.Current, index++));

						if (buffer.Count == size)
						{
							yield return buffer.AsSpan().ToArray(); // defensive copy
							buffer.Dispose();
							buffer = new PooledBufferBuilder<TResult>(size);
						}
					}

					if (buffer.Count > 0)
						yield return buffer.AsSpan().ToArray();
				}
				finally
				{
					buffer.Dispose();
				}
			}
#elif NETSTANDARD2_1
			IEnumerable<ReadOnlyMemory<TResult>> BatchIterator()
			{
				using var enumerator = source.GetEnumerator();
				var buffer = new PooledBufferBuilder<TResult>(size);
				int index = 0;

				try
				{
					while (enumerator.MoveNext())
					{
						buffer.Append(selector(enumerator.Current, index++));

						if (buffer.Count == size)
						{
							yield return buffer.AsSpan().ToArray(); // defensive copy
							buffer.Dispose();
							buffer = new PooledBufferBuilder<TResult>(size);
						}
					}

					if (buffer.Count > 0)
						yield return buffer.AsSpan().ToArray();
				}
				finally
				{
					buffer.Dispose();
				}
			}
#else
#error BatchPooled is only supported on netstandard2.1 or greater
#endif
		}

		/// <summary>
		/// Batches a sequence using a pooled array to reduce allocations.
		/// </summary>
		/// <typeparam name="TSource">The type of elements in the source sequence.</typeparam>
		/// <param name="source">The source sequence to batch.</param>
		/// <param name="size">The maximum number of items per batch.</param>
		/// <returns>
		/// An <see cref="IEnumerable{T}" /> of <see cref="ReadOnlyMemory{TSource}" /> batches.
		/// </returns>
		/// <remarks>
		/// This overload returns untransformed batches of the original type.
		/// </remarks>
		/// <example>
		/// <code language="csharp">
		/// // Input:  Enumerable.Range(1, 9)
		/// Batch size: 3
		///
		/// Expected output:
		/// Batch 1: 1, 2, 3
		/// Batch 2: 4, 5, 6
		/// Batch 3: 7, 8, 9
		/// var source = Enumerable.Range(1, 9);
		/// foreach (var batch in source.BatchPooled(3))
		/// {
		///     Console.WriteLine(string.Join(", ", batch.ToArray()));
		/// }
		/// </code>
		/// </example>
		public static IEnumerable<ReadOnlyMemory<TSource>> BatchPooled<TSource>(
				this IEnumerable<TSource> source,
				int size)
		{
			return source.BatchPooled(size, static (x, _) => x);
		}
	}
}

#endif