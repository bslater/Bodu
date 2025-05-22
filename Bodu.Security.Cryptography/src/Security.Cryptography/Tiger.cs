// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Tiger.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------
using Bodu.Extensions;
using System;
using System.Buffers.Binary;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes the hash for the input data using the <c>Tiger</c> hash algorithm. This implementation uses a block-based transformation
	/// optimized for 64-bit platforms and supports multiple output lengths (128, 160, or 192 bits). This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="Tiger" /> is a non-cryptographic hash function designed for high-speed hashing on 64-bit architectures. It processes
	/// input in 512-bit (64-byte) blocks using three 64-bit internal state variables and applies three rounds of transformations. Each
	/// round includes eight S-box–driven mixing steps and word-level permutations to ensure diffusion.
	/// </para>
	/// <para>
	/// This implementation supports configurable output sizes corresponding to the standard Tiger variants: <c>Tiger/128</c>,
	/// <c>Tiger/160</c>, and <c>Tiger/192</c>. The full 192-bit hash is always computed internally, and shorter outputs are produced by truncation.
	/// </para>
	/// <para>
	/// Tiger is well suited for fast, non-cryptographic checksums, fingerprinting, and hash table indexing. It is not intended for secure
	/// integrity validation or cryptographic use.
	/// </para>
	/// <note type="important">This algorithm is <b>not</b> suitable for cryptographic applications such as password hashing, digital
	/// signatures, or secure data integrity checks.</note>
	/// </remarks>
	public sealed partial class Tiger
		: BlockHashAlgorithm<Tiger>
	{
		private const int MaxOutputBits = 192;

		private bool disposed;

		private TigerHashingVariant variant = TigerHashingVariant.Tiger;

		private ulong state0 = 0x0123456789ABCDEF;
		private ulong state1 = 0xFEDCBA9876543210;
		private ulong state2 = 0xF096A5B4C3B2E187;

		private static readonly int[] ValidHashSizes = { 128, 160, 192 };

		/// <summary>
		/// Gets the fully qualified algorithm name, including round configuration and hash output size.
		/// </summary>
		/// <remarks>
		/// Follows the convention "SipHash-c-d-x", where:
		/// <list type="bullet">
		/// <item>
		/// <description><c>c</c>: compression rounds</description>
		/// </item>
		/// <item>
		/// <description><c>d</c>: finalization rounds</description>
		/// </item>
		/// <item>
		/// <description><c>x</c>: output hash size in bits</description>
		/// </item>
		/// </list>
		/// </remarks>
		public string AlgorithmName =>
			$"Tiger/{HashSizeValue}";

		/// <summary>
		/// Initializes a new instance of the <see cref="Tiger" /> class with a 192-bit output hash size.
		/// </summary>
		public Tiger() : this(192) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Tiger" /> class with the specified output size.
		/// </summary>
		/// <param name="hashSize">The desired output size in bits. Must be one of 128, 160, or 192.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="hashSize" /> is not valid.</exception>
		public Tiger(int hashSize)
			: base(64)
		{
			if (Array.IndexOf(ValidHashSizes, hashSize) == -1)
				throw new ArgumentOutOfRangeException(nameof(hashSize),
					string.Format(ResourceStrings.CryptographicException_InvalidHashSize, hashSize, string.Join(", ", ValidHashSizes)));

			this.HashSizeValue = hashSize;
		}

		/// <summary>
		/// Gets or sets the Tiger variant to use when computing the hash value.
		/// </summary>
		/// <remarks>
		/// <para>The <see cref="TigerHashingVariant" /> determines the padding byte used in the final message block:</para>
		/// <list type="bullet">
		/// <item>
		/// <description><see cref="TigerHashingVariant.Tiger" /> uses a padding byte of <c>0x01</c> as per the original Tiger specification.</description>
		/// </item>
		/// <item>
		/// <description>
		/// <see cref="TigerHashingVariant.Tiger2" /> uses a padding byte of <c>0x80</c> as introduced in the Tiger2 variant to match
		/// typical Merkle–Damgård padding semantics.
		/// </description>
		/// </item>
		/// </list>
		/// <para>The variant must be specified before hash computation begins. Changing it after processing has started will throw an exception.</para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the assigned value is not a valid <see cref="TigerHashingVariant" /> enumeration value.
		/// </exception>
		/// <exception cref="ObjectDisposedException">Thrown if the hash algorithm instance has already been disposed.</exception>
		/// <exception cref="CryptographicUnexpectedOperationException">
		/// Thrown if the hash algorithm has already started processing input and is in an immutable state.
		/// </exception>
		public TigerHashingVariant Variant
		{
			get
			{
				ThrowIfDisposed();
				return variant;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				ThrowHelper.ThrowIfEnumValueIsUndefined(value);

				variant = value;
			}
		}

		/// <summary>
		/// Gets or sets the size, in bits, of the final computed hash output.
		/// </summary>
		/// <remarks>
		/// Valid values are <c>128</c>, <c>160</c>, or <c>192</c>. This determines how many bits of the internal state are returned in the
		/// final digest. Larger sizes increase output strength but may reduce compatibility with some Tiger implementations.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the value is not 128, 160, or 192.</exception>
		/// <exception cref="ObjectDisposedException">The algorithm instance has been disposed.</exception>
		/// <exception cref="CryptographicUnexpectedOperationException">Thrown if the hash computation has already started.</exception>
		public new int HashSize
		{
			get
			{
				ThrowIfDisposed();
				return this.HashSizeValue;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();

				if (Array.IndexOf(ValidHashSizes, value) == -1)
					throw new ArgumentOutOfRangeException(nameof(value),
						string.Format(ResourceStrings.CryptographicException_InvalidHashSize, value, string.Join(", ", ValidHashSizes)));

				this.HashSizeValue = value;
			}
		}

		/// <inheritdoc />
		public override void Initialize()
		{
			ThrowIfDisposed();
			base.Initialize();
#if !NET6_0_OR_GREATER
			this.State = 0;
			this.finalized = false;
#endif
			state0 = 0x0123456789ABCDEF;
			state1 = 0xFEDCBA9876543210;
			state2 = 0xF096A5B4C3B2E187;
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (disposed) return;
			if (disposing)
			{
				CryptoUtilities.ClearAndNullify(ref HashValue);

				state0 = state1 = state2 = 0;
			}
			disposed = true;
			base.Dispose(disposing);
		}

		/// <inheritdoc />
		protected override byte[] PadBlock(ReadOnlySpan<byte> block, ulong messageLength)
		{
			int inputLength = block.Length;
			bool needsSecondBlock = inputLength >= BlockSizeBytes - 8;
			int totalLength = needsSecondBlock ? BlockSizeBytes * 2 : BlockSizeBytes;

			// Use stackalloc if small enough; fallback to heap if larger
			Span<byte> padded = totalLength <= 128
				? stackalloc byte[totalLength]
				: new byte[totalLength];

			// Copy input to padded buffer
			block.CopyTo(padded);

			// Write padding byte depending on Tiger variant
			padded[inputLength] = variant == TigerHashingVariant.Tiger ? (byte)0x01 : (byte)0x80;

			// Clear bytes between padding and message length field
			padded.Slice(inputLength + 1, totalLength - inputLength - 1 - 8).Clear();

			// Append message length in bits in little-endian at end
			ulong bitLength = messageLength * 8;
			BinaryPrimitives.WriteUInt64LittleEndian(padded.Slice(totalLength - 8), bitLength);

			return padded.Slice(0, totalLength).ToArray();
		}

		/// <summary>
		/// Finalizes the hash computation and produces the output hash value.
		/// </summary>
		/// <returns>A byte array containing the final hash value (8 or 16 bytes).</returns>
		/// <remarks>Combines all partial input and applies the finalization round logic based on the configured output size.</remarks>
		protected override byte[] ProcessFinalBlock()
		{
			Span<byte> output = stackalloc byte[MaxOutputBits / 8];
			BinaryPrimitives.WriteUInt64LittleEndian(output[0..8], state0);
			BinaryPrimitives.WriteUInt64LittleEndian(output[8..16], state1);
			BinaryPrimitives.WriteUInt64LittleEndian(output[16..24], state2);

			return output.Slice(0, HashSizeValue / 8).ToArray();
		}

		/// <summary>
		/// Applies the Tiger key schedule to mutate the block before subsequent passes.
		/// </summary>
		private static void KeySchedule(Span<ulong> x)
		{
			x[0] -= x[7] ^ 0xA5A5A5A5A5A5A5A5UL;
			x[1] ^= x[0];
			x[2] += x[1];
			x[3] -= x[2] ^ (~x[1] << 19);
			x[4] ^= x[3];
			x[5] += x[4];
			x[6] -= x[5] ^ (~x[4] >> 23);
			x[7] ^= x[6];
			x[0] += x[7];
			x[1] -= x[0] ^ (~x[7] << 19);
			x[2] ^= x[1];
			x[3] += x[2];
			x[4] -= x[3] ^ (~x[2] >> 23);
			x[5] ^= x[4];
			x[6] += x[5];
			x[7] -= x[6] ^ 0x0123456789ABCDEFUL;
		}

		/// <inheritdoc />
		protected override void ProcessBlock(ReadOnlySpan<byte> block)
		{
			Span<ulong> blockWords = stackalloc ulong[8];
			MemoryMarshal.Cast<byte, ulong>(block).CopyTo(blockWords);
			TransformBlock(blockWords);
		}

		/// <summary>
		/// Transforms the state using the Tiger compression function.
		/// </summary>
		/// <param name="blockWords">The 512-bit input block represented as 8 x 64-bit words.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void TransformBlock(Span<ulong> blockWords)
		{
			ulong a = state0, b = state1, c = state2;

			DoPass(ref a, ref b, ref c, blockWords, 5);
			KeySchedule(blockWords);
			DoPass(ref c, ref a, ref b, blockWords, 7);
			KeySchedule(blockWords);
			DoPass(ref b, ref c, ref a, blockWords, 9);

			this.state0 = a ^ this.state0;
			this.state1 = b - this.state1;
			this.state2 = c + this.state2;
		}

		/// <summary>
		/// Applies one full "pass" of eight Tiger mixing rounds with the specified multiplier.
		/// </summary>
		private static void DoPass(ref ulong a, ref ulong b, ref ulong c, ReadOnlySpan<ulong> x, int mul)
		{
			Round(ref a, ref b, ref c, x[0], mul);
			Round(ref b, ref c, ref a, x[1], mul);
			Round(ref c, ref a, ref b, x[2], mul);
			Round(ref a, ref b, ref c, x[3], mul);
			Round(ref b, ref c, ref a, x[4], mul);
			Round(ref c, ref a, ref b, x[5], mul);
			Round(ref a, ref b, ref c, x[6], mul);
			Round(ref b, ref c, ref a, x[7], mul);
		}

		/// <summary>
		/// Performs a single Tiger mixing round on three state values and one message word.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Round(ref ulong a, ref ulong b, ref ulong c, ulong x, int mul)
		{
			var tmp = c ^= x;

			a -=
				SBox0[(byte)(tmp >> 0)] ^
				SBox1[(byte)(tmp >> 16)] ^
				SBox2[(byte)(tmp >> 32)] ^
				SBox3[(byte)(tmp >> 48)];

			tmp = b +=
				SBox3[(byte)(tmp >> 8)] ^
				SBox2[(byte)(tmp >> 24)] ^
				SBox1[(byte)(tmp >> 40)] ^
				SBox0[(byte)(tmp >> 56)];

			b = tmp * (ulong)mul;
		}
	}
}