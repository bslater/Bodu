using System;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Performs cryptographic transformations using the Skipjack block cipher algorithm. Supports encryption and decryption in CBC mode
	/// with configurable padding.
	/// </summary>
	/// <remarks>
	/// This class integrates a block cipher engine ( <see cref="IBlockCipher" />) with a cipher mode (
	/// <see cref="IBlockCipherModeTransform" />) and padding scheme ( <see cref="IPaddingStrategy" />). It supports both streaming (via
	/// <see cref="TransformBlock" />) and final block processing (via <see cref="TransformFinalBlock" />), following the
	/// <see cref="ICryptoTransform" /> contract.
	/// </remarks>
	public sealed class SkipjackTransform : ICryptoTransform
	{
		private readonly int blockSize;
		private readonly IBlockCipher cipher;
		private readonly bool encrypt;
		private readonly IBlockCipherModeTransform mode;
		private readonly IPaddingStrategy padding;
		private byte[]? deferredInput;

		/// <summary>
		/// Initializes a new instance of the <see cref="SkipjackTransform" /> class using the specified cipher, mode, padding, and
		/// initialization vector.
		/// </summary>
		/// <param name="cipher">The block cipher engine to use for encryption or decryption.</param>
		/// <param name="cipherMode">The block cipher mode of operation (e.g., CBC, CFB).</param>
		/// <param name="paddingMode">The padding scheme to apply to input data.</param>
		/// <param name="iv">The initialization vector to use for the block cipher mode.</param>
		/// <param name="encrypt"><c>true</c> to encrypt; <c>false</c> to decrypt.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="cipher" /> or <paramref name="iv" /> is <c>null</c>.</exception>
		public SkipjackTransform(IBlockCipher cipher, CipherBlockMode cipherMode, PaddingMode paddingMode, byte[] iv, bool encrypt)
		{
			this.cipher = cipher ?? throw new ArgumentNullException(nameof(cipher));
			this.encrypt = encrypt;
			this.blockSize = cipher.BlockSize;
			this.mode = BlockCipherModeFactory.Create(cipherMode, cipher, iv);
			this.padding = PaddingFactory.Create(paddingMode);
		}

		/// <inheritdoc />
		public bool CanReuseTransform => false;

		/// <inheritdoc />
		public bool CanTransformMultipleBlocks => true;

		/// <inheritdoc />
		public int InputBlockSize => blockSize;

		/// <inheritdoc />
		public int OutputBlockSize => blockSize;

		/// <inheritdoc />
		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc />
		/// <summary>
		/// Transforms a block of bytes and writes the output to the specified buffer.
		/// </summary>
		/// <param name="inputBuffer">The input data buffer.</param>
		/// <param name="inputOffset">The byte offset into <paramref name="inputBuffer" /> to begin reading from.</param>
		/// <param name="inputCount">The number of bytes to read from <paramref name="inputBuffer" />.</param>
		/// <param name="outputBuffer">The buffer to write the transformed data to.</param>
		/// <param name="outputOffset">The byte offset into <paramref name="outputBuffer" /> to begin writing at.</param>
		/// <returns>The number of bytes written to <paramref name="outputBuffer" />.</returns>
		/// <exception cref="ArgumentException">Thrown if the input or output spans are invalid or insufficient in length.</exception>
		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount,
								  byte[] outputBuffer, int outputOffset)
		{
			ReadOnlySpan<byte> input = inputBuffer.AsSpan(inputOffset, inputCount);
			Span<byte> output = outputBuffer.AsSpan(outputOffset, inputCount);

			if (encrypt)
			{
				// ENCRYPTION: transform all blocks immediately
				return mode.Transform(input, output, true);
			}
			else
			{
				// DECRYPTION: defer final block to allow padding validation
				bool stripPadding = padding is Pkcs7Padding; // Extend with other depaddable schemes

				if (stripPadding && input.Length <= blockSize)
				{
					// Buffer the last block until finalization
					deferredInput = input.ToArray();
					return 0;
				}

				int bytesToProcess = input.Length;
				if (stripPadding)
				{
					// Retain last block for padding removal later
					bytesToProcess -= blockSize;
					deferredInput = input.Slice(bytesToProcess).ToArray();
				}

				return mode.Transform(input.Slice(0, bytesToProcess), output.Slice(0, bytesToProcess), false);
			}
		}

		/// <inheritdoc />
		/// <summary>
		/// Transforms the final block of data, applying padding (or removing it, if decrypting).
		/// </summary>
		/// <param name="inputBuffer">The final portion of input data to transform.</param>
		/// <param name="inputOffset">The byte offset in the input buffer to begin reading from.</param>
		/// <param name="inputCount">The number of bytes to read from <paramref name="inputBuffer" />.</param>
		/// <returns>A new array containing the transformed final block.</returns>
		/// <exception cref="CryptographicException">Thrown if the padding is invalid during decryption.</exception>
		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			ReadOnlySpan<byte> input = inputBuffer.AsSpan(inputOffset, inputCount);

			if (encrypt)
			{
				byte[] padded = padding.Pad(input, blockSize);
				byte[] output = new byte[padded.Length];
				mode.Transform(padded, output, true);
				return output;
			}
			else
			{
				byte[] combined = Combine(deferredInput, input);
				byte[] decrypted = new byte[combined.Length];
				mode.Transform(combined, decrypted, false);
				return padding.Unpad(decrypted, cipher.BlockSize);
			}
		}

		/// <summary>
		/// Combines a deferred block with a new input span to produce a single contiguous byte array.
		/// </summary>
		/// <param name="first">The previously cached partial block or <c>null</c>.</param>
		/// <param name="second">The incoming data to append.</param>
		/// <returns>A new array containing the concatenated data.</returns>
		private static byte[] Combine(byte[]? first, ReadOnlySpan<byte> second)
		{
			if (first == null || first.Length == 0)
				return second.ToArray();

			byte[] result = new byte[first.Length + second.Length];
			Buffer.BlockCopy(first, 0, result, 0, first.Length);
			second.CopyTo(result.AsSpan(first.Length));
			return result;
		}
	}
}