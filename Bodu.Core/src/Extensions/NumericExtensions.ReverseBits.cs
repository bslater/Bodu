// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="NumericExtensions.ReverseBits.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace Bodu.Extensions
{
	public static partial class NumericExtensions
	{
		/// <summary>
		/// Reverses the order of the bits of an unsigned <see cref="byte" /> value.
		/// </summary>
		/// <param name="value">The unsigned byte value to reverse the bits for.</param>
		/// <returns>A byte where the order of the bits is reversed.</returns>
		/// <remarks>This method performs the reversal of the bits in the 8-bit unsigned byte value.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReverseBits(this byte value)
		{
			value = (byte)(((value >> 1) & 0x55) | ((value & 0x55) << 1));
			value = (byte)(((value >> 2) & 0x33) | ((value & 0x33) << 2));
			value = (byte)((value >> 4) | (value << 4));
			return value;
		}

		/// <summary>
		/// Reverses the order of the bits of an unsigned <see cref="ushort" /> value.
		/// </summary>
		/// <param name="value">The 16-bit unsigned integer value to reverse the bits for.</param>
		/// <returns>A 16-bit unsigned integer where the order of the bits is reversed.</returns>
		/// <remarks>This method performs the reversal of the bits in the 16-bit unsigned integer value.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ReverseBits(this ushort value)
		{
			value = (ushort)(((value >> 1) & 0x5555) | ((value & 0x5555) << 1));
			value = (ushort)(((value >> 2) & 0x3333) | ((value & 0x3333) << 2));
			value = (ushort)(((value >> 4) & 0x0F0F) | ((value & 0x0F0F) << 4));
			value = (ushort)((value >> 8) | (value << 8));
			return value;
		}

		/// <summary>
		/// Reverses the order of the bits of an unsigned <see cref="uint" /> value.
		/// </summary>
		/// <param name="value">The 32-bit unsigned integer value to reverse the bits for.</param>
		/// <returns>A 32-bit unsigned integer where the order of the bits is reversed.</returns>
		/// <remarks>This method performs the reversal of the bits in the 32-bit unsigned integer value.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReverseBits(this uint value)
		{
			value = ((value >> 1) & 0x55555555) | ((value & 0x55555555) << 1);
			value = ((value >> 2) & 0x33333333) | ((value & 0x33333333) << 2);
			value = ((value >> 4) & 0x0F0F0F0F) | ((value & 0x0F0F0F0F) << 4);
			value = ((value >> 8) & 0x00FF00FF) | ((value & 0x00FF00FF) << 8);
			value = (value >> 16) | (value << 16);
			return value;
		}

		/// <summary>
		/// Reverses the order of the bits of an unsigned <see cref="ulong" /> value.
		/// </summary>
		/// <param name="value">The 64-bit unsigned integer value to reverse the bits for.</param>
		/// <returns>A 64-bit unsigned integer where the order of the bits is reversed.</returns>
		/// <remarks>This method performs the reversal of the bits in the 64-bit unsigned integer value.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ReverseBits(this ulong value)
		{
			value = ((value >> 1) & 0x5555555555555555UL) | ((value & 0x5555555555555555UL) << 1);
			value = ((value >> 2) & 0x3333333333333333UL) | ((value & 0x3333333333333333UL) << 2);
			value = ((value >> 4) & 0x0F0F0F0F0F0F0F0FUL) | ((value & 0x0F0F0F0F0F0F0F0FUL) << 4);
			value = ((value >> 8) & 0x00FF00FF00FF00FFUL) | ((value & 0x00FF00FF00FF00FFUL) << 8);
			value = ((value >> 16) & 0x0000FFFF0000FFFFUL) | ((value & 0x0000FFFF0000FFFFUL) << 16);
			value = (value >> 32) | (value << 32);
			return value;
		}

		/// <summary>
		/// Reverses the order of the bits in a byte array.
		/// </summary>
		/// <param name="bytes">The byte array whose bits are to be reversed.</param>
		/// <returns>A new byte array with the bits of each byte reversed.</returns>
		/// <remarks>This method processes each byte in the array individually and reverses the bits within each byte.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] ReverseBits(this byte[] bytes)
		{
			if (bytes == null) return new byte[0];

			Span<byte> reversed = new byte[bytes.Length];

			for (int i = 0; i < bytes.Length; i++)
			{
				reversed[i] = bytes[i].ReverseBits();
			}

			return reversed.ToArray();
		}
	}
}