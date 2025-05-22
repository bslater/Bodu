// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Fletcher64.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Bodu.Extensions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Base class for computing hash using the <c>Fletcher</c> hash algorithm family (Fletcher-16, Fletcher-32, Fletcher-64). This class
	/// cannot be inherited.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The Fletcher algorithm is used to generate non-cryptographic hash values for a given byte sequence. It operates by computing two
	/// components (partA and partB) over the input data and produces a hash based on these.
	/// </para>
	/// <para>
	/// This implementation handles Fletcher hash sizes of 16, 32, and 64 bits. Derived classes (see <see cref="Fletcher16" />,
	/// <see cref="Fletcher32" />, <see cref="Fletcher64" />) can implement specific hash sizes.
	/// </para>
	/// <note type="important">This algorithm is <b>not</b> cryptographically secure and should <b>not</b> be used for digital signatures,
	/// password hashing, or integrity verification in security-sensitive contexts.</note>
	/// </remarks>
	public abstract class Fletcher
		: System.Security.Cryptography.HashAlgorithm
	{
		private static readonly int[] ValidHashSizes = { 16, 32, 64 };

		private readonly ulong modulus;
		private readonly int wordSize;
		private readonly int blockSize;
		private ulong partA;
		private ulong partB;
		private readonly Memory<byte> residualByteBuffer;  // To hold residual bytes
		private int residualBytes;
		private bool disposed = false;
#if !NET6_0_OR_GREATER

		// Required for .NET Standard 2.0 or older frameworks
		private bool finalized;
#endif

		/// <inheritdoc />
		public override int InputBlockSize => this.blockSize;

		/// <inheritdoc />
		public override int OutputBlockSize => this.blockSize;

		/// <summary>
		/// Initializes a new instance of the <see cref="Fletcher" /> class with the specified hash size.
		/// </summary>
		/// <param name="hashSize">The hash size in bits. Valid values are 16, 32, or 64.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="hashSize" /> is not 16, 32, or 64.</exception>
		protected Fletcher(int hashSize)
		{
			if (!ValidHashSizes.Contains(hashSize))
				throw new ArgumentException(
					string.Format(ResourceStrings.CryptographicException_InvalidHashSize, hashSize, string.Join(", ", ValidHashSizes)),
					nameof(hashSize));

			this.HashSizeValue = hashSize;
			this.wordSize = hashSize / 16; // Word size for Fletcher algorithm
			this.blockSize = hashSize / 8; // Block size for Fletcher algorithm
			this.modulus = (1UL << (hashSize / 2)) - 1;
			this.residualByteBuffer = new Memory<byte>(new byte[this.wordSize]);
			this.partA = this.partB = 0;
			this.residualBytes = 0;
		}

		/// <summary>
		/// Gets the fully qualified algorithm name for this Fletcher variant, based on the configured hash output size.
		/// </summary>
		/// <value>
		/// A string in the form <c>Fletcher-N</c>, where <c>N</c> is the number of bits in the final hash output (e.g., <c>Fletcher-32</c>
		/// or <c>Fletcher-64</c>).
		/// </value>
		/// <remarks>
		/// The naming convention follows the established Fletcher standard, where the suffix indicates the bit-width of the resulting checksum:
		/// </remarks>
		public string AlgorithmName =>
			$"Fletcher-{HashSizeValue}";

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (this.disposed) return;

			if (disposing)
			{
				CryptoUtilities.ClearAndNullify(ref HashValue);
				CryptoUtilities.Clear(residualByteBuffer);

				this.partA = this.partB = 0;
				this.residualBytes = 0;
			}

			this.disposed = true;
			base.Dispose(disposing);
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

		/// <summary>
		/// Initializes the internal state of the hash algorithm.
		/// </summary>
		public override void Initialize()
		{
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
			this.State = 0;
			this.finalized = false;
#endif
			this.partA = 0;
			this.partB = 0;
			this.residualByteBuffer.Span.Clear();
			this.residualBytes = 0;
		}

		/// <inheritdoc />
		/// <summary>
		/// Processes a block of data by feeding it into the <see cref="Fletcher" /> algorithm.
		/// </summary>
		/// <param name="array">The byte array containing the data to be hashed.</param>
		/// <param name="ibStart">The offset at which to start processing in the byte array.</param>
		/// <param name="cbSize">The length of the data to process.</param>
		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			ThrowHelper.ThrowIfNull(array);
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
			ThrowHelper.ThrowIfLessThan(ibStart, 0);
			ThrowHelper.ThrowIfLessThan(cbSize, 0);
			ThrowHelper.ThrowIfArrayLengthIsInsufficient(array, offset, cbSize);
			if (this.finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
#endif

			this.ProcessBlocks(array.AsSpan(), ibStart, cbSize);
		}

		/// <inheritdoc />
		/// <summary>
		/// Finalizes the <see cref="Fletcher" /> checksum computation after all input data has been processed, and returns the resulting
		/// hash value.
		/// </summary>
		/// <returns>
		/// A byte array containing the Fletcher result. The length depends on the <see cref="HashAlgorithm.HashSize" /> setting:
		/// <list type="bullet">
		/// <item>
		/// <description><c>HashSize = 32</c>: Returns a 4-byte Fletcher-32 checksum</description>
		/// </item>
		/// <item>
		/// <description><c>HashSize = 64</c>: Returns an 8-byte Fletcher-64 checksum</description>
		/// </item>
		/// </list>
		/// </returns>
		/// <remarks>
		/// The hash reflects all data previously supplied via <see cref="HashCore(byte[], int, int)" />. Once finalized, the internal state
		/// is invalidated and <see cref="HashAlgorithm.Initialize" /> must be called before reusing the instance.
		/// </remarks>
		protected override byte[] HashFinal()
		{
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
			if (this.finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);

			this.finalized = true;
			this.State = 2;
#endif

			// Process any residual data if necessary
			if (this.residualBytes > 0)
			{
				this.ProcessBlock(this.residualByteBuffer.Span);
				this.residualBytes = 0;
				this.residualByteBuffer.Span.Clear();
			}

			// Combine partA and partB into the final hash value
			ulong finalHash = (this.partA << (this.HashSize / 2)) | this.partB;

			// Convert to a byte array and take the size
			return finalHash.GetBytes().SliceInternal(0, this.blockSize);
		}

		/// <summary>
		/// Processes blocks of data for hashing. It handles both full and residual blocks.
		/// </summary>
		/// <param name="buffer">The data buffer.</param>
		/// <param name="offset">The starting index in the buffer.</param>
		/// <param name="length">The length of the data to process.</param>
		private void ProcessBlocks(Span<byte> buffer, int offset, int length)
		{
			int pos = offset;

			// If there is any residual data, handle it first
			if (this.residualBytes > 0)
			{
				int remainingLength = this.wordSize - this.residualBytes;
				if (remainingLength <= length)
				{
					// Fill the buffer to make a full block
					buffer.Slice(offset, remainingLength).CopyTo(this.residualByteBuffer.Span.Slice(this.residualBytes));
					this.ProcessBlock(this.residualByteBuffer.Span);
					this.residualBytes = 0;
					this.residualByteBuffer.Span.Clear();
					pos += remainingLength;
				}
				else
				{
					buffer.Slice(offset, length).CopyTo(this.residualByteBuffer.Span.Slice(this.residualBytes));
					this.residualBytes += length;
					return;
				}
			}

			// Process full blocks in the buffer
			int end = offset + length - this.wordSize;
			for (; pos <= end; pos += this.wordSize)
			{
				this.ProcessBlock(buffer.Slice(pos, this.wordSize));
			}

			// Handle residual bytes (partial block at the end)
			this.residualBytes = (this.wordSize + end - pos) % this.wordSize;
			buffer.Slice(pos, this.residualBytes).CopyTo(this.residualByteBuffer.Span);
		}

		/// <summary>
		/// Processes a single block of data and updates the internal state.
		/// </summary>
		/// <param name="buffer">The data buffer.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ProcessBlock(Span<byte> buffer)
		{
			// Process the current block using loop unrolling for word size of 8
			ulong block = 0;
			for (int i = 0; i < this.wordSize; i++)
			{
				block |= ((ulong)buffer[i]) << ((this.wordSize - (i + 1)) << 3);
			}

			// Update the internal state (partA and partB)
			this.partA = (this.partA + block) % this.modulus;
			this.partB = (this.partB + this.partA) % this.modulus;
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
	}
}