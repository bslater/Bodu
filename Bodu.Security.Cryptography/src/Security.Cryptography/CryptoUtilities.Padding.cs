// ---------------------------------------------------------------------------------------------------------------
// <copyright file="CryptoFunctions.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	public static partial class CryptoUtilities
	{
		/// <summary>
		/// Attempts to apply padding to the given input buffer using the specified mode.
		/// </summary>
		/// <param name="padding">The padding mode to apply.</param>
		/// <param name="blockSizeBytes">The block size in bytes.</param>
		/// <param name="source">The input buffer to pad.</param>
		/// <param name="destination">The destination span that receives the padded data.</param>
		/// <param name="bytesWritten">The number of bytes written to <paramref name="destination" />.</param>
		/// <returns><c>true</c> if padding was successfully applied; otherwise, <c>false</c>.</returns>
		public static bool TryPadBlock(
			PaddingMode padding,
			int blockSizeBytes,
			ReadOnlySpan<byte> source,
			Span<byte> destination,
			out int bytesWritten)
		{
			try
			{
				bytesWritten = PadBlock(padding, blockSizeBytes, source, destination);
				return true;
			}
			catch
			{
				bytesWritten = 0;
				return false;
			}
		}

		/// <summary>
		/// Attempts to remove padding from the given input buffer using the specified mode.
		/// </summary>
		/// <param name="padding">The padding mode to validate and remove.</param>
		/// <param name="blockSizeBytes">The block size in bytes.</param>
		/// <param name="source">The padded input buffer.</param>
		/// <param name="destination">The destination span that receives the depadded data.</param>
		/// <param name="bytesWritten">The number of bytes written to <paramref name="destination" />.</param>
		/// <returns><c>true</c> if depadding was successful; otherwise, <c>false</c>.</returns>
		public static bool TryDepadBlock(
			PaddingMode padding,
			int blockSizeBytes,
			ReadOnlySpan<byte> source,
			Span<byte> destination,
			out int bytesWritten)
		{
			try
			{
				bytesWritten = DepadBlock(padding, blockSizeBytes, source, destination);
				return true;
			}
			catch
			{
				bytesWritten = 0;
				return false;
			}
		}

		/// <summary>
		/// Applies padding to a block and returns a newly allocated array.
		/// </summary>
		public static byte[] PadBlock(PaddingMode padding, int blockSizeBytes, byte[] block, int offset, int count)
		{
			ThrowHelper.ThrowIfLessThan(blockSizeBytes, 1);
			byte[] result = new byte[count + blockSizeBytes];
			int written = PadBlock(padding, blockSizeBytes, new ReadOnlySpan<byte>(block, offset, count), result);
			Array.Resize(ref result, written);
			return result;
		}

		/// <summary>
		/// Applies padding to a block using the specified padding mode and writes to the destination span.
		/// </summary>
		/// <param name="padding">Padding mode to apply.</param>
		/// <param name="blockSizeBytes">The block size in bytes.</param>
		/// <param name="source">Input data to pad.</param>
		/// <param name="destination">Destination span for the padded result.</param>
		/// <returns>Total bytes written to the destination span.</returns>
		/// <exception cref="ArgumentException">Thrown when the destination span is too small.</exception>
		/// <exception cref="CryptographicException">Thrown if the padding mode is invalid.</exception>
		public static int PadBlock(
			PaddingMode padding,
			int blockSizeBytes,
			ReadOnlySpan<byte> source,
			Span<byte> destination)
		{
			// Validate padding mode
			switch (padding)
			{
				case PaddingMode.PKCS7:
				case PaddingMode.ANSIX923:
				case PaddingMode.ISO10126:
				case PaddingMode.Zeros:
				case PaddingMode.None:
					break;

				default:
					throw new CryptographicException(
						string.Format(ResourceStrings.CryptographicException_InvalidPropertyValue, nameof(SymmetricAlgorithm.Padding)));
			}

			ThrowHelper.ThrowIfLessThan(blockSizeBytes, 1);

			// Special case for PaddingMode.None — must be block-aligned
			if (padding == PaddingMode.None && source.Length % blockSizeBytes != 0)
				throw new CryptographicException(ResourceStrings.CryptographicException_PaddingModeNone_InputNotAligned);

			int padCount = blockSizeBytes - (source.Length % blockSizeBytes);
			if (padCount == blockSizeBytes && (padding == PaddingMode.None || padding == PaddingMode.Zeros))
				padCount = 0;

			int totalLen = source.Length + padCount;
			ThrowHelper.ThrowIfSpanLengthIsInsufficient(destination, source.Length, padCount);

			source.CopyTo(destination);

			Span<byte> padSpan = destination.Slice(source.Length, padCount);

			switch (padding)
			{
				case PaddingMode.None: // No action required (already validated alignment)
				case PaddingMode.Zeros:
					padSpan.Clear();
					break;

				case PaddingMode.PKCS7:
					padSpan.Fill((byte)padCount);
					break;

				case PaddingMode.ANSIX923:
					padSpan.Clear();
					padSpan[^1] = (byte)padCount;
					break;

				case PaddingMode.ISO10126:
					CryptoUtilities.FillWithRandomNonZeroBytes(padSpan.Slice(0, padCount - 1));
					padSpan[^1] = (byte)padCount;
					break;
			}

			return totalLen;
		}

		/// <summary>
		/// Removes padding from a block and returns a newly allocated array.
		/// </summary>
		public static byte[] DepadBlock(PaddingMode padding, int blockSizeBytes, byte[] block, int offset, int count)
		{
			byte[] temp = new byte[count];
			int written = DepadBlock(padding, blockSizeBytes, new ReadOnlySpan<byte>(block, offset, count), temp);
			byte[] result = new byte[written];
			Buffer.BlockCopy(temp, 0, result, 0, written);
			return result;
		}

		/// <summary>
		/// Removes padding from a block and writes the depadded data into the destination span.
		/// </summary>
		/// <param name="padding">The padding mode applied to the input data.</param>
		/// <param name="blockSizeBytes">The block size in bytes for validation.</param>
		/// <param name="source">The padded input data.</param>
		/// <param name="destination">The destination span to receive depadded data.</param>
		/// <returns>The number of unpadded bytes written to <paramref name="destination" />.</returns>
		/// <exception cref="CryptographicException">Thrown if the padding is invalid or unsupported.</exception>
		/// <summary>
		/// Removes padding from a block and writes the depadded data into the destination span.
		/// </summary>
		/// <param name="padding">The padding mode applied to the input data.</param>
		/// <param name="blockSizeBytes">The block size in bytes for validation.</param>
		/// <param name="source">The padded input data.</param>
		/// <param name="destination">The destination span to receive depadded data.</param>
		/// <returns>The number of unpadded bytes written to <paramref name="destination" />.</returns>
		/// <exception cref="CryptographicException">
		/// Thrown if the padding is invalid, the source is not block-aligned, or the padding mode is unsupported.
		/// </exception>
		public static int DepadBlock(
			PaddingMode padding,
			int blockSizeBytes,
			ReadOnlySpan<byte> source,
			Span<byte> destination)
		{
			ThrowHelper.ThrowIfLessThanOrEqual(blockSizeBytes, 0);
			ThrowHelper.ThrowIfSpanLengthNotPositiveMultipleOf(source, blockSizeBytes);

			int count = source.Length;

			switch (padding)
			{
				// Return the whole block where PaddingMode is None or Zeros
				case PaddingMode.None:
				case PaddingMode.Zeros:
					source.CopyTo(destination);
					return count;

				case PaddingMode.PKCS7:
				case PaddingMode.ANSIX923:
				case PaddingMode.ISO10126:
					break;

				default:
					throw new CryptographicException(
						string.Format(ResourceStrings.CryptographicException_InvalidPropertyValue, nameof(SymmetricAlgorithm.Padding)));
			}

			int padCount = source[^1];
			if (padCount <= 0 || padCount > blockSizeBytes)
				throw new CryptographicException(ResourceStrings.CryptographicException_InvalidPadding);

			ReadOnlySpan<byte> padRegion = source[^padCount..];

			if (padding == PaddingMode.PKCS7 && !IsUniformPadding(padRegion, (byte)padCount))
				throw new CryptographicException(ResourceStrings.CryptographicException_InvalidPadding);

			if (padding == PaddingMode.ANSIX923 && !IsUniformPadding(padRegion[..^1], 0x00))
				throw new CryptographicException(ResourceStrings.CryptographicException_InvalidPadding);

			int unpadded = count - padCount;
			source.Slice(0, unpadded).CopyTo(destination);
			return unpadded;
		}

		/// <summary>
		/// Validates whether the span contains only the expected value.
		/// </summary>
		private static bool IsUniformPadding(ReadOnlySpan<byte> span, byte expected)
		{
			foreach (byte b in span)
			{
				if (b != expected)
					return false;
			}

			return true;
		}
	}
}