// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Number.ReverseWords.cs" company="PlaceholderCompany">
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
		/// Reverses the byte order of an unsigned <see cref="ushort" /> integer value.
		/// </summary>
		/// <param name="value">The 16-bit unsigned integer value.</param>
		/// <returns>The 16-bit unsigned integer value with the byte order reversed.</returns>
		/// <remarks>
		/// <para>
		/// This method swaps the byte order of an unsigned <see cref="ushort" /> integer value, converting from little endian to big endian
		/// and vice versa.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value" /> is not within the valid range of <see cref="ushort" />.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ReverseWords(this ushort value) =>
			(ushort)((value >> 8) | (value << 8));

		/// <summary>
		/// Reverses the byte order of an unsigned <see cref="uint" /> integer value.
		/// </summary>
		/// <param name="value">The 32-bit unsigned integer value.</param>
		/// <returns>The 32-bit unsigned integer value with the byte order reversed.</returns>
		/// <remarks>
		/// <para>
		/// This method swaps the byte order of an unsigned <see cref="uint" /> integer value, converting from little endian to big endian
		/// and vice versa.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value" /> is not within the valid range of <see cref="uint" />.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReverseWords(this uint value) =>
			(value >> 24) | ((value >> 8) & 0x0000FF00U) |
			((value << 8) & 0x00FF0000U) | (value << 24);

		/// <summary>
		/// Reverses the byte order of an unsigned <see cref="ulong" /> integer value.
		/// </summary>
		/// <param name="value">The 64-bit unsigned integer value.</param>
		/// <returns>The 64-bit unsigned integer value with the byte order reversed.</returns>
		/// <remarks>
		/// <para>
		/// This method swaps the byte order of an unsigned <see cref="ulong" /> integer value, converting from little endian to big endian
		/// and vice versa.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value" /> is not within the valid range of <see cref="ulong" />.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ReverseWords(this ulong value) =>
			(value >> 56) | ((value >> 40) & 0x000000000000FF00UL) | ((value >> 24) & 0x0000000000FF0000UL) | ((value >> 8) & 0x00000000FF000000UL) |
			((value << 8) & 0x000000FF00000000UL) | ((value << 24) & 0x0000FF0000000000UL) | ((value << 40) & 0x00FF000000000000UL) | (value << 56);

		/// <summary>
		/// Reverses the byte order of each 16-bit word in a byte array and returns a new array.
		/// </summary>
		/// <param name="bytes">The byte array to reverse the byte order of each 16-bit word.</param>
		/// <returns>A new byte array with the byte order of each 16-bit word reversed.</returns>
		/// <remarks>This method processes the byte array in pairs of bytes (16-bit words), reversing the order of bytes in each pair.</remarks>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="bytes" /> is null.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if the length of <paramref name="bytes" /> is not a positive multiple of 2 (even length).
		/// </exception>
		public static byte[] ReverseWords(this byte[] bytes)
		{
			ThrowHelper.ThrowIfNull(bytes);
			ThrowHelper.ThrowIfArrayLengthNotPositiveMultipleOf(bytes, 2);

			Span<byte> span = new Span<byte>(bytes);

			byte temp;
			for (int i = 0; i < bytes.Length; i += 2)
			{
				temp = span[i];
				span[i] = span[i + 1];
				span[i + 1] = temp;
			}

			return span.ToArray();
		}

		/// <summary>
		/// Reverses the byte order of each 16-bit word in a <see cref="Span{T}" /> and modifies it in place.
		/// </summary>
		/// <param name="bytes">The byte span to reverse the byte order of each 16-bit word.</param>
		/// <remarks>This method processes the byte span in pairs of bytes (16-bit words), reversing the order of bytes in each pair.</remarks>
		/// <exception cref="ArgumentException">
		/// Thrown if the length of <paramref name="bytes" /> is not a positive multiple of 2 (even length).
		/// </exception>
		/// <returns>Nothing. The original <see cref="Span{T}" /> is modified in place.</returns>
		public static void ReverseWords(this Span<byte> bytes)
		{
			ThrowHelper.ThrowIfArrayLengthNotPositiveMultipleOf(bytes, 2);

			byte temp;
			for (int i = 0; i < bytes.Length; i += 2)
			{
				temp = bytes[i];
				bytes[i] = bytes[i + 1];
				bytes[i + 1] = temp;
			}
		}
	}
}