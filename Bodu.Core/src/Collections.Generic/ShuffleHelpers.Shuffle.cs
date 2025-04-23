using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Bodu.Collections.Extensions;

namespace Bodu.Collections.Generic
{
	public static partial class ShuffleHelpers
	{
		/// <summary>
		/// Performs an in-place Fisher–Yates shuffle over a span of elements.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the span.</typeparam>
		/// <param name="span">The span of elements to shuffle.</param>
		/// <param name="rng">The random number generator to use for shuffling.</param>
		/// <remarks>
		/// This method shuffles the span in-place using the Fisher–Yates algorithm. The input span is mutated. This method is optimized for
		/// performance and is suitable for use on stack-allocated or pooled data.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Shuffle<T>(Span<T> span, IRandomGenerator rng)
		{
			for (int i = span.Length - 1; i > 0; i--)
			{
				int j = rng.Next(i + 1);
				(span[i], span[j]) = (span[j], span[i]);
			}
		}

		/// <summary>
		/// Performs an in-place Fisher–Yates shuffle over a memory region.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the memory.</typeparam>
		/// <param name="memory">The memory region to shuffle.</param>
		/// <param name="rng">The random number generator to use for shuffling.</param>
		/// <remarks>
		/// This method shuffles the memory in-place by accessing its underlying span. The input memory region is mutated. Useful when
		/// working with pooled or shared buffers represented as <see cref="Memory{T}" />.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Shuffle<T>(Memory<T> memory, IRandomGenerator rng)
		{
			Shuffle(memory.Span, rng);
		}

		/// <summary>
		/// Performs an in-place Fisher–Yates shuffle over the provided array.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the array.</typeparam>
		/// <param name="array">The array of elements to shuffle.</param>
		/// <param name="rng">The random number generator to use for shuffling.</param>
		/// <remarks>
		/// This method modifies the original array using the Fisher–Yates algorithm. The order of elements will be randomized in-place.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Shuffle<T>(T[] array, IRandomGenerator rng)
		{
			int available = array.Length;

			while (available-- > 0)
			{
				int i = rng.Next(available);
				array[i] = array[--available];
			}
		}
	}
}
