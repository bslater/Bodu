// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Bodu.Extensions;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes the hash for the input data using the <c>Bernstein</c> (djb2) hash algorithm. This variant performs a non-cryptographic
	/// 32-bit hash using iterative multiplication and addition for fast, well-distributed string hashing. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="Bernstein" /> class implements the non-cryptographic hash function known as djb2, created by Daniel J. Bernstein. It
	/// is widely used in hash tables, data indexing, and similar scenarios where speed and simplicity are preferred over cryptographic guarantees.
	/// </para>
	/// <para>
	/// This implementation includes an optional variant of the algorithm that uses an XOR instead of addition when combining characters
	/// into the hash. You can control this behavior with the <see cref="UseModifiedAlgorithm" /> property:
	/// </para>
	/// <list type="bullet">
	/// <item>
	/// <description>
	/// Set <see cref="UseModifiedAlgorithm" /> to <see langword="false" /> (default) to use the standard djb2 logic: <c>hash = (hash * 33)
	/// + c</c>.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// Set <see cref="UseModifiedAlgorithm" /> to <see langword="true" /> to use the XOR-modified variant: <c>hash = (hash * 33) ^ c</c>.
	/// This version may offer improved distribution properties in certain hash permutationTable implementations.
	/// </description>
	/// </item>
	/// </list>
	/// <para>Both versions produce a 32-bit integer hash from the input stream of bytes.</para>
	/// <note type="important">This algorithm is <b>not</b> cryptographically secure and should <b>not</b> be used for password hashing,
	/// digital signatures, or integrity validation in security-sensitive applications.</note>
	/// </remarks>
	public sealed class Bernstein
		: System.Security.Cryptography.HashAlgorithm
	{
		/// <summary>
		/// The default initial value used to seed the hash algorithm. This is constant.
		/// </summary>
		public const uint DefaultInitialValue = 5381U;

		private uint workingHash;
		private uint initialValue;
		private bool useModified;
		private bool disposed = false;
#if !NET6_0_OR_GREATER

		// Required for .NET Standard 2.0 or older frameworks
		private bool finalized;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="Bernstein" /> class with default parameters.
		/// </summary>
		public Bernstein()
		{
			this.HashSizeValue = 32;
			this.initialValue = this.workingHash = DefaultInitialValue;
			this.useModified = false;
		}

		/// <summary>
		/// Gets or sets the initial seed value used to start the hash computation.
		/// </summary>
		/// <value>The initial hash code value. Defaults to <see cref="DefaultInitialValue" />.</value>
		/// <exception cref="ObjectDisposedException">Instance has been disposed and its members are accessed.</exception>
		/// <exception cref="CryptographicUnexpectedOperationException">The hash computation has already started.</exception>
		public uint InitialValue
		{
			get
			{
				ThrowIfDisposed();

				return this.initialValue;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();

				this.initialValue = value;
				this.Initialize();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to use the XOR-modified variant of the <see cref="Bernstein" /> hash algorithm.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to use the modified algorithm ( <c>hash = (hash * 33) ^ c</c>); <see langword="false" /> to use the
		/// original djb2 form ( <c>hash = (hash * 33) + c</c>). The default is <see langword="false" />.
		/// </value>
		/// <exception cref="ObjectDisposedException">Instance has been disposed and its members are accessed.</exception>
		/// <exception cref="CryptographicUnexpectedOperationException">The hash computation has already started.</exception>
		public bool UseModifiedAlgorithm
		{
			get
			{
				ThrowIfDisposed();

				return this.useModified;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();

				this.useModified = value;
				this.Initialize();
			}
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (this.disposed) return;

			if (disposing)
			{
				CryptoUtilities.ClearAndNullify(ref HashValue);

				this.initialValue = workingHash = 0;
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

		/// <inheritdoc />
		public override void Initialize()
		{
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
			this.State = 0;
			this.finalized = false;
#endif
			this.workingHash = this.initialValue;
		}

		/// <summary>
		/// Processes a segment of the input byte array and feeds it into the <see cref="Bernstein" /> hashing algorithm. This method
		/// updates the internal state by processing <paramref name="cbSize" /> bytes starting at the specified <paramref name="ibStart" /> offset.
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
			ThrowHelper.ThrowIfArrayLengthIsInsufficient(array, offset, cbSize);
			if (this.finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
#endif

			if (this.useModified)
				HashModified(array.AsSpan(ibStart, cbSize));
			else
				HashOriginal(array.AsSpan(ibStart, cbSize));
		}

		/// <summary>
		/// Processes the entirety of the input <paramref name="source" /> and feeds it into the <see cref="Bernstein" /> hashing algorithm.
		/// This method updates the internal hash state accordingly by consuming the entire input span.
		/// </summary>
		/// <param name="source">The input byte span containing the data to hash.</param>
		/// <exception cref="CryptographicUnexpectedOperationException">
		/// The hash algorithm has already been finalized and cannot accept more input data.
		/// </exception>
		protected override void HashCore(ReadOnlySpan<byte> source)
		{
			ThrowIfDisposed();

#if !NET6_0_OR_GREATER
	if (this.finalized)
		throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
#endif

			if (this.useModified)
				HashModified(source);
			else
				HashOriginal(source);
		}

		/// <summary>
		/// Updates the internal hash state using the original Bernstein hash algorithm (hash = hash * 33 + b).
		/// </summary>
		/// <param name="data">
		/// The input data to hash. Each byte is processed sequentially and combined into the internal hash state using addition.
		/// </param>
		/// <remarks>This implementation corresponds to the original "djb2" hash function commonly used in hash table implementations.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void HashOriginal(ReadOnlySpan<byte> data)
		{
			uint v = workingHash;
			foreach (var b in data)
			{
				v = ((v << 5) + v) + b;
			}
			workingHash = v;
		}

		/// <summary>
		/// Updates the internal hash state using the modified Bernstein hash algorithm (hash = hash * 33 ^ b).
		/// </summary>
		/// <param name="data">
		/// The input data to hash. Each byte is processed sequentially and combined into the internal hash state using XOR.
		/// </param>
		/// <remarks>
		/// This variation of the Bernstein hash function replaces the addition step with a bitwise XOR operation for alternative mixing behavior.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void HashModified(ReadOnlySpan<byte> data)
		{
			uint v = workingHash;
			foreach (byte b in data)
			{
				v = ((v << 5) + v) ^ b;
			}
			workingHash = v;
		}

		/// <summary>
		/// Finalizes the hash computation and returns the resulting 32-bit <see cref="Bernstein" /> hash in big-endian format. This method
		/// reflects all input previously processed via <see cref="HashAlgorithm.HashCore(byte[], int, int)" /> or
		/// <see cref="HashAlgorithm.HashCore(ReadOnlySpan{byte})" /> and produces a final, stable hash output.
		/// </summary>
		/// <returns>
		/// A 4-byte array representing the computed <c>Bernstein</c> hash value. The result is encoded in <b>big-endian</b> byte order.
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
			if (this.finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);

			this.finalized = true;
			this.State = 2;
#endif

			Span<byte> span = stackalloc byte[4];
			BinaryPrimitives.WriteUInt32BigEndian(span, this.workingHash); // Explicit big-endian output
			return span.ToArray();
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
				throw new ObjectDisposedException(nameof(Bernstein));
#endif
		}
	}
}