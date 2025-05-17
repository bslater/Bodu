// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Adler32.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Bodu.Extensions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Provides a common base implementation for Adler-32-style checksum algorithms.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Adler-32 is a fast, non-cryptographic checksum algorithm developed by Mark Adler for use in the zlib compression library. It
	/// produces a 32-bit checksum composed of two running sums (A and B) combined into a single value. Variants of the algorithm differ in
	/// the modulus used for summation.
	/// </para>
	/// <para>
	/// This abstract base class supports reuse and multiple block processing, and can be extended to implement specific Adler-32 variants.
	/// Concrete implementations include:
	/// <list type="bullet">
	/// <item>
	/// <term><see cref="Adler32" /></term>
	/// <description>The standard Adler-32 algorithm using modulo 65521.</description>
	/// </item>
	/// <item>
	/// <term><see cref="Adler32C" /></term>
	/// <description>An optimized variant using modulo 65536 for SIMD alignment.</description>
	/// </item>
	/// </list>
	/// </para>
	/// <note type="important">This algorithm family is <b>not</b> cryptographically secure and should <b>not</b> be used for password
	/// hashing, digital signatures, or integrity verification in security-critical contexts.</note>
	/// </remarks>
	public abstract class Adler
		: System.Security.Cryptography.HashAlgorithm
	{
		/// <summary>
		/// The standard Adler-32 modulus (65521), used in the original zlib implementation.
		/// </summary>
		protected const uint Adler32_Modulo = 65521; // Largest prime smaller than 2^16

		/// <summary>
		/// The Adler-32C modulus (65536), used for performance-oriented implementations with SIMD alignment.
		/// </summary>
		protected const uint Adler32C_Modulo = 65536; // 2^16, used in vectorized variants

		private readonly uint modulo;
		private uint partA;
		private uint partB;
		private bool disposed;
#if !NET6_0_OR_GREATER

		// Required for .NET Standard 2.0 or older frameworks
		private bool finalized;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="Adler" /> class.
		/// </summary>
		protected Adler(uint modulo)
		{
			this.modulo = modulo;
			this.HashSizeValue = 32;
			this.partA = 1;
			this.partB = 0;
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
		/// Initializes or resets the internal state for a new checksum computation.
		/// </summary>
		public override void Initialize()
		{
#if !NET6_0_OR_GREATER
			this.State = 0;
			this.finalized = false;
#endif
			ThrowIfDisposed();
			this.partA = 1;
			this.partB = 0;
		}

		/// <inheritdoc />
		/// <summary>
		/// Processes a block of data by feeding it into the <see cref="Adler" /> algorithm.
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

			const int NMAX = 5552;
			while (cbSize > 0)
			{
				int chunkSize = System.Math.Min(cbSize, NMAX);
				for (int i = 0; i < chunkSize; i++)
				{
					this.partA += array[ibStart++];
					this.partB += this.partA;
				}

				this.partA %= this.modulo;
				this.partB %= this.modulo;
				cbSize -= chunkSize;
			}
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (this.disposed) return;

			if (disposing)
			{
				this.partA = this.partB = 0;
			}

			this.disposed = true;
			base.Dispose(disposing);
		}

		/// <inheritdoc />
		/// <summary>
		/// Finalizes the <see cref="Adler" /> checksum computation after all input data has been processed, and returns the resulting hash value.
		/// </summary>
		/// <returns>
		/// A byte array containing the Adler-32 result. The length is always 4 bytes, representing a 32-bit checksum composed of the
		/// combined values of the two internal accumulators.
		/// </returns>
		/// <remarks>
		/// The hash reflects all data previously supplied via <see cref="HashCore(byte[], int, int)" />. Once finalized, the internal state
		/// is invalidated and <see cref="HashAlgorithm.Initialize" /> must be called before reusing the instance.
		/// </remarks>
		protected override byte[] HashFinal()
		{
#if !NET6_0_OR_GREATER
			if (this.finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);

			this.finalized = true;
			this.State = 2;
#endif
			ThrowIfDisposed();

			uint hash = (this.partB << 16) | this.partA;
			return hash.GetBytes(asBigEndian: true);
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
				throw new ObjectDisposedException(nameof(Adler));
#endif
		}
	}
}