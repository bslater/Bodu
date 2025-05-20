// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Pjw32.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	using System.Buffers.Binary;
	using System.Runtime.CompilerServices;

	#region Using Statements

	using System.Security.Cryptography;

	using Bodu.Extensions;

	#endregion Using Statements

	/// <summary>
	/// Computes the hash for the input data by using the <see cref="Bodu.Security.Cryptography.Pjw32" /> hash algorithm. This class cannot
	/// be inherited.
	/// </summary>
	public sealed class Pjw32
		: System.Security.Cryptography.HashAlgorithm
	{
		private uint hashValue;
		private bool disposed = false;
#if !NET6_0_OR_GREATER

		// Required for .NET Standard 2.0 or older frameworks
		private bool finalized;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="Bodu.Security.Cryptography.Pjw32" /> class.
		/// </summary>
		public Pjw32()
		{
			this.HashSizeValue = 32;
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

		/// <inheritdoc />
		public override void Initialize()
		{
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
			this.State = 0;
			this.finalized = false;
#endif
			this.hashValue = 0;
		}

		/// <inheritdoc />
		/// <summary>
		/// Processes a block of data by feeding it into the <see cref="PjW32" /> algorithm.
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

			HashCore(array.AsSpan(ibStart, cbSize));
		}

		/// <summary>
		/// Processes a block of data by feeding it into the <see cref="PjW32" /> algorithm.
		/// </summary>
		/// <param name="source">
		/// The input data to process. This method consumes the entire span and updates the internal checksum state accordingly.
		/// </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void HashCore(ReadOnlySpan<byte> source)
		{
			const uint HighBitsMask = 0xF0000000u;
			const int Shift = 28;
			const uint LowBitsMask = 0x0FFFFFFF;

			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
			if (this.finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
#endif
			uint v = hashValue;

			foreach (var b in source)
			{
				v = (v << 4) + b;

				uint high = v & HighBitsMask;
				v ^= high >> Shift;
				v &= LowBitsMask;
			}
			hashValue = v;
		}

		/// <inheritdoc />
		/// <summary>
		/// Finalizes the <see cref="BKDR" /> hash computation after all input data has been processed, and returns the resulting hash value
		/// as a byte array.
		/// </summary>
		/// <returns>
		/// A byte array representing the finalized PjW32 hash. The length corresponds to the configured
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
				throw new ObjectDisposedException(nameof(PjW32));
#endif
		}

		private void ProcessBlocks(byte[] array, int offset, int length)
		{
			int end = offset + length;
			for (int i = offset; i < end; i++)
			{
				this.hashValue = (this.hashValue << 4) + array[i];
				uint work = this.hashValue & 0xf0000000u;
				if (work != 0)
				{
					this.hashValue = (this.hashValue ^ (work >> 28)) & 0x0FFFFFFF;
				}
			}
		}
	}
}