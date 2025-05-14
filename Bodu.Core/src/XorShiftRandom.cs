﻿// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="XorShiftRandom.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace Bodu
{
	/// <summary>
	/// Represents a high-performance, non-cryptographic pseudo-random number generator based on the XOR-shift algorithm.
	/// </summary>
	/// <remarks>
	/// This class is designed as a drop-in replacement for <see cref="System.Random" /> with better performance characteristics. It is not
	/// suitable for cryptographic purposes.
	/// </remarks>
	public sealed class XorShiftRandom
		: System.Random
		, IRandomGenerator
	{
		private uint x, y, z, w;

		/// <summary>
		/// Initializes a new instance of the <see cref="XorShiftRandom" /> class using a system-generated seed.
		/// </summary>
		/// <remarks>The default seed is derived from <see cref="Environment.TickCount" /> at the time of construction.</remarks>
		public XorShiftRandom()
			: this((uint)Environment.TickCount)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="XorShiftRandom" /> class with a 32-bit signed integer seed.
		/// </summary>
		/// <param name="seed">The seed used to initialize the random generator.</param>
		public XorShiftRandom(int seed)
			: this((uint)seed)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="XorShiftRandom" /> class with a 32-bit unsigned seed.
		/// </summary>
		/// <param name="seed">The seed used to initialize the random generator.</param>
		public XorShiftRandom(uint seed)
		{
			// Initialize four internal states with XOR-variations of the seed for better distribution
			x = seed;
			y = seed ^ 0x6C8E9CF5U;
			z = seed ^ 0x94D049BBU;
			w = seed ^ 0x5A17D7F9U;
		}

		/// <inheritdoc />
		public override int Next()
			=> Next(int.MaxValue);

		/// <inheritdoc />
		public override int Next(int maxValue)
		{
			ThrowHelper.ThrowIfLessThanOrEqual(maxValue, 0);
			return (int)(NextUInt32() % (uint)maxValue);
		}

		/// <inheritdoc />
		public override int Next(int minValue, int maxValue)
		{
			ThrowHelper.ThrowIfGreaterThanOrEqualOther(minValue, maxValue);
			uint range = (uint)(maxValue - minValue);
			return minValue + (int)(NextUInt32() % range);
		}

		/// <inheritdoc />
		public override double NextDouble()
			=> NextUInt32() / (double)uint.MaxValue;

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void NextBytes(byte[] buffer)
		{
			ThrowHelper.ThrowIfNull(buffer);

			for (int i = 0; i < buffer.Length; i++)
			{
				if ((i & 3) == 0)
				{
					uint rnd = NextUInt32();
					buffer[i++] = (byte)(rnd & 0xFF);
					if (i < buffer.Length) buffer[i++] = (byte)((rnd >> 8) & 0xFF);
					if (i < buffer.Length) buffer[i++] = (byte)((rnd >> 16) & 0xFF);
					if (i < buffer.Length) buffer[i++] = (byte)((rnd >> 24) & 0xFF);
					i--; // account for loop increment
				}
			}
		}

		/// <summary>
		/// Generates the next 32-bit random number using XOR-shift algorithm.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private uint NextUInt32()
		{
			uint t = x ^ (x << 11);
			x = y; y = z; z = w;
			w ^= (w >> 19) ^ t ^ (t >> 8);
			return w;
		}
	}
}