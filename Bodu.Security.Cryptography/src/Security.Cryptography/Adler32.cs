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
	/// Computes an Adler-32 checksum over input data.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Adler-32 is a fast, non-cryptographic checksum algorithm designed by Mark Adler for use in the zlib compression library. It produces
	/// a 32-bit checksum composed of two sums (A and B) modulo 65521.
	/// </para>
	/// <note type="important">This algorithm is <b>not</b> cryptographically secure and should <b>not</b> be used for digital signatures,
	/// password hashing, or integrity verification in security-sensitive contexts.</note>
	/// </remarks>
	public sealed class Adler32
		: System.Security.Cryptography.HashAlgorithm
	{
		private const uint Modulo = 65521; // Largest prime smaller than 2^16
		private uint partA;
		private uint partB;
		private readonly uint seed;
		private bool disposed;
#if !NET6_0_OR_GREATER

		// Required for .NET Standard 2.0 or older frameworks
		private bool finalized;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="Adler32" /> class.
		/// </summary>
		public Adler32()
		{
			this.HashSizeValue = 32;
			this.seed = 0;
			this.Initialize();
		}

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

#if !NET6_0_OR_GREATER

		/// <summary>
		/// Processes a block of data by feeding it into the Fletcher algorithm.
		/// </summary>
		/// <param name="array">The byte array containing the data to be hashed.</param>
		/// <param name="ibStart">The offset at which to start processing in the byte array.</param>
		/// <param name="cbSize">The length of the data to process.</param>
		/// <exception cref="ArgumentNullException"><paramref name="array" /> is <c>null</c>.</exception>
#else

		/// <summary>
		/// Processes a block of data by feeding it into the Fletcher algorithm.
		/// </summary>
		/// <param name="array">The byte array containing the data to be hashed.</param>
		/// <param name="ibStart">The offset at which to start processing in the byte array.</param>
		/// <param name="cbSize">The length of the data to process.</param>
#endif

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

				this.partA %= Modulo;
				this.partB %= Modulo;
				cbSize -= chunkSize;
			}
		}

#if !NET6_0_OR_GREATER

		/// <summary>
		/// Finalizes the hash computation and returns the resulting hash value.
		/// </summary>
		/// <returns>A byte array containing the computed hash.</returns>
		/// <exception cref="CryptographicUnexpectedOperationException">This method was called more than once without reinitializing.</exception>
#else

		/// <summary>
		/// Finalizes the hash computation and returns the resulting hash value.
		/// </summary>
		/// <returns>A byte array containing the computed hash.</returns>
#endif

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

		///// <inheritdoc />
		//public override bool Equals(object obj)
		//{
		//	return obj is Adler32 other &&
		//		   this. == other.Seed &&
		//		   this.Prime == other.Prime;
		//}

		///// <inheritdoc />
		//public override int GetHashCode()
		//{
		//	return HashCode.Combine(this.Seed, this.Prime);
		//}

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
				throw new ObjectDisposedException(nameof(Adler32));
#endif
		}
	}
}