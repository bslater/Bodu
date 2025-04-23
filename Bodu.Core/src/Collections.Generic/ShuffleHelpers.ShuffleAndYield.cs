using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Bodu.Collections.Extensions;

namespace Bodu.Collections.Generic
{
	public static partial class ShuffleHelpers
	{
		/// <summary>
		/// Yields a randomized subset of an enumerable source using an in-place shuffle of a temporary buffer.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="source">The source enumerable.</param>
		/// <param name="rng">The random number generator.</param>
		/// <param name="count">The number of elements to yield.</param>
		/// <returns>A randomized sequence of elements from the source.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="source" /> or <paramref name="rng" /> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="count" /> is negative or exceeds the number of available elements.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> ShuffleAndYield<T>(IEnumerable<T> source, IRandomGenerator rng, int count)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			if (rng == null)
				throw new ArgumentNullException(nameof(rng));

			T[] buffer = source.ToArray();
			return ShuffleAndYield(buffer, rng, count); // calls array overload
		}

		/// <summary>
		/// Yields a randomized subset of the given array using an in-place partial Fisher–Yates shuffle.
		/// </summary>
		/// <typeparam name="T">The type of element.</typeparam>
		/// <param name="array">The array to shuffle.</param>
		/// <param name="rng">The random number generator to use.</param>
		/// <param name="count">The number of elements to yield.</param>
		/// <returns>A randomized sequence of elements.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count" /> is less than 0 or exceeds the array length.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> ShuffleAndYield<T>(T[] array, IRandomGenerator rng, int count)
		{
			int available = array.Length;
			T[] buffer = array.ToArray(); // copy the buffer

			if (count < 0 || count > available)
				throw new ArgumentOutOfRangeException(nameof(count), "Count exceeds available items in array.");

			while (count-- > 0)
			{
				int i = rng.Next(available);
				yield return buffer[i];
				buffer[i] = buffer[--available];
			}
		}

		/// <summary>
		/// Yields a randomized subset of a span using in-place shuffle logic.
		/// </summary>
		/// <typeparam name="T">The type of element.</typeparam>
		/// <param name="span">The span to shuffle.</param>
		/// <param name="rng">The random number generator to use.</param>
		/// <param name="count">The number of items to return.</param>
		/// <returns>A randomized sequence of elements from the span.</returns>
		/// <remarks>This method shuffles the span in-place and yields the first <paramref name="count" /> results.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> ShuffleAndYield<T>(ReadOnlySpan<T> span, IRandomGenerator rng, int count)
			=> ShuffleAndYield(span.ToArray(), rng, count);

		/// <summary>
		/// Yields a randomized subset of a memory array using in-place shuffle logic.
		/// </summary>
		/// <typeparam name="T">The type of element.</typeparam>
		/// <param name="memory">The memory to shuffle.</param>
		/// <param name="rng">The random number generator.</param>
		/// <param name="count">The number of items to yield.</param>
		/// <returns>A randomized sequence of elements from memory.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> ShuffleAndYield<T>(Memory<T> memory, IRandomGenerator rng, int count)
			=> ShuffleAndYield(memory.ToArray(), rng, count);
	}
}
