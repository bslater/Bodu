// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Bodu.Extensions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes the hash for the input data using the <see cref="Bernstein" /> (djb2) hash algorithm.
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
	/// digital signatures, or any use case that requires secure integrity or confidentiality.</note>
	/// </remarks>
	public sealed class Bernstein
			: System.Security.Cryptography.HashAlgorithm
	{
		/// <summary>
		/// The default initial value used to seed the hash algorithm. This is constant.
		/// </summary>
		public const uint DefaultInitialValue = 5381U;

		private uint value;
		private uint initialValue;
		private bool useModified;
		private bool disposed;
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
			this.initialValue = DefaultInitialValue;
			this.useModified = false;
			this.Initialize();
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
				this.initialValue = 0;
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
		/// Most hash algorithms support processing multiple input blocks in a single call to <see cref="TransformBlock" /> or
		/// <see cref="HashCore" />, making this property typically return <see langword="true" />. Override this to return
		/// <see langword="false" /> for algorithms that require strict block-by-block input.
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
			this.value = this.initialValue;
		}

		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
			return obj is Bernstein other &&
				   this.useModified == other.useModified &&
				   this.initialValue == other.initialValue;
		}

		/// <inheritdoc />
		/// <summary>
		/// Processes a block of data by feeding it into the <see cref="Bernstein" /> algorithm.
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

			if (this.useModified)
			{
				this.HashModified(array, ibStart, cbSize);
			}
			else
			{
				this.HashOriginal(array, ibStart, cbSize);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void HashOriginal(byte[] data, int offset, int length)
		{
			int end = offset + length;
			for (int i = offset; i < end; i++)
			{
				// hash = hash * 33 + data[i]
				this.value = ((this.value << 5) + this.value) + data[i];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void HashModified(byte[] data, int offset, int length)
		{
			int end = offset + length;
			for (int i = offset; i < end; i++)
			{
				// hash = hash * 33 ^ data[i]
				this.value = ((this.value << 5) + this.value) ^ data[i];
			}
		}

		/// <inheritdoc />
		/// <summary>
		/// Finalizes the <see cref="Bernstein" /> hash computation after all input data has been processed, and returns the resulting hash value.
		/// </summary>
		/// <returns>
		/// A byte array containing the Bernstein result. The length depends on the configured <see cref="HashAlgorithm.HashSize" />,
		/// typically 4 bytes for a 32-bit variant or 8 bytes for a 64-bit variant.
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

			return this.value.GetBytes(asBigEndian: true);
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