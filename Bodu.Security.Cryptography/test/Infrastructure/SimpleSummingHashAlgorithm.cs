using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Infrastructure
{
	/// <summary>
	/// A simple hash algorithm that computes the sum of all input bytes as a 32-bit unsigned integer. This implementation is
	/// non-cryptographic and should not be used for security purposes.
	/// </summary>
	public class SimpleSummingHashAlgorithm
		: System.Security.Cryptography.HashAlgorithm
	{
		// Holds the running total of the byte values.
		private uint hashValue;

		private bool disposed;
		private long bytesProcessed;
		private uint seedValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleSummingHashAlgorithm" /> class.
		/// </summary>
		public SimpleSummingHashAlgorithm()
		{
			// The hash size is fixed at 32 bits (4 bytes)
			this.HashSizeValue = sizeof(uint) * 8;
			this.bytesProcessed = this.seedValue = this.hashValue = 0;
			this.Initialize();
		}

		/// <summary>
		/// Gets the total number of bytes processed by the algorithm.
		/// </summary>
		public long BytesProcessed => this.bytesProcessed;

		public uint Seed
		{
			get
			{
				this.ThrowIfDisposed();
				return this.seedValue;
			}

			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfInvalidState();

				this.seedValue = value;
				this.Initialize();
			}
		}

		/// <summary>
		/// Initializes or resets the hash algorithm to its initial state.
		/// </summary>
		public override void Initialize()
		{
			this.ThrowIfDisposed();
#if !NET6_0_OR_GREATER
            this.State = 0;
            this.finalized = false;
#endif
			this.bytesProcessed = 0;
			this.hashValue = this.seedValue;
		}

		/// <summary>
		/// Processes a block of data and updates the internal hash state.
		/// </summary>
		/// <param name="array">The input byte array.</param>
		/// <param name="ibStart">The zero-based starting index in the array.</param>
		/// <param name="cbSize">The number of bytes to process.</param>
		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			this.ThrowIfDisposed();

			// Sum each byte into the hash hashValue
			for (int i = ibStart; i < ibStart + cbSize; i++)
				this.hashValue += array[i];

			// Track how many bytes were processed
			this.bytesProcessed += cbSize;
		}

		/// <summary>
		/// Finalizes the hash computation and returns the computed hash hashValue.
		/// </summary>
		/// <returns>A byte array representing the computed hash.</returns>
		protected override byte[] HashFinal()
		{
			this.ThrowIfDisposed();

			// Convert the final sum to a byte array
			return BitConverter.GetBytes(this.hashValue);
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (this.disposed) return;

			if (disposing)
			{
				this.hashValue = 0;
				this.bytesProcessed = 0;
			}

			this.disposed = true;
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
				throw new ObjectDisposedException(nameof(SipHash));
#endif
		}
	}
}