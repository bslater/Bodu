using System.Buffers.Binary;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes a non-cryptographic 32-bit hash using the <see cref="JSHash" /> algorithm by Justin Sobel.
	/// </summary>
	/// <remarks>
	/// <para>
	/// JSHash is a compact and efficient bitwise hash function, originally developed by Justin Sobel. It is suitable for hash
	/// permutationTable indexing and other non-cryptographic use cases where speed is preferred over security.
	/// </para>
	/// <para>The core hash function operates using the expression: <c><![CDATA[hash ^= (hash << 5) + (hash >> 2) + byte]]></c>.</para>
	/// <note type="important">This algorithm is <b>not</b> cryptographically secure and should <b>not</b> be used for digital signatures,
	/// password hashing, or integrity verification in security-sensitive contexts.</note>
	/// </remarks>
	public sealed class JSHash
		: System.Security.Cryptography.HashAlgorithm
	{
		private const uint DefaultValue = 0x4E67C6A7;

		private uint value;
		private bool disposed;

#if !NET6_0_OR_GREATER

        // Required for .NET Standard 2.0 or older frameworks
        private bool finalized;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="JSHash" /> class.
		/// </summary>
		/// <remarks>This constructor initializes the hash algorithm to a default 32-bit output with a seed value of <c>0x4E67C6A7</c>.</remarks>
		public JSHash()
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
#if !NET6_0_OR_GREATER
            this.State = 0;
            this.finalized = false;
#endif
			ThrowIfDisposed();
			this.value = DefaultValue;
		}

		/// <inheritdoc />
		/// <summary>
		/// Processes a block of data by feeding it into the <see cref="JSHash" /> algorithm.
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

			foreach (byte b in array.AsSpan(ibStart, cbSize))
			{
				this.value ^= (this.value << 5) + (this.value >> 2) + b;
			}
		}

		/// <inheritdoc />
		/// <summary>
		/// Finalizes the <see cref="JSHash" /> computation after all input data has been processed, and returns the resulting hash value.
		/// </summary>
		/// <returns>A byte array containing the JSHash result. The length is always 4 bytes, representing the 32-bit hash output.</returns>
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
			byte[] buffer = new byte[4];
			BinaryPrimitives.WriteUInt32BigEndian(buffer, this.value);
			return buffer;
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (this.disposed)
				return;

			if (disposing)
			{
				this.value = 0;
			}

			this.disposed = true;
			base.Dispose(disposing);
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException" /> if the instance has already been disposed.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown when the algorithm has been disposed and further access is attempted.</exception>
		private void ThrowIfDisposed()
		{
#if NET8_0_OR_GREATER
			ObjectDisposedException.ThrowIf(this.disposed, this);
#else
			if (this.disposed)
				throw new ObjectDisposedException(nameof(JSHash));
#endif
		}
	}
}