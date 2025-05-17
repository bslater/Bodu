// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Number.ReverseBytes.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort ReverseBytes(this ushort value) =>
			(ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);

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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ReverseBytes(this uint value) =>
			(value & 0x000000FFU) << 0x18 | (value & 0x0000FF00U) << 0x08 |
			(value & 0x00FF0000U) >> 0x08 | (value & 0xFF000000U) >> 0x18;

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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ReverseBytes(this ulong value) =>
			(value & 0x00000000000000FFUL) << 0x38 | (value & 0x000000000000FF00UL) << 0x28 | (value & 0x0000000000FF0000UL) << 0x18 | (value & 0x00000000FF000000UL) << 0x08 |
			(value & 0x000000FF00000000UL) >> 0x08 | (value & 0x0000FF0000000000UL) >> 0x18 | (value & 0x00FF000000000000UL) >> 0x28 | (value & 0xFF00000000000000UL) >> 0x38;
	}
}