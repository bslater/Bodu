using Bodu;
using Bodu.Security.Cryptography;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Provides a base implementation for block-oriented hash algorithms.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This abstract class supports hashing algorithms that process input data in fixed-size blocks (e.g., 8 bytes at a time). It manages
	/// internal residual buffering, final block padding, and length tracking to support incremental data input via <see cref="HashCore" /> methods.
	/// </para>
	/// <para>
	/// Subclasses must implement <see cref="ProcessBlock" />, <see cref="PadBlock" />, and <see cref="ProcessFinalBlock" /> to define
	/// algorithm-specific behavior.
	/// </para>
	/// </remarks>
	public abstract class BlockHashAlgorithm<T>
		: HashAlgorithm
		where T : BlockHashAlgorithm<T>, new()
	{
		/// <summary>
		/// The fixed size, in bytes, of each block processed by the algorithm.
		/// </summary>
		protected readonly int BlockSizeBytes; // Size of a single block (in bytes) to be processed per hash step

		private readonly Memory<byte> residualByteBuffer; // Temporary buffer to hold incomplete blocks between HashCore calls
		private int residualBytes;                        // Number of bytes currently in the residual buffer
		private ulong totalLength;                        // Total length of data processed, used for padding
		private bool disposed;                            // Tracks whether Dispose() has been called

#if !NET6_0_OR_GREATER

    /// <summary>
    /// Indicates whether the hash computation has been finalized. Used in .NET Standard environments.
    /// </summary>
    protected bool finalized;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="BlockHashAlgorithm{T}" /> class with a specified block size.
		/// </summary>
		/// <param name="blockSize">The block size, in bytes, used by the algorithm.</param>
		protected BlockHashAlgorithm(int blockSize)
		{
			BlockSizeBytes = blockSize;
			residualByteBuffer = new byte[BlockSizeBytes];
		}

		/// <inheritdoc />
		public override void Initialize()
		{
			// Clear internal buffers and reset state
			residualByteBuffer.Span.Clear();
			residualBytes = 0;
			totalLength = 0;
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (disposed) return;

			if (disposing)
			{
				CryptoUtilities.Clear(residualByteBuffer);
				residualBytes = 0;
				totalLength = 0;
			}

			disposed = true;
			base.Dispose(disposing);
		}

		/// <summary>
		/// Processes a span of input bytes in fixed-size blocks, handling any residual bytes from previous invocations.
		/// </summary>
		/// <param name="buffer">The input span to process. May include incomplete blocks.</param>
		/// <remarks>
		/// This method handles leftover bytes from previous calls and ensures full blocks are processed immediately. Any incomplete tail
		/// data is buffered until more input is received.
		/// </remarks>
		private void ProcessBlocks(ReadOnlySpan<byte> buffer)
		{
			int pos = 0;
			totalLength += (ulong)buffer.Length;

			Span<byte> residualSpan = residualByteBuffer.Span;

			// Attempt to fill a partial residual block if it exists
			if (residualBytes > 0)
			{
				int remaining = BlockSizeBytes - residualBytes;

				if (buffer.Length >= remaining)
				{
					// Complete residual block and process it
					buffer.Slice(pos, remaining).CopyTo(residualSpan[residualBytes..]);
					ProcessBlock(residualByteBuffer.Span);
					residualBytes = 0;
					pos += remaining;
				}
				else
				{
					// Not enough to complete a block, buffer it for later
					buffer.CopyTo(residualSpan[residualBytes..]);
					residualBytes += buffer.Length;
					return;
				}
			}

			// Process complete blocks from input span
			while (pos + BlockSizeBytes <= buffer.Length)
			{
				ProcessBlock(buffer.Slice(pos, BlockSizeBytes));
				pos += BlockSizeBytes;
			}

			// Buffer any trailing bytes that form an incomplete block
			residualBytes = buffer.Length - pos;
			if (residualBytes > 0)
				buffer.Slice(pos, residualBytes).CopyTo(residualSpan);
		}

		/// <inheritdoc />
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

			ProcessBlocks(array.AsSpan(ibStart, cbSize));
		}

		/// <inheritdoc />
		protected override void HashCore(ReadOnlySpan<byte> source)
		{
			ThrowIfDisposed();

#if !NET6_0_OR_GREATER
        if (finalized)
            throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
#endif

			ProcessBlocks(source);
		}

		/// <inheritdoc />
		protected override byte[] HashFinal()
		{
			ThrowIfDisposed();

#if !NET6_0_OR_GREATER
        if (finalized)
            throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
#endif

			// Generate padded final block and process all parts of it
			var finalBlock = PadBlock(residualByteBuffer.Span.Slice(0, residualBytes), totalLength);
			for (int i = 0; i < finalBlock.Length; i += BlockSizeBytes)
				ProcessBlock(finalBlock.AsSpan(i, BlockSizeBytes));

			return ProcessFinalBlock();
		}

		/// <summary>
		/// Pads the final block of input data and encodes the total message length into the resulting block(s).
		/// </summary>
		/// <param name="block">The final partial block, containing 0 to BlockSize–1 bytes of input.</param>
		/// <param name="messageLength">The total number of bytes processed by the algorithm.</param>
		/// <returns>A fully padded final block (or blocks) ready for processing.</returns>
		protected abstract byte[] PadBlock(ReadOnlySpan<byte> block, ulong messageLength);

		/// <summary>
		/// Processes a full block of input data, updating the internal state.
		/// </summary>
		/// <param name="block">The block of input data to process.</param>
		protected abstract void ProcessBlock(ReadOnlySpan<byte> block);

		/// <summary>
		/// Computes the final hash output after all blocks have been processed.
		/// </summary>
		/// <returns>A byte array containing the final hash value.</returns>
		protected abstract byte[] ProcessFinalBlock();

		/// <summary>
		/// Throws if the algorithm has begun processing and can no longer be reconfigured.
		/// </summary>
		/// <exception cref="CryptographicUnexpectedOperationException">
		/// Thrown if an attempt is made to change configuration after the algorithm has started.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void ThrowIfInvalidState()
		{
			if (this.State != 0)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_ReconfigurationNotAllowed);
		}

		/// <summary>
		/// Throws an exception if the instance has been disposed.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown when the instance has already been disposed.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void ThrowIfDisposed()
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