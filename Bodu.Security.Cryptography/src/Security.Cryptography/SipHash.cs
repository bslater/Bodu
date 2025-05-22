// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Fletcher64.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Bodu.Extensions;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Provides the base implementation of the <c>SipHash</c> cryptographic hash algorithm - a fast, secure, and keyed pseudorandom
	/// function optimized for short input messages. See the official <a href="https://131002.net/siphash/">SipHash specification</a> for details.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="SipHash" /> is a keyed hash function that requires a 128-bit (16-byte) secret key. It operates on short messages using a
	/// sequence of Add-Rotate-XOR (ARX) mixing steps across four 64-bit state variables ( <c>v0</c> through <c>v3</c>). The algorithm is
	/// resistant to hash-flooding attacks and is particularly effective for securing hash tables.
	/// </para>
	/// <para>
	/// This abstract base class defines the core SipHash logic and exposes configuration options such as the number of compression and
	/// finalization rounds. It is extended by:
	/// </para>
	/// <list type="bullet">
	/// <item>
	/// <description><see cref="SipHash64" /> – Produces a 64-bit hash output suitable for compact keyed checksums.</description>
	/// </item>
	/// <item>
	/// <description><see cref="SipHash128" /> – Produces a 128-bit hash output offering increased collision resistance.</description>
	/// </item>
	/// </list>
	/// <para>The algorithm proceeds in two primary phases:</para>
	/// <list type="number">
	/// <item>
	/// <description>
	/// <b>Compression:</b> Each 64-bit block of the input is mixed into the internal state using a configurable number of compression
	/// rounds, as defined by <see cref="CompressionRounds" />.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <b>Finalization:</b> After processing all input, a configurable number of finalization rounds—specified via
	/// <see cref="FinalizationRounds" />—are applied to produce the final hash output.
	/// </description>
	/// </item>
	/// </list>
	/// <note type="important">This algorithm is <b>not</b> suitable for cryptographic applications such as password hashing, digital
	/// signatures, or secure data integrity checks.</note>
	/// </remarks>
	public abstract class SipHash<T>
		: KeyedBlockHashAlgorithm<T>
		where T : SipHash<T>, new()
	{
		/// <summary>
		/// The fixed key size in bytes (128 bits).
		/// </summary>
		public const int KeySize = 16;

		/// <summary>
		/// The minimum number of compression rounds required by SipHash.
		/// </summary>
		public const int MinCompressionRounds = 2;

		/// <summary>
		/// The minimum number of finalization rounds required by SipHash.
		/// </summary>
		public const int MinFinalizationRounds = 4;

		private static readonly int BlockSize = 8;
		private static readonly int[] ValidHashSizes = { 64, 128 };

		private static readonly ulong[] InitialStates = new ulong[]
		{
			0x736f6d6570736575UL,
			0x646f72616e646f6dUL,
			0x6c7967656e657261UL,
			0x7465646279746573UL,
		};

		private int compressionRounds;
		private int finalizationRounds;
		private ulong v0, v1, v2, v3;
		private bool disposed = false;
#if !NET6_0_OR_GREATER

		// Required for .NET Standard 2.0 or older frameworks
		private bool finalized;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="SipHash" /> class with a specified hash size.
		/// </summary>
		/// <param name="hashSize">The desired size of the final hash in bits. Supported values are 64 or 128.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="hashSize" /> is not supported.</exception>
		protected SipHash(int hashSize)
			: base(BlockSize)
		{
			if (Array.IndexOf(ValidHashSizes, hashSize) == -1)
				throw new ArgumentOutOfRangeException(nameof(hashSize),
					string.Format(ResourceStrings.CryptographicException_InvalidHashSize, hashSize, string.Join(", ", ValidHashSizes)));

			KeyValue = new byte[KeySize];
			CryptoUtilities.FillWithRandomNonZeroBytes(KeyValue);
			compressionRounds = MinCompressionRounds;
			finalizationRounds = MinFinalizationRounds;
			HashSizeValue = hashSize;
			InitializeVectors();
		}

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
			$"SipHash-{CompressionRounds}-{FinalizationRounds}-{HashSizeValue}";

		/// <summary>
		/// Gets or sets the number of compression rounds performed per message block.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the value is less than <see cref="MinCompressionRounds" />.</exception>
		/// <exception cref="ObjectDisposedException">Instance has been disposed and its members are accessed.</exception>
		/// <exception cref="CryptographicUnexpectedOperationException">The hash computation has already started.</exception>
		public int CompressionRounds
		{
			get
			{
				ThrowIfDisposed();
				return compressionRounds;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				ThrowHelper.ThrowIfLessThan(value, MinCompressionRounds);

				compressionRounds = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of finalization rounds performed after all input has been processed.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the value is less than <see cref="MinFinalizationRounds" />.</exception>
		/// <exception cref="ObjectDisposedException">Instance has been disposed and its members are accessed.</exception>
		/// <exception cref="CryptographicUnexpectedOperationException">The hash computation has already started.</exception>
		public int FinalizationRounds
		{
			get
			{
				ThrowIfDisposed();
				return finalizationRounds;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				ThrowHelper.ThrowIfLessThan(value, MinFinalizationRounds);

				finalizationRounds = value;
			}
		}

		/// <inheritdoc />
		public override int InputBlockSize => BlockSize;

		/// <inheritdoc />
		public override int OutputBlockSize => BlockSize;

		/// <inheritdoc />
		public override byte[] Key
		{
			get
			{
				ThrowIfDisposed();
				return KeyValue.Copy();
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				ThrowHelper.ThrowIfNull(value);
				if (value.Length != KeySize)
					throw new CryptographicException(string.Format(ResourceStrings.CryptographicException_InvalidKeySize, value.Length, SipHash<T>.KeySize));

				KeyValue = value.Copy();
				InitializeVectors();
			}
		}

		/// <inheritdoc />
		/// <summary>
		/// Gets a value indicating whether the current hash algorithm instance can be reused after the hash computation is finalized.
		/// </summary>
		/// <returns><see langword="true" /> if the current instance supports reuse via <see cref="Initialize" />; otherwise, <see langword="false" />.</returns>
		/// <remarks>
		/// When this property returns <see langword="true" />, you may call <see cref="Initialize" /> after computing a hash to reset the
		/// internal state and perform a new hash computation without creating a new instance.
		/// </remarks>
		public override bool CanReuseTransform => true;

		/// <inheritdoc />
		/// <summary>
		/// Gets a value indicating whether multiple blocks can be transformed in a single <see cref="HashCore" /> call.
		/// </summary>
		/// <returns>
		/// <see langword="true" /> if the implementation supports processing multiple blocks in a single operation; otherwise, <see langword="false" />.
		/// </returns>
		/// <remarks>
		/// Most hash algorithms support processing multiple input blocks in a single call to <see cref="HashAlgorithm.TransformBlock" /> or
		/// <see cref="HashAlgorithm.HashCore(byte[], int, int)" />, making this property typically return <see langword="true" />. Override
		/// this to return <see langword="false" /> for algorithms that require strict block-by-block input.
		/// </remarks>
		public override bool CanTransformMultipleBlocks => true;

		/// <inheritdoc />
		public override void Initialize()
		{
			ThrowIfDisposed();
			base.Initialize();
#if !NET6_0_OR_GREATER
            State = 0;
            finalized = false;
#endif
			InitializeVectors();
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (disposed) return;

			if (disposing)
			{
				CryptoUtilities.ClearAndNullify(ref HashValue);

				v0 = v1 = v2 = v3 = 0;
				compressionRounds = finalizationRounds = 0;
			}

			disposed = true;
			base.Dispose(disposing);
		}

		/// <summary>
		/// Initializes the internal SipHash state vectors based on the current key value and hash size.
		/// </summary>
		/// <remarks>This method XORs the <see cref="Key" /> with predefined constants to initialize the internal state.</remarks>
		private void InitializeVectors()
		{
			ulong k0 = BitConverter.ToUInt64(KeyValue, 0);
			ulong k1 = BitConverter.ToUInt64(KeyValue, 8);
			v0 = InitialStates[0] ^ k0;
			v1 = InitialStates[1] ^ k1;
			v2 = InitialStates[2] ^ k0;
			v3 = InitialStates[3] ^ k1;

			if (HashSizeValue == 128) v1 ^= 0xee;
		}

		/// <summary>
		/// Performs a fixed number of SipHash compression or finalization rounds on the internal state.
		/// </summary>
		/// <param name="iterations">The number of rounds to perform.</param>
		/// <remarks>Each round consists of multiple bitwise operations and rotations defined by the SipHash specification.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void PerformSipRounds(int iterations)
		{
			ulong r0 = v0, r1 = v1, r2 = v2, r3 = v3;

			for (int i = 0; i < iterations; i++)
			{
				r0 += r1;
				r1 = BitOperations.RotateLeft(r1, 13);
				r1 ^= r0;
				r0 = BitOperations.RotateLeft(r0, 32);
				r2 += r3;
				r3 = BitOperations.RotateLeft(r3, 16);
				r3 ^= r2;
				r0 += r3;
				r3 = BitOperations.RotateLeft(r3, 21);
				r3 ^= r0;
				r2 += r1;
				r1 = BitOperations.RotateLeft(r1, 17);
				r1 ^= r2;
				r2 = BitOperations.RotateLeft(r2, 32);
			}

			v0 = r0; v1 = r1; v2 = r2; v3 = r3;
		}

		///// <summary>
		///// Processes one or more blocks of input data into the SipHash internal state.
		///// </summary>
		///// <param name="buffer">The byte array containing the input data.</param>
		///// <param name="offset">The byte offset in the buffer to start reading from.</param>
		///// <param name="length">The number of bytes to process.</param>
		///// <remarks>Handles buffering of partial blocks and invokes <see cref="ProcessBlock" /> for each complete block.</remarks>
		//private void ProcessBlocks(byte[] buffer, int offset, int length)
		//{
		//	int pos = offset;
		//	Span<byte> residualSpan = residualByteBuffer.Span;

		// // Handle residual bytes from the previous call if (residualBytes > 0) { int remaining = BlockSize - residualBytes;

		// if (length >= remaining) { // Fill up the buffer and process one full block buffer.AsSpan(pos,
		// remaining).CopyTo(residualSpan[residualBytes..]); ulong block = MemoryMarshal.Read<ulong>(residualSpan); ProcessBlock(block);

		// residualBytes = 0; pos += remaining; } else { // Not enough to complete a block, just append to residuals buffer.AsSpan(pos,
		// length).CopyTo(residualSpan[residualBytes..]); residualBytes += length; return; } }

		// // Process full blocks directly from the input int end = offset + length; while (pos + BlockSize <= end) { ulong block =
		// MemoryMarshal.Read<ulong>(buffer.AsSpan(pos, BlockSize)); ProcessBlock(block); pos += BlockSize; }

		//	// Buffer any remaining residual bytes
		//	residualBytes = end - pos;
		//	if (residualBytes > 0)
		//		buffer.AsSpan(pos, residualBytes).CopyTo(residualSpan);
		//}

		/// <summary>
		/// Processes a single 64-bit block of data using SipHash compression.
		/// </summary>
		/// <param name="block">The 64-bit block to process.</param>
		/// <remarks>Updates internal state using <see cref="PerformSipRounds" /> and XOR operations.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void ProcessBlock(ReadOnlySpan<byte> block)
		{
			var b = BinaryPrimitives.ReadUInt64LittleEndian(block);
			v3 ^= b;
			PerformSipRounds(compressionRounds);
			v0 ^= b;
		}

		protected override byte[] PadBlock(ReadOnlySpan<byte> block, ulong messageLength)
		{
			if ((uint)block.Length > 7)
				throw new ArgumentOutOfRangeException(nameof(block), "Residual block must be 0–7 bytes.");

			Span<byte> buffer = stackalloc byte[8];
			block.CopyTo(buffer);
			buffer[7] = (byte)messageLength;

			return buffer.ToArray();
		}

		/// <summary>
		/// Finalizes the hash computation and produces the output hash value.
		/// </summary>
		/// <returns>A byte array containing the final hash value (8 or 16 bytes).</returns>
		/// <remarks>Combines all partial input and applies the finalization round logic based on the configured output size.</remarks>
		protected override byte[] ProcessFinalBlock()
		{
			v2 ^= (HashSizeValue == 64) ? 0xffUL : 0xeeUL;
			PerformSipRounds(finalizationRounds);

			byte[] hash = new byte[HashSizeValue / 8];

			// First 64-bit output
			ulong h0 = v0 ^ v1 ^ v2 ^ v3;
			MemoryMarshal.Write(hash.AsSpan(0, 8), in h0);

			// Optional second block for SipHash-128
			if (HashSizeValue == 128)
			{
				v1 ^= 0xdd;
				PerformSipRounds(finalizationRounds);

				ulong h1 = v0 ^ v1 ^ v2 ^ v3;
				MemoryMarshal.Write(hash.AsSpan(8, 8), in h1);
			}

			return hash;
		}
	}
}