// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="IEnumerableExtensions.Randomize.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

#if !NETSTANDARD2_0

using Bodu.Buffers;

#endif

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bodu.Collections.Generic.Extensions
{
	/// <summary>
	/// Specifies the available strategies for handling sequences during randomization.
	/// </summary>
	/// <remarks>Use <see cref="RandomizationMode" /> values to control how the source sequence is processed and randomized.</remarks>
	public enum RandomizationMode
	{
		/// <summary>
		/// Buffers all elements and applies in-place shuffling.
		/// </summary>
		BufferAll,

		/// <summary>
		/// Selects elements using reservoir sampling without full buffering.
		/// </summary>
		ReservoirSample,

		/// <summary>
		/// Uses a sliding window to shuffle while streaming.
		/// </summary>
		StreamWindowed,

		/// <summary>
		/// Lazily builds and shuffles a subset of items.
		/// </summary>
		LazyShuffle
	}

	/// <summary>
	/// Wraps <see cref="System.Random" /> as an <see cref="IRandomGenerator" /> implementation.
	/// </summary>
	public sealed class SystemRandomAdapter : IRandomGenerator
	{
		private readonly Random random;

		/// <summary>
		/// Initializes a new adapter with a default <see cref="System.Random" />.
		/// </summary>
		public SystemRandomAdapter() : this(new Random()) { }

		/// <summary>
		/// Initializes a new adapter using the specified <paramref name="random" /> instance.
		/// </summary>
		public SystemRandomAdapter(Random random) => this.random = random ?? throw new ArgumentNullException(nameof(random));

		/// <inheritdoc />
		public int Next(int maxValue) => random.Next(maxValue);
	}

	public static partial class IEnumerableExtensions
	{
		/// <inheritdoc cref="Randomize{T}(IEnumerable{T}, RandomizationMode, IRandomGenerator, int?)" />
		public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source) =>
			source.Randomize(RandomizationMode.BufferAll, new SystemRandomAdapter(), null);

		/// <summary>
		/// Randomizes the source sequence using a specified strategy and generator.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="source">The sequence to randomize.</param>
		/// <param name="mode">The randomization strategy.</param>
		/// <param name="rng">The random number generator.</param>
		/// <param name="count">The number of items to return; all if null.</param>
		/// <returns>A randomized sequence of <typeparamref name="T" />.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="source" /> or <paramref name="rng" /> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="count" /> is negative or too large.</exception>
		/// <exception cref="ArgumentException">If <paramref name="count" /> is required but not provided.</exception>
		public static IEnumerable<T> Randomize<T>(
			this IEnumerable<T> source,
			RandomizationMode mode,
			IRandomGenerator rng,
			int? count = null)
		{
			ThrowHelper.ThrowIfNull(source);
			ThrowHelper.ThrowIfNull(rng);
			ThrowHelper.ThrowIfLessThan(count, 0);

			return mode switch
			{
				RandomizationMode.BufferAll => RandomizeBuffered(source, rng, count),
				RandomizationMode.ReservoirSample => count is null
					? throw new ArgumentException(
						string.Format(ResourceStrings.Arg_Required_ParameterRequiredIf, nameof(count), nameof(mode), nameof(RandomizationMode.ReservoirSample)), nameof(count))
					: ReservoirSample(source, rng, count.Value),
				RandomizationMode.StreamWindowed => StreamWindowedShuffle(source, rng),
				RandomizationMode.LazyShuffle => count is null
					? throw new ArgumentException(
						string.Format(ResourceStrings.Arg_Required_ParameterRequiredIf, nameof(count), nameof(mode), nameof(RandomizationMode.LazyShuffle)), nameof(count))
					: LazyShuffle(source, rng, count.Value),
				_ => throw new ArgumentOutOfRangeException(nameof(mode), string.Format(ResourceStrings.Arg_OutOfRangeException_EnumValue, nameof(RandomizationMode)))
			};
		}

		/// <summary>
		/// Buffers all elements from the source and yields a randomized subset using in-place shuffling.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="source">The sequence to randomize.</param>
		/// <param name="rng">The random number generator.</param>
		/// <param name="count">The number of items to return; all if null.</param>
		/// <returns>A shuffled subset of the source sequence.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count" /> exceeds the available number of items.</exception>
		private static IEnumerable<T> RandomizeBuffered<T>(IEnumerable<T> source, IRandomGenerator rng, int? count)
		{
			IList<T> buffer;
			int availableCount;

#if NETSTANDARD2_0
			var list = source is ICollection<T> col ? new List<T>(col) : new List<T>(source);
			buffer = list;
			availableCount = list.Count;
#else
			using var builder = new PooledBufferBuilder<T>();

			if (source is IReadOnlyCollection<T> collection && builder.TryCopyFrom(collection))
			{
				buffer = builder.AsArray(); // T[]
				availableCount = builder.Count;
			}
			else
			{
				builder.AppendRange(source);
				buffer = builder.AsArray();
				availableCount = builder.Count;
			}
#endif

			int takeCount = count ?? availableCount;
			ThrowHelper.ThrowIfGreaterThanOther(takeCount, availableCount);

			if (takeCount == availableCount)
				return ShuffleHelpers.ShuffleAndYield(buffer, rng); // IList<T> or T[]
			else
				return ShuffleHelpers.ShuffleAndYield(buffer.ToArray(), rng, takeCount);
		}

		/// <summary>
		/// Performs a reservoir sampling algorithm to randomly select a fixed number of elements from the sequence.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="source">The source sequence to sample from.</param>
		/// <param name="rng">The random number generator.</param>
		/// <param name="count">The number of elements to sample.</param>
		/// <returns>A shuffled array of <paramref name="count" /> sampled elements.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count" /> exceeds the number of available items.</exception>
		private static IEnumerable<T> ReservoirSample<T>(IEnumerable<T> source, IRandomGenerator rng, int count)
		{
			T[] reservoir = new T[count];
			int index = 0;
			using var enumerator = source.GetEnumerator();
			while (index < count && enumerator.MoveNext())
				reservoir[index++] = enumerator.Current;

			if (index < count)
				throw new ArgumentOutOfRangeException(nameof(count), ResourceStrings.Arg_OutOfRange_CountGreaterThanSource);

			int seen = count;
			while (enumerator.MoveNext())
			{
				int j = rng.Next(++seen);
				if (j < count)
					reservoir[j] = enumerator.Current;
			}

			return ShuffleHelpers.ShuffleAndYield(reservoir, rng, count);
		}

		/// <summary>
		/// Lazily samples and shuffles a fixed-size subset of elements from the source sequence.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="source">The sequence to randomize.</param>
		/// <param name="rng">The random number generator.</param>
		/// <param name="count">The number of elements to return.</param>
		/// <returns>A lazily shuffled sequence of <paramref name="count" /> elements.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count" /> exceeds available elements.</exception>
		private static IEnumerable<T> LazyShuffle<T>(IEnumerable<T> source, IRandomGenerator rng, int count)
		{
			if (count <= 0)
				yield break;

			T[] reservoir = new T[count];
			int index = 0;
			using var enumerator = source.GetEnumerator();
			while (index < count && enumerator.MoveNext())
				reservoir[index++] = enumerator.Current;

			if (index < count)
				throw new ArgumentOutOfRangeException(nameof(count), ResourceStrings.Arg_OutOfRange_CountGreaterThanSource);

			int seen = count;
			while (enumerator.MoveNext())
			{
				int i = rng.Next(++seen);
				if (i < count)
					reservoir[i] = enumerator.Current;
			}

			foreach (var item in ShuffleHelpers.ShuffleAndYield(reservoir, rng, count))
				yield return item;
		}

		/// <summary>
		/// Applies windowed random sampling from the source sequence while streaming.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="source">The sequence to randomize.</param>
		/// <param name="rng">The random number generator.</param>
		/// <param name="windowSize">The size of the rolling window used during sampling.</param>
		/// <returns>A randomized sequence of elements using a sliding window.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="windowSize" /> is zero or negative.</exception>
		private static IEnumerable<T> StreamWindowedShuffle<T>(IEnumerable<T> source, IRandomGenerator rng, int windowSize = 64)
		{
			ThrowHelper.ThrowIfLessThan(windowSize, 0);

			Queue<T> window = new(windowSize);
			using var enumerator = source.GetEnumerator();

			while (window.Count < windowSize && enumerator.MoveNext())
				window.Enqueue(enumerator.Current);

			while (enumerator.MoveNext())
			{
				int i = rng.Next(window.Count);
				T[] temp = window.ToArray();
				yield return temp[i];
				window.Clear();
				for (int j = 0; j < temp.Length; j++)
					if (j != i) window.Enqueue(temp[j]);
				window.Enqueue(enumerator.Current);
			}

			foreach (var item in ShuffleHelpers.ShuffleAndYield(window.ToArray(), rng, window.Count))
				yield return item;
		}
	}
}