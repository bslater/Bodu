// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Fletcher64.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Bodu.Extensions;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Provides the base implementation of the <see cref="SipHash" /> cryptographic hash algorithm - a fast, secure, and keyed pseudorandom
	/// function optimized for short input messages. See the <see href="https://131002.net/siphash/">official SipHash specification</see>
	/// for details.
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
	/// <description><see cref="SipHash64" /> -  Produces a 64-bit hash output suitable for compact keyed checksums.</description>
	/// </item>
	/// <item>
	/// <description><see cref="SipHash128" /> - Produces a 128-bit hash output offering increased collision resistance.</description>
	/// </item>
	/// </list>
	/// <para>The algorithm proceeds in two primary phases:</para>
	/// <list type="number">
	/// <item>
	/// <description>
	/// <b>Compression:</b> Each 64-bit block of the input is mixed into the internal state using a configurable number of rounds ( <see cref="CompressionRounds" />).
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// <b>Finalization:</b> After processing all input, additional rounds ( <see cref="FinalizationRounds" />) are applied to produce the
	/// final hash output.
	/// </description>
	/// </item>
	/// </list>
	/// <note type="important"> This algorithm is <b>not</b> suitable for cryptographic applications such as password hashing, digital
	/// signatures, or secure data integrity checks. </note>
	/// </remarks>
	public abstract class SipHash
		: System.Security.Cryptography.KeyedHashAlgorithm
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
		private ulong length;
		private int residualBytes;
		private ulong v0, v1, v2, v3;
		private Memory<byte> residualByteBuffer;
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="SipHash" /> class with a specified hash size.
		/// </summary>
		/// <param name="hashSize">The desired size of the final hash in bits. Supported values are 64 or 128.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="hashSize" /> is not supported.</exception>
		protected SipHash(int hashSize)
		{
			if (Array.IndexOf(ValidHashSizes, hashSize) == -1)
				throw new ArgumentException($"Invalid hash size {hashSize}. Valid hash sizes are: {string.Join(", ", ValidHashSizes)}", nameof(hashSize));

			this.KeyValue = new byte[KeySize];
			CryptoUtilities.FillWithRandomNonZeroBytes(this.KeyValue);
			this.compressionRounds = MinCompressionRounds;
			this.finalizationRounds = MinFinalizationRounds;
			this.HashSizeValue = hashSize;
			this.residualByteBuffer = new Memory<byte>(new byte[BlockSize]);
			this.length = 0;
			this.InitializeVectors();
		}

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
				this.ThrowIfDisposed();
				return this.compressionRounds;
			}

			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfInvalidState();
				ThrowHelper.ThrowIfLessThan(value, MinCompressionRounds);

				this.compressionRounds = value;
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
				this.ThrowIfDisposed();
				return this.finalizationRounds;
			}

			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfInvalidState();
				ThrowHelper.ThrowIfLessThan(value, MinFinalizationRounds);

				this.finalizationRounds = value;
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
				this.ThrowIfDisposed();
				return this.KeyValue.Copy();
			}

			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfInvalidState();
				ThrowHelper.ThrowIfNull(value);
				if (value.Length != KeySize)
					throw new CryptographicException(string.Format(ResourceStrings.CryptographicException_InvalidKeySize, value.Length, SipHash.KeySize));

				this.KeyValue = value.Copy();
				this.InitializeVectors();
			}
		}

		/// <inheritdoc />
		public override void Initialize()
		{
			this.ThrowIfDisposed();
#if !NET6_0_OR_GREATER
            this.State = 0;
            this.finalized = false;
#endif
			this.residualByteBuffer.Span.Clear();
			this.length = 0;
			this.residualBytes = 0;
			this.InitializeVectors();
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (this.disposed) return;

			if (disposing)
			{
				this.residualByteBuffer.Span.Clear();
				this.v0 = this.v1 = this.v2 = this.v3 = 0;
				this.length = 0;
				this.residualBytes = 0;

				if (this.KeyValue is not null)
				{
					CryptographicOperations.ZeroMemory(this.KeyValue);
					this.KeyValue = null;
				}
			}

			this.disposed = true;
			base.Dispose(disposing);
		}

		/// <inheritdoc />
		protected override void HashCore(byte[] buffer, int offset, int length)
		{
			this.ThrowIfDisposed();
			this.length += (ulong)length;
			this.ProcessBlocks(buffer, offset, length);
		}

		/// <inheritdoc />
		protected override byte[] HashFinal()
		{
			this.ThrowIfDisposed();
			return this.ProcessFinalBlock();
		}

		/// <summary>
		/// Throws a <see cref="CryptographicUnexpectedOperationException" /> if the hash algorithm has already started processing data,
		/// indicating that the instance is in a finalized or non-configurable state.
		/// </summary>
		/// <remarks>
		/// This method is used to prevent reconfiguration of algorithm parameters such as the key, number of rounds, or other settings once
		/// hashing has begun. It ensures settings are immutable after initialization.
		/// </remarks>
		/// <exception cref="CryptographicUnexpectedOperationException">
		/// Thrown when an attempt is made to modify the algorithm after it has entered a non-zero state, which indicates that hashing has
		/// started or been finalized.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfInvalidState()
		{
			if (this.State != 0)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_ReconfigurationNotAllowed);
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException" /> if the algorithm instance has been disposed.
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Thrown when any public method or property is accessed after the instance has been disposed.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfDisposed()
		{
#if NET8_0_OR_GREATER
			ObjectDisposedException.ThrowIf(this.disposed, this);
#else
			if (this.disposed)
				throw new ObjectDisposedException(nameof(SipHash));
#endif
		}

		/// <summary>
		/// Initializes the internal SipHash state vectors based on the current key value and hash size.
		/// </summary>
		/// <remarks>This method XORs the <see cref="Key" /> with predefined constants to initialize the internal state.</remarks>
		private void InitializeVectors()
		{
			ulong k0 = BitConverter.ToUInt64(this.KeyValue, 0);
			ulong k1 = BitConverter.ToUInt64(this.KeyValue, 8);
			this.v0 = InitialStates[0] ^ k0;
			this.v1 = InitialStates[1] ^ k1;
			this.v2 = InitialStates[2] ^ k0;
			this.v3 = InitialStates[3] ^ k1;

			if (this.HashSizeValue == 128) this.v1 ^= 0xee;
		}

		/// <summary>
		/// Performs a fixed number of SipHash compression or finalization rounds on the internal state.
		/// </summary>
		/// <param name="iterations">The number of rounds to perform.</param>
		/// <remarks>Each round consists of multiple bitwise operations and rotations defined by the SipHash specification.</remarks>
		private void PerformSipRounds(int iterations)
		{
			for (int i = 0; i < iterations; i++)
			{
				this.v0 += this.v1;
				this.v1 = (this.v1 << 13) | (this.v1 >> (64 - 13));
				this.v1 ^= this.v0;
				this.v0 = (this.v0 << 32) | (this.v0 >> (64 - 32));
				this.v2 += this.v3;
				this.v3 = (this.v3 << 16) | (this.v3 >> (64 - 16));
				this.v3 ^= this.v2;
				this.v0 += this.v3;
				this.v3 = (this.v3 << 21) | (this.v3 >> (64 - 21));
				this.v3 ^= this.v0;
				this.v2 += this.v1;
				this.v1 = (this.v1 << 17) | (this.v1 >> (64 - 17));
				this.v1 ^= this.v2;
				this.v2 = (this.v2 << 32) | (this.v2 >> (64 - 32));
			}
		}

		/// <summary>
		/// Processes one or more blocks of input data into the SipHash internal state.
		/// </summary>
		/// <param name="buffer">The byte array containing the input data.</param>
		/// <param name="offset">The byte offset in the buffer to start reading from.</param>
		/// <param name="length">The number of bytes to process.</param>
		/// <remarks>Handles buffering of partial blocks and invokes <see cref="ProcessBlock" /> for each complete block.</remarks>
		private void ProcessBlocks(byte[] buffer, int offset, int length)
		{
			int pos = offset;

			if (this.residualBytes > 0)
			{
				int remainingLength = BlockSize - this.residualBytes;
				if (remainingLength <= length)
				{
					new Span<byte>(buffer, offset, remainingLength).CopyTo(this.residualByteBuffer.Span.Slice(this.residualBytes));
					ulong block = BitConverter.ToUInt64(this.residualByteBuffer.Span.Slice(0, BlockSize));
					this.ProcessBlock(block);
					this.residualBytes = 0;
					pos += remainingLength;
				}
				else
				{
					new Span<byte>(buffer, offset, length).CopyTo(this.residualByteBuffer.Span.Slice(this.residualBytes));
					this.residualBytes += length;
					return;
				}
			}

			int end = offset + length - BlockSize;
			for (; pos <= end; pos += BlockSize)
			{
				ulong block = BitConverter.ToUInt64(buffer, pos);
				this.ProcessBlock(block);
			}

			this.residualBytes = (BlockSize + end - pos) % BlockSize;
			new Span<byte>(buffer, pos, this.residualBytes).CopyTo(this.residualByteBuffer.Span);
		}

		/// <summary>
		/// Processes a single 64-bit block of data using SipHash compression.
		/// </summary>
		/// <param name="block">The 64-bit block to process.</param>
		/// <remarks>Updates internal state using <see cref="PerformSipRounds" /> and XOR operations.</remarks>
		private void ProcessBlock(ulong block)
		{
			this.v3 ^= block;
			this.PerformSipRounds(this.compressionRounds);
			this.v0 ^= block;
		}

		/// <summary>
		/// Finalizes the hash computation and produces the output hash value.
		/// </summary>
		/// <returns>A byte array containing the final hash value (8 or 16 bytes).</returns>
		/// <remarks>Combines all partial input and applies the finalization round logic based on the configured output size.</remarks>
		private byte[] ProcessFinalBlock()
		{
			ulong block = this.length << 56;

			if (this.residualBytes > 0)
			{
				block |= BitConverter.ToUInt64(this.residualByteBuffer.Span.Slice(0, BlockSize));
			}

			this.ProcessBlock(block);

			this.v2 ^= (this.HashSizeValue == 64) ? 0xffUL : 0xeeUL;
			this.PerformSipRounds(this.finalizationRounds);

			byte[] hash = new byte[this.HashSizeValue / 8];
			ulong finalHash = this.v0 ^ this.v1 ^ this.v2 ^ this.v3;
			Array.Copy(BitConverter.GetBytes(finalHash), 0, hash, 0, 8);

			if (this.HashSizeValue == 128)
			{
				this.v1 ^= 0xdd;
				this.PerformSipRounds(this.finalizationRounds);

				finalHash = this.v0 ^ this.v1 ^ this.v2 ^ this.v3;
				Array.Copy(BitConverter.GetBytes(finalHash), 0, hash, 8, 8);
			}

			return hash;
		}
	}
}