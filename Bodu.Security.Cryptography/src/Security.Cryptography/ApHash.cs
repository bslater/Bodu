// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ApHash.cs" company="PlaceholderCompany">
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
	/// <summary>
	/// Computes a 32-bit non-cryptographic hash using the APHash algorithm.
	/// </summary>
	/// <remarks>
	/// <para>
	/// APHash is a simple, fast, and non-cryptographic hashing algorithm designed for hash table lookups and similar use cases. It combines
	/// bitwise and arithmetic operations with alternating logic based on the input index.
	/// </para>
	/// <para>The algorithm uses the following rules:</para>
	/// <list type="number">
	/// <item>
	/// <description>Initialize the checksum to a fixed 32-bit constant (0xAAAAAAAA).</description>
	/// </item>
	/// <item>
	/// <description>For each byte, use alternating XOR patterns depending on whether the index is even or odd.</description>
	/// </item>
	/// </list>
	/// <note type="important">This algorithm is <b>not cryptographically secure</b>. It should not be used for password hashing, integrity
	/// verification, or any security-sensitive applications.</note>
	/// </remarks>
	public sealed class ApHash
		: System.Security.Cryptography.HashAlgorithm
	{
		private const uint DefaultCheckSumValue = 0xAAAAAAAA;

		private uint hashValue;
		private ulong size;
		private bool disposed = false;
#if !NET6_0_OR_GREATER

		// Required for .NET Standard 2.0 or older frameworks
		private bool finalized;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="ApHash" /> class with a 32-bit hash size.
		/// </summary>
		public ApHash()
		{
			this.HashSizeValue = 32;
			this.Initialize();
		}

		/// <inheritdoc />
		public override void Initialize()
		{
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
			this.State = 0;
			this.finalized = false;
#endif
			this.hashValue = ApHash.DefaultCheckSumValue;
			this.size = 0;
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (this.disposed) return;

			if (disposing)
			{
				this.hashValue = 0;
			}

			this.disposed = true;
			base.Dispose(disposing);
		}

		/// <inheritdoc />
		/// <summary>
		/// Processes a block of data by feeding it into the <see cref="ApHash" /> algorithm.
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

		/// <summary>
		/// Processes a block of data by feeding it into the <see cref="ApHash" /> algorithm.
		/// </summary>
		/// <param name="source">
		/// The input data to process. This method consumes the entire span and updates the internal checksum state accordingly.
		/// </param>
		protected override void HashCore(ReadOnlySpan<byte> source)
		{
			ThrowIfDisposed();

			var v = hashValue;
			foreach (var b in source)
			{
				if ((size & 1) == 0)
					v ^= (v << 7) ^ b ^ (v >> 3);
				else
					v ^= ~((v << 11) ^ b ^ (v >> 5));

				size++;
			}
			hashValue = v;
		}

		/// <inheritdoc />
		/// <summary>
		/// Finalizes the hash computation and returns the resulting hash as a byte array.
		/// </summary>
		/// <returns>A 4-byte array containing the final 32-bit APHash result.</returns>
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
			MemoryMarshal.Write(span, in this.hashValue);
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