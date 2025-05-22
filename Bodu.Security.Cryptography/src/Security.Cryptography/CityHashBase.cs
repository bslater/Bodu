// ---------------------------------------------------------------------------------------------------------------
// <copyright file="CityHash.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------
using Bodu.Extensions;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bodu.Security.Cryptography
{
	public abstract class CityHashBase<T>
		: System.Security.Cryptography.HashAlgorithm
		where T : CityHashBase<T>, new()
	{
		protected const UInt64 K0 = 0xc3a5c85c97cb3127;
		protected const UInt64 K1 = 0xb492b66fbe98f273;
		protected const UInt64 K2 = 0x9ae16a3b2f90404f;

		protected const UInt32 C1 = 0xcc9e2d51;
		protected const UInt32 C2 = 0x1b873593;

		protected const uint HashMagic = 0xe6546b64;

		private static readonly int[] ValidHashSizes = { 32, 64, 128 };

		private bool disposed = false;
#if !NET6_0_OR_GREATER

		// Required for .NET Standard 2.0 or older frameworks
		private bool finalized;
#endif

		protected CityHashBase(int hashSize)
		{
			if (Array.IndexOf(ValidHashSizes, hashSize) == -1)
				throw new ArgumentOutOfRangeException(nameof(hashSize),
					string.Format(ResourceStrings.CryptographicException_InvalidHashSize, hashSize, string.Join(", ", ValidHashSizes)));

			HashSizeValue = hashSize;
			this.Initialize();
		}

		/// <inheritdoc />
		public override void Initialize()
		{
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (this.disposed) return;

			if (disposing)
			{
				CryptoUtilities.ClearAndNullify(ref HashValue);
			}

			this.disposed = true;
			base.Dispose(disposing);
		}

		/// <inheritdoc />
		/// <summary>
		/// Processes a block of data by feeding it into the <see cref="CityHashBase{T}" /> algorithm.
		/// </summary>
		/// <param name="array">The byte array containing the data to be hashed.</param>
		/// <param name="ibStart">The offset at which to start processing in the byte array.</param>
		/// <param name="cbSize">The length of the data to process.</param>
		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			ThrowHelper.ThrowIfNull(array);
#if !NET6_0_OR_GREATER
			ThrowHelper.ThrowIfLessThan(ibStart, 0);
			ThrowHelper.ThrowIfLessThan(cbSize, 0);
			ThrowHelper.ThrowIfArrayLengthIsInsufficient(array, offset, cbSize);
			if (this.finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
#endif

			HashCore(array.AsSpan(ibStart, cbSize));
		}

		private byte[]? _computedHash;

		/// <summary>
		/// Processes a block of data by feeding it into the <see cref="CityHashBase{T}" /> algorithm.
		/// </summary>
		/// <param name="source">
		/// The input data to process. This method consumes the entire span and updates the internal checksum state accordingly.
		/// </param>
		protected override void HashCore(ReadOnlySpan<byte> source)
		{
			ThrowIfDisposed();

			this._computedHash = ComputeHashCore(source);
		}

		/// <summary>
		/// Performs the full hashing process on a complete input span. Used when ComputeHash is called in one shot (non-streaming).
		/// </summary>
		protected abstract byte[] ComputeHashCore(ReadOnlySpan<byte> source);

		/// <summary>
		/// Performs the final hash computation after all blocks have been processed. Only used if streaming (TransformBlock) is supported.
		/// </summary>
		protected virtual byte[] FinalizeHashCore() => Array.Empty<byte>();

		/// <inheritdoc />
		protected override byte[] HashFinal()
		{
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
			if (this.finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);

			this.finalized = true;
			this.State = 2;
#endif

			return new byte[0];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected static uint Mur(uint a, uint h) =>
			unchecked((a * C1) ^ h.RotateBitsRight(17));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected static uint Mix(uint h) =>
			h ^ (h >> 16);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected static void Permute3(ref uint a, ref uint b, ref uint c)
		{
			uint t = a;
			a = c;
			c = b;
			b = t;
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
				throw new ObjectDisposedException(nameof(T));
#endif
		}
	}
}