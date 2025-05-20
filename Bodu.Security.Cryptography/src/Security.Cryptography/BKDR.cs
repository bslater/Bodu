// ---------------------------------------------------------------------------------------------------------------
// <copyright file="BKDR.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------
using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

using Bodu.Extensions;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes the hash for the input data using the <see cref="BKDR" /> hash algorithm.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="BKDR" /> class implements a simple non-cryptographic hash algorithm described by Brian Kernighan and Dennis Ritchie
	/// in "The C Programming Language". It is widely used for string hashing in data indexing and lookup scenarios.
	/// </para>
	/// <para>
	/// This implementation uses a multiplicative scheme based on selectable seed values. The algorithm iteratively computes: <c>hash =
	/// (hash * seed) + c</c> where <c>c</c> is the current byte.
	/// </para>
	/// <para>
	/// Supported seeds include: 31, 131, 1313, 13131, 131313, 1313131, 13131313, 131313131, 1313131313. These values follow a pattern
	/// derived from alternating 1s and 3s to tune the distribution.
	/// </para>
	/// <note type="important">This algorithm is <b>not</b> cryptographically secure and must <b>not</b> be used for password hashing,
	/// digital signatures, or secure data integrity validation.</note>
	/// </remarks>
	public sealed class BKDR
		: System.Security.Cryptography.HashAlgorithm
	{
		/// <summary>
		/// Represents the default seed value used by the <see cref="BKDR" /> hash algorithm.
		/// </summary>
		public const uint DefaultSeed = 131U;

		private static readonly uint[] ValidSeedValues = new[]
		{
			31U, 131U, 1313U, 13131U, 131313U, 1313131U, 13131313U, 131313131U, 1313131313U
		};

		private uint seedValue;
		private uint hashValue;
		private bool disposed = false;
#if !NET6_0_OR_GREATER
		private bool finalized;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="BKDR" /> class.
		/// </summary>
		public BKDR()
		{
			seedValue = DefaultSeed;
			HashSizeValue = 32;
			Initialize();
		}

		/// <summary>
		/// Gets or sets the seed value used in the BKDR hash algorithm.
		/// </summary>
		/// <value>The seed value. Must be one of the supported seed constants.</value>
		/// <exception cref="ObjectDisposedException">Thrown when accessing after disposal.</exception>
		/// <exception cref="CryptographicUnexpectedOperationException">Thrown when modified after hashing has started.</exception>
		/// <exception cref="ArgumentException">Thrown when the seed value is not supported.</exception>
		public uint Seed
		{
			get
			{
				ThrowIfDisposed();
				return seedValue;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();

				if (Array.IndexOf(ValidSeedValues, value) == -1)
					throw new ArgumentException(
						string.Format(ResourceStrings.CryptographicException_InvalidPropertyValue, nameof(Seed)), nameof(value));

				seedValue = value;
				Initialize();
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
#if !NET6_0_OR_GREATER
			State = 0;
			finalized = false;
#endif
			hashValue = seedValue;
		}

		/// <inheritdoc />
		/// <summary>
		/// Processes a block of data by feeding it into the <see cref="BKDR" /> algorithm.
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
			ThrowHelper.ThrowIfArrayLengthIsInsufficient(array, ibStart, cbSize);
			if (finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
#endif
			HashCore(array.AsSpan(ibStart, cbSize));
		}

		/// <summary>
		/// Processes a block of data by feeding it into the <see cref="BKDR" /> algorithm.
		/// </summary>
		/// <param name="source">
		/// The input data to process. This method consumes the entire span and updates the internal checksum state accordingly.
		/// </param>
		protected override void HashCore(ReadOnlySpan<byte> source)
		{
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
			if (finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
#endif
			uint v = hashValue;
			foreach (var b in source)
			{
				v = (v * seedValue) + b;
			}
			hashValue = v;
		}

		/// <inheritdoc />
		/// <summary>
		/// Finalizes the <see cref="BKDR" /> hash computation after all input data has been processed, and returns the resulting hash value
		/// as a byte array.
		/// </summary>
		/// <returns>
		/// A byte array representing the finalized Bernstein hash. The length corresponds to the configured
		/// <see cref="HashAlgorithm.HashSize" />, which is 4 bytes for this 32-bit implementation. The returned hash is encoded in
		/// <b>big-endian</b> byte order to ensure platform-independent consistency.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method computes the final hash by applying any remaining state transformations and serializing the internal 32-bit hash
		/// value into a standardized byte array. The hash reflects all data previously supplied through calls to
		/// <see cref="HashCore(byte[], int, int)" /> or <see cref="HashCore(ReadOnlySpan{byte})" />.
		/// </para>
		/// </remarks>
		protected override byte[] HashFinal()
		{
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
			if (finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
			finalized = true;
			State = 2;
#endif
			Span<byte> span = stackalloc byte[4];
			BinaryPrimitives.WriteUInt32BigEndian(span, hashValue);
			return span.ToArray();
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (disposed) return;
			if (disposing)
			{
				hashValue = 0;
			}
			disposed = true;
			base.Dispose(disposing);
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
			if (State != 0)
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
			ObjectDisposedException.ThrowIf(disposed, this);
#else
			if (disposed)
				throw new ObjectDisposedException(nameof(BKDR));
#endif
		}
	}
}