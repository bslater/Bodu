// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Fletcher64.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Bodu.Extensions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes a 64-bit non-cryptographic hash using the ELF (Executable and Linkable Format) hashing algorithm.
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
		private ulong value;
		private bool disposed;
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
		/// Most hash algorithms support processing multiple input blocks in a single call to <see cref="TransformBlock" /> or
		/// <see cref="HashCore" />, making this property typically return <see langword="true" />. Override this to return
		/// <see langword="false" /> for algorithms that require strict block-by-block input.
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
			this.value = this.seedValue;
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return obj is Elf64 other &&
				this.seedValue == other.seedValue;
		}

		/// <inheritdoc />
		/// <summary>
		/// Processes a block of data by feeding it into the <see cref="Elf64" /> algorithm.
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

			this.ProcessBlocks(array, ibStart, cbSize);
		}

		/// <inheritdoc />
		/// <summary>
		/// Finalizes the <see cref="Elf64" /> hash computation after all input data has been processed, and returns the resulting hash value.
		/// </summary>
		/// <returns>A byte array containing the Elf64 result. The length is always 8 bytes, representing the 64-bit hash output.</returns>
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

			return this.value.GetBytes(asBigEndian: true);
		}

		/// <summary>
		/// Performs the ELF hash mixing routine over a block of bytes.
		/// </summary>
		/// <param name="array">The input data.</param>
		/// <param name="offset">Starting index within the array.</param>
		/// <param name="length">Number of bytes to process.</param>
		/// <remarks>
		/// This method shifts the internal hash state left by 4 bits, adds the current byte, then XORs and clears the high-order bits to
		/// ensure even distribution of bits.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ProcessBlocks(byte[] array, int offset, int length)
		{
			int end = offset + length;
			for (int i = offset; i < end; i++)
			{
				this.value = (this.value << 4) + array[i];
				ulong work = this.value & 0xF000000000000000UL;
				if (work != 0)
				{
					this.value ^= work >> 56;
					this.value &= ~work;
				}
			}
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

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (this.disposed) return;

			if (disposing)
			{
				this.seedValue = this.value = 0;
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