using System.Runtime.CompilerServices;

namespace Bodu.Extensions
{
	public static partial class NumericExtensions
	{
		/// <summary>
		/// Reverses the order of the bits of an signed <see cref="short" /> value.
		/// </summary>
		/// <param name="value">The 16-bit unsigned integer value to reverse the bits for.</param>
		/// <returns>A 16-bit unsigned integer where the order of the bits is reversed.</returns>
		/// <remarks>This method performs the reversal of the bits in the 16-bit unsigned integer value.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static short ReverseBits(this short value)
		{
			short result = 0;
			for (int x = 0; x < 64; ++x)
			{
				result <<= 1;
				result |= (short)(value & 1);
				value >>= 1;
			}

			return result;
		}

		/// <summary>
		/// Reverses the order of the bits of an signed <see cref="int" /> value.
		/// </summary>
		/// <param name="value">The 32-bit unsigned integer value to reverse the bits for.</param>
		/// <returns>A 32-bit unsigned integer where the order of the bits is reversed.</returns>
		/// <remarks>This method performs the reversal of the bits in the 32-bit unsigned integer value.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ReverseBits(this int value)
		{
			int reversed = 0;
			for (int i = 0; i < 32; i++)  // 32 bits for int
			{
				reversed <<= 1;  // Shift the result left by 1 bit
				reversed |= (value & 1);  // Set the last bit of reversed to the last bit of value
				value >>= 1;  // Shift the value right by 1 bit
			}
			return reversed;
		}

		/// <summary>
		/// Reverses the order of the bits of an unsigned <see cref="long" /> value.
		/// </summary>
		/// <param name="value">The 64-bit unsigned integer value to reverse the bits for.</param>
		/// <returns>A 64-bit unsigned integer where the order of the bits is reversed.</returns>
		/// <remarks>This method performs the reversal of the bits in the 64-bit unsigned integer value.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ReverseBits(this long value)
		{
			long result = 0;
			for (int x = 0; x < 64; ++x)
			{
				result <<= 1;
				result |= (value & 1);
				value >>= 1;
			}

			return result;
		}

		/// <summary>
		/// Reverses the order of the bits of an unsigned <see cref="byte" /> value.
		/// </summary>
		/// <param name="value">The unsigned byte value to reverse the bits for.</param>
		/// <returns>A byte where the order of the bits is reversed.</returns>
		/// <remarks>This method performs the reversal of the bits in the 8-bit unsigned byte value.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte ReverseBits(this byte value)
		{
			byte result = 0;
			for (int x = 0; x < 8; ++x)
			{
				result <<= 1;
				result |= (byte)(value & 1);
				value >>= 1;
			}

			return result;
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
			ushort result = 0;
			for (int x = 0; x < 16; ++x)
			{
				result <<= 1;
				result |= (ushort)(value & 1);
				value >>= 1;
			}

			return result;
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
			uint result = 0U;
			for (int x = 0; x < 32; ++x)
			{
				result <<= 1;
				result |= value & 1;
				value >>= 1;
			}

			return result;
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
			ulong result = 0UL;
			for (int x = 0; x < 64; ++x)
			{
				result <<= 1;
				result |= value & 1;
				value >>= 1;
			}

			return result;
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
			byte[] reversed = new byte[bytes.Length];
			for (int i = 0; i < bytes.Length; i++)
			{
				reversed[i] = bytes[i].ReverseBits();
			}
			return reversed;
		}
	}
}
