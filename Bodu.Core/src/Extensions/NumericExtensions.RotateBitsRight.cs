// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Number.RotateBitsLeft.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace Bodu.Extensions
{
	public static partial class NumericExtensions
	{
		/// <summary>
		/// Rotates the bits of the specified 8-bit unsigned ( <see cref="byte" />) value to the right by the specified <paramref name="count" />.
		/// </summary>
		/// <param name="value">The 8-bit unsigned value whose bits are to be rotated.</param>
		/// <param name="count">The number of positions to rotate the bits. Must be between 0 and 8.</param>
		/// <returns>A <see cref="byte" /> with its bits rotated to the right by <paramref name="count" /> positions.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="count" /> is less than 0 or greater than the number of bits (8) in a <see cref="byte" />.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte RotateBitsRight(this byte value, int count)
		{
			ThrowHelper.ThrowIfOutOfRange(count, 0, 8);

			return (byte)((value >> count) | (value << (8 - count)));
		}

		/// <summary>
		/// Rotates the bits of the specified 16-bit unsigned ( <see cref="ushort" />) value to the right by the specified <paramref name="count" />.
		/// </summary>
		/// <param name="value">The 16-bit unsigned integer whose bits are to be rotated.</param>
		/// <param name="count">The number of positions to rotate the bits. Must be between 0 and 16.</param>
		/// <returns>A <see cref="ushort" /> with its bits rotated to the right by <paramref name="count" /> positions.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="count" /> is less than 0 or greater than the number of bits (16) in a <see cref="ushort" />.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort RotateBitsRight(this ushort value, int count)
		{
			ThrowHelper.ThrowIfOutOfRange(count, 0, 16);

			return (ushort)((value >> count) | (value << (16 - count)));
		}

		/// <summary>
		/// Rotates the bits of the specified 32-bit unsigned ( <see cref="uint" />) value to the right by the specified <paramref name="count" />.
		/// </summary>
		/// <param name="value">The 32-bit unsigned integer whose bits are to be rotated.</param>
		/// <param name="count">The number of positions to rotate the bits. Must be between 0 and 32.</param>
		/// <returns>A <see cref="uint" /> with its bits rotated to the right by <paramref name="count" /> positions.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="count" /> is less than 0 or greater than the number of bits (32) in a <see cref="uint" />.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint RotateBitsRight(this uint value, int count)
		{
			ThrowHelper.ThrowIfOutOfRange(count, 0, 32);

			return (value >> count) | (value << (32 - count));
		}

		/// <summary>
		/// Rotates the bits of the specified 64-bit unsigned ( <see cref="ulong" />) value to the right by the specified <paramref name="count" />.
		/// </summary>
		/// <param name="value">The 64-bit unsigned integer whose bits are to be rotated.</param>
		/// <param name="count">The number of positions to rotate the bits. Must be between 0 and 64.</param>
		/// <returns>A <see cref="ulong" /> with its bits rotated to the right by <paramref name="count" /> positions.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="count" /> is less than 0 or greater than the number of bits (64) in a <see cref="ulong" />.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong RotateBitsRight(this ulong value, int count)
		{
			ThrowHelper.ThrowIfOutOfRange(count, 0, 64);

			return (value >> count) | (value << (64 - count));
		}
	}
}