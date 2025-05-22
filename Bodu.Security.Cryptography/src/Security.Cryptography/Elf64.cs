// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Fletcher64.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Bodu.Extensions;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes the hash for the input data using the <c>ELF-64</c> (Executable and Linkable Format) hash algorithm. This variant applies a
	/// non-cryptographic bitwise transformation commonly used in Unix object file processing and hash table indexing. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// <para>
	/// ELF hashing is a simple, non-cryptographic routine originally used in the UNIX System V ELF object file format. It processes each
	/// byte of input by shifting and mixing bits to produce a pseudo-random but repeatable hash output.
	/// </para>
	/// <para>
	/// This implementation uses a 64-bit internal state and is intended for fast hashing of byte sequences such as identifiers or text
	/// keys. It is <b>not suitable</b> for cryptographic purposes.
	/// </para>
	/// <para>
	/// An optional <see cref="Seed" /> value may be specified to alter the initial state. The seed cannot be changed once hashing begins.
	/// </para>
	/// <note type="important">This algorithm is <b>not</b> cryptographically secure and should <b>not</b> be used for digital signatures,
	/// password hashing, or integrity verification in security-sensitive contexts.</note>
	/// </remarks>
	public sealed class Elf64
		: System.Security.Cryptography.HashAlgorithm
	{
		private ulong seedValue;
		private ulong workingHash;
		private bool disposed = false;
#if !NET6_0_OR_GREATER

		// Required for .NET Standard 2.0 or older frameworks
		private bool finalized;
#endif

		/// <inheritdoc />
		public Elf64()
		{
			this.HashSizeValue = 64;
			this.seedValue = 0;
			this.Initialize();
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
		/// Gets or sets the seed used to initialize the internal hash state.
		/// </summary>
		/// <value>The seed value applied before hashing begins.</value>
		/// <exception cref="ObjectDisposedException">Instance has been disposed and its members are accessed.</exception>
		/// <exception cref="CryptographicUnexpectedOperationException">The hash computation has already started.</exception>
		/// <remarks>
		/// Changing the seed influences the initial hash state and therefore the resulting hash output. Common seed values such as 31, 131,
		/// or 1313 are often used to reduce clustering or bias.
		/// </remarks>
		public ulong Seed
		{
			get
			{
				ThrowIfDisposed();

				return this.seedValue;
			}

			set
			{
				ThrowIfInvalidState();
				ThrowIfDisposed();

				this.seedValue = value;
				this.Initialize();
			}
		}

		/// <inheritdoc />
		public override void Initialize()
		{
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
			this.State = 0;
			this.finalized = false;
#endif
			this.workingHash = this.seedValue;
		}

		/// <summary>
		/// Processes a segment of the input byte array and feeds it into the <see cref="Elf64" /> hashing algorithm. This method updates
		/// the internal state by processing <paramref name="cbSize" /> bytes starting at the specified <paramref name="ibStart" /> offset.
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

			this.HashCore(array.AsSpan(ibStart, cbSize));
		}

		/// <summary>
		/// Processes the entirety of the input <paramref name="source" /> and feeds it into the <see cref="Elf64" /> hashing algorithm.
		/// This method updates the internal hash state accordingly by consuming the entire input span.
		/// </summary>
		/// <param name="source">The input byte span containing the data to hash.</param>
		/// <exception cref="CryptographicUnexpectedOperationException">
		/// The hash algorithm has already been finalized and cannot accept more input data.
		/// </exception>
		protected override void HashCore(ReadOnlySpan<byte> source)
		{
			ThrowIfDisposed();

			var v = workingHash;
			foreach (byte b in source)
			{
				v = (v << 4) + b;
				ulong work = v & 0xF000000000000000UL;

				v ^= work >> 56;
				v &= ~work;
			}
			workingHash = v;
		}

		/// <summary>
		/// Finalizes the hash computation and returns the resulting 64-bit <see cref="Elf64" /> hash in big-endian format. This method
		/// reflects all input previously processed via <see cref="HashAlgorithm.HashCore(byte[], int, int)" /> or
		/// <see cref="HashAlgorithm.HashCore(ReadOnlySpan{byte})" /> and produces a final, stable hash output.
		/// </summary>
		/// <returns>
		/// A 8-byte array representing the computed <c>Elf64</c> hash value. The result is encoded in <b>big-endian</b> byte order.
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

			return this.workingHash.GetBytes(asBigEndian: true);
		}

		///// <summary>
		///// Performs the ELF hash mixing routine over a block of bytes.
		///// </summary>
		///// <param name="array">The input data.</param>
		///// <param name="offset">Starting index within the array.</param>
		///// <param name="length">Number of bytes to process.</param>
		///// <remarks>
		///// This method shifts the internal hash state left by 4 bits, adds the current byte, then XORs and clears the high-order bits to
		///// ensure even distribution of bits.
		///// </remarks>
		//private void ProcessBlocks(byte[] array, int offset, int length)
		//{
		//	int end = offset + length;
		//	for (int i = offset; i < end; i++)
		//	{
		//		this.workingHash = (this.workingHash << 4) + array[i];
		//		ulong work = this.workingHash & 0xF000000000000000UL;
		//		if (work != 0)
		//		{
		//			this.workingHash ^= work >> 56;
		//			this.workingHash &= ~work;
		//		}
		//	}
		//}

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

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (this.disposed) return;

			if (disposing)
			{
				this.seedValue = this.workingHash = 0;
			}

			this.disposed = true;
			base.Dispose(disposing);
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
				throw new ObjectDisposedException(nameof(Elf64));
#endif
		}
	}
}