﻿using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	public static partial class CryptoHelpers
	{
		/// <summary>
		/// Fills the provided byte array with cryptographically secure random bytes, ensuring that no byte is equal to <c>0x00</c>.
		/// </summary>
		/// <param name="buffer">The byte array to fill.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="buffer" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="buffer" /> is empty.</exception>
		/// <remarks>Delegates to the span-based overload. Repeats random generation until the buffer contains no zero bytes.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void FillWithRandomNonZeroBytes(byte[] buffer)
		{
			ThrowHelper.ThrowIfNull(buffer);
			ThrowHelper.ThrowIfArrayLengthIsZero(buffer);
			FillWithRandomNonZeroBytes(buffer.AsSpan());
		}

		/// <summary>
		/// Fills the provided span with cryptographically secure random bytes, ensuring that no byte in the span is equal to <c>0x00</c>.
		/// </summary>
		/// <param name="buffer">The span to fill.</param>
		/// <remarks>Loops until all bytes are non-zero. Uses <see cref="RandomNumberGenerator.Fill(Span{byte})" />.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void FillWithRandomNonZeroBytes(Span<byte> buffer)
		{
			FillWithRandomBytesExcluding(0x00, buffer);
		}

		/// <summary>
		/// Attempts to fill the provided span with cryptographically secure random bytes that do not include <c>0x00</c>. Retries a limited
		/// number of times.
		/// </summary>
		/// <param name="buffer">The span to fill.</param>
		/// <returns>
		/// <c>true</c> if the buffer was filled successfully without any zero bytes; otherwise, <c>false</c> after the retry limit is reached.
		/// </returns>
		/// <remarks>Intended for performance-sensitive scenarios where indefinite retry is undesirable.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool TryFillWithRandomNonZeroBytes(Span<byte> buffer)
		{
			const int maxAttempts = 5;

#if NETSTANDARD2_0
    using (var rng = RandomNumberGenerator.Create())
    {
        byte[] temp = new byte[buffer.Length];

        for (int i = 0; i < maxAttempts; i++)
        {
            rng.GetBytes(temp);
            if (Array.IndexOf(temp, (byte)0) < 0)
            {
                temp.CopyTo(buffer);
                return true;
            }
        }
    }
#else
			for (int i = 0; i < maxAttempts; i++)
			{
				RandomNumberGenerator.Fill(buffer);
				if (buffer.IndexOf((byte)0) < 0)
					return true;
			}
#endif

			return false;
		}

		/// <summary>
		/// Returns a new byte array filled with cryptographically secure random bytes that are guaranteed to be non-zero.
		/// </summary>
		/// <param name="length">The number of random bytes to generate.</param>
		/// <returns>A <see cref="byte" /> array of the specified length with only non-zero values.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="length" /> is less than or equal to zero.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static byte[] GetRandomNonZeroBytes(int length)
		{
			ThrowHelper.ThrowIfLessThanOrEqual(length, 0);
			byte[] buffer = GC.AllocateUninitializedArray<byte>(length);
			FillWithRandomNonZeroBytes(buffer.AsSpan());
			return buffer;
		}

		/// <summary>
		/// Fills the specified span with random bytes, excluding the given <paramref name="forbidden" /> byte.
		/// </summary>
		/// <param name="forbidden">The byte value to exclude from the result.</param>
		/// <param name="buffer">The span to fill with random bytes.</param>
		/// <remarks>Repeatedly fills the buffer until <paramref name="forbidden" /> is no longer present.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void FillWithRandomBytesExcluding(byte forbidden, Span<byte> buffer)
		{
#if NETSTANDARD2_0
			using (var rng = RandomNumberGenerator.Create())
			{
				byte[] temp = new byte[buffer.Length];
				do rng.GetBytes(temp);
				while (Array.IndexOf(temp, forbidden) >= 0);

				temp.CopyTo(buffer);
			}
#else
			do RandomNumberGenerator.Fill(buffer);
			while (buffer.IndexOf(forbidden) >= 0);
#endif
		}

		/// <summary>
		/// Returns a new array of random bytes of the specified <paramref name="length" />, excluding any occurrences of the
		/// <paramref name="forbidden" /> byte.
		/// </summary>
		/// <param name="forbidden">The byte value to exclude.</param>
		/// <param name="length">The number of bytes to generate.</param>
		/// <returns>A <see cref="byte" /> array filled with random bytes that do not include <paramref name="forbidden" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="length" /> is less than or equal to zero.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static byte[] GetRandomBytesExcluding(byte forbidden, int length)
		{
			ThrowHelper.ThrowIfLessThanOrEqual(length, 0);
			byte[] buffer = GC.AllocateUninitializedArray<byte>(length);
			FillWithRandomBytesExcluding(forbidden, buffer.AsSpan());
			return buffer;
		}
	}
}