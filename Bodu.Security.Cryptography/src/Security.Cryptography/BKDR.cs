// ---------------------------------------------------------------------------------------------------------------
// <copyright file="BKDR.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes the hash for the input data using the <c>BKDR</c> hash algorithm. This variant uses a simple polynomial rolling technique
	/// with a fixed seed multiplier to produce fast, non-cryptographic 32-bit hashes. This class cannot be inherited.
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
	/// <note type="important">This algorithm is <b>not</b> cryptographically secure and should <b>not</b> be used for password hashing,
	/// digital signatures, or integrity validation in security-sensitive applications.</note>
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

		private bool disposed = false;
		private uint seedValue;
		private uint workingHash;
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
		/// Gets a value indicating whether this transform instance can be reused after a hash operation is completed.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the transform supports multiple hash computations via <see cref="HashAlgorithm.Initialize" />;
		/// otherwise, <see langword="false" />.
		/// </value>
		/// <remarks>
		/// Reusable transforms allow the internal state to be reset for subsequent operations using the same instance. One-shot algorithms
		/// that clear sensitive key material after finalization typically return <see langword="false" />.
		/// </remarks>
		public override bool CanReuseTransform => true;

		/// <summary>
		/// Gets a value indicating whether this transform supports processing multiple blocks of data in a single operation.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if multiple input blocks can be transformed in sequence without intermediate finalization; otherwise, <see langword="false" />.
		/// </value>
		/// <remarks>
		/// Most hash algorithms and block ciphers support multi-block transformations for streaming input. If <see langword="false" />, the
		/// transform must be invoked one block at a time.
		/// </remarks>
		public override bool CanTransformMultipleBlocks => true;

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
		public override void Initialize()
		{
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
			State = 0;
			finalized = false;
#endif
			workingHash = seedValue;
		}

		/// <summary>
		/// Releases the unmanaged resources used by the algorithm and clears the key from memory.
		/// </summary>
		/// <param name="disposing">
		/// <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.
		/// </param>
		/// <remarks>Ensures all internal secrets are overwritten with zeros before releasing resources.</remarks>
		protected override void Dispose(bool disposing)
		{
			if (disposed) return;
			if (disposing)
			{
				CryptoUtilities.ClearAndNullify(ref HashValue);

				workingHash = seedValue = 0;
			}
			disposed = true;
			base.Dispose(disposing);
		}

		/// <summary>
		/// Processes a segment of the input byte array and feeds it into the <see cref="BKDR" /> hashing algorithm. This method updates the
		/// internal state by processing <paramref name="cbSize" /> bytes starting at the specified <paramref name="ibStart" /> offset.
		/// </summary>
		/// <param name="array">The input byte array containing the data to hash.</param>
		/// <param name="ibStart">The zero-based index in <paramref name="array" /> at which to begin reading data.</param>
		/// <param name="cbSize">The number of bytes to process from <paramref name="array" />.</param>
		/// <exception cref="ArgumentNullException"><paramref name="array" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para><paramref name="ibStart" /> is less than 0.</para>
		/// <para>-or-</para>
		/// <para><paramref name="cbSize" /> is less than 0.</para>
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="ibStart" /> and <paramref name="cbSize" /> specify a range that exceeds the length of <paramref name="array" />.
		/// </exception>
		/// <exception cref="CryptographicUnexpectedOperationException">
		/// The hash algorithm has already been finalized and cannot accept more input data.
		/// </exception>
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
		/// Processes the entirety of the input <paramref name="source" /> and feeds it into the <see cref="BKDR" /> hashing algorithm. This
		/// method updates the internal hash state accordingly by consuming the entire input span.
		/// </summary>
		/// <param name="source">The input byte span containing the data to hash.</param>
		/// <exception cref="CryptographicUnexpectedOperationException">
		/// The hash algorithm has already been finalized and cannot accept more input data.
		/// </exception>
		protected override void HashCore(ReadOnlySpan<byte> source)
		{
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
			if (finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
#endif
			uint v = workingHash;
			foreach (var b in source)
			{
				v = (v * seedValue) + b;
			}
			workingHash = v;
		}

		/// <summary>
		/// Finalizes the hash computation and returns the resulting 32-bit <see cref="BKDR" /> hash in big-endian format. This method
		/// reflects all input previously processed via <see cref="HashAlgorithm.HashCore(byte[], int, int)" /> or
		/// <see cref="HashAlgorithm.HashCore(ReadOnlySpan{byte})" /> and produces a final, stable hash output.
		/// </summary>
		/// <returns>
		/// A 4-byte array representing the computed <c>BKDR</c> hash value. The result is encoded in <b>big-endian</b> byte order.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method completes the internal state of the hashing algorithm and serializes the final hash value into a
		/// platform-independent format. It is invoked automatically by <see cref="HashAlgorithm.ComputeHash(byte[])" /> and related methods
		/// once all data has been processed.
		/// </para>
		/// <para>After this method returns, the internal state is considered finalized and the computed hash is stable.</para>
		/// <para>
		/// In .NET 6.0 and later, the algorithm is automatically reset by invoking <see cref="HashAlgorithm.Initialize" />, allowing the
		/// instance to be reused immediately.
		/// </para>
		/// <para>
		/// In earlier versions of .NET, the internal state is marked as finalized, and any subsequent calls to
		/// <see cref="HashAlgorithm.HashCore(byte[], int, int)" />, <see cref="HashAlgorithm.HashCore(ReadOnlySpan{byte})" />, or
		/// <see cref="HashAlgorithm.HashFinal" /> will throw a <see cref="CryptographicUnexpectedOperationException" />. To compute another
		/// hash, you must explicitly call <see cref="HashAlgorithm.Initialize" /> to reset the algorithm.
		/// </para>
		/// <para>
		/// Implementations should ensure all residual or pending data is processed and integrated into the final hash value before returning.
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
			BinaryPrimitives.WriteUInt32BigEndian(span, workingHash);
			return span.ToArray();
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
	}
}