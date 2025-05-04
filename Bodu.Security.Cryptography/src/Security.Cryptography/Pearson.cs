using Bodu.Extensions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes a hash value using the Pearson hashing algorithm—a fast, lightweight, and non-cryptographic hash function suitable for
	/// basic checksums and hash-based lookups.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see href="https://en.wikipedia.org/wiki/Pearson_hashing">Pearson hashing algorithm</see>, introduced by Peter K. Pearson in
	/// 1990, computes a fixed-size hash (typically 8-bit or 16-bit) by transforming each byte of the input using a 256-element permutation table.
	/// </para>
	/// <para>The algorithm operates as follows:</para>
	/// <list type="number">
	/// <item>
	/// <description>Initialize the hash result to 0 (or a seed value).</description>
	/// </item>
	/// <item>
	/// <description>For each byte in the input, use the current hash value and the input byte as an index into the permutation table.</description>
	/// </item>
	/// <item>
	/// <description>The output from the table becomes the new hash value.</description>
	/// </item>
	/// </list>
	/// <para>
	/// When computing multi-byte hashes (e.g., 64-bit), the algorithm is repeated for each byte of the result, often using different
	/// initialization or byte offsets to reduce collisions.
	/// </para>
	/// <note type="important">This algorithm is <b>not</b> cryptographically secure. It must <b>not</b> be used for digital signatures,
	/// password hashing, or data integrity checks in security-critical applications.</note>
	/// </remarks>
	public sealed class Pearson
		: System.Security.Cryptography.HashAlgorithm
	{
		/// <summary>
		/// The minimum allowable hash size in bits.
		/// </summary>
		public const int MinHashSize = 8;

		/// <summary>
		/// The maximum allowable hash size in bits.
		/// </summary>
		public const int MaxHashSize = 2048;

		private static readonly byte[] DefaultTable = new byte[256]
		{
			0x62, 0x06, 0x55, 0x96, 0x24, 0x17, 0x70, 0xA4, 0x87, 0xCF, 0xA9, 0x05, 0x1A, 0x40, 0xA5, 0xDB,
			0x3D, 0x14, 0x44, 0x59, 0x82, 0x3F, 0x34, 0x66, 0x18, 0xE5, 0x84, 0xF5, 0x50, 0xD8, 0xC3, 0x73,
			0x5A, 0xA8, 0x9C, 0xCB, 0xB1, 0x78, 0x02, 0xBE, 0xBC, 0x07, 0x64, 0xB9, 0xAE, 0xF3, 0xA2, 0x0A,
			0xED, 0x12, 0xFD, 0xE1, 0x08, 0xD0, 0xAC, 0xF4, 0xFF, 0x7E, 0x65, 0x4F, 0x91, 0xEB, 0xE4, 0x79,
			0x7B, 0xFB, 0x43, 0xFA, 0xA1, 0x00, 0x6B, 0x61, 0xF1, 0x6F, 0xB5, 0x52, 0xF9, 0x21, 0x45, 0x37,
			0x3B, 0x99, 0x1D, 0x09, 0xD5, 0xA7, 0x54, 0x5D, 0x1E, 0x2E, 0x5E, 0x4B, 0x97, 0x72, 0x49, 0xDE,
			0xC5, 0x60, 0xD2, 0x2D, 0x10, 0xE3, 0xF8, 0xCA, 0x33, 0x98, 0xFC, 0x7D, 0x51, 0xCE, 0xD7, 0xBA,
			0x27, 0x9E, 0xB2, 0xBB, 0x83, 0x88, 0x01, 0x31, 0x32, 0x11, 0x8D, 0x5B, 0x2F, 0x81, 0x3C, 0x63,
			0x9A, 0x23, 0x56, 0xAB, 0x69, 0x22, 0x26, 0xC8, 0x93, 0x3A, 0x4D, 0x76, 0xAD, 0xF6, 0x4C, 0xFE,
			0x85, 0xE8, 0xC4, 0x90, 0xC6, 0x7C, 0x35, 0x04, 0x6C, 0x4A, 0xDF, 0xEA, 0x86, 0xE6, 0x9D, 0x8B,
			0xBD, 0xCD, 0xC7, 0x80, 0xB0, 0x13, 0xD3, 0xEC, 0x7F, 0xC0, 0xE7, 0x46, 0xE9, 0x58, 0x92, 0x2C,
			0xB7, 0xC9, 0x16, 0x53, 0x0D, 0xD6, 0x74, 0x6D, 0x9F, 0x20, 0x5F, 0xE2, 0x8C, 0xDC, 0x39, 0x0C,
			0xDD, 0x1F, 0xD1, 0xB6, 0x8F, 0x5C, 0x95, 0xB8, 0x94, 0x3E, 0x71, 0x41, 0x25, 0x1B, 0x6A, 0xA6,
			0x03, 0x0E, 0xCC, 0x48, 0x15, 0x29, 0x38, 0x42, 0x1C, 0xC1, 0x28, 0xD9, 0x19, 0x36, 0xB3, 0x75,
			0xEE, 0x57, 0xF0, 0x9B, 0xB4, 0xAA, 0xF2, 0xD4, 0xBF, 0xA3, 0x4E, 0xDA, 0x89, 0xC2, 0xAF, 0x6E,
			0x2B, 0x77, 0xE0, 0x47, 0x7A, 0x8E, 0x2A, 0xA0, 0x68, 0x30, 0xF7, 0x67, 0x0F, 0x0B, 0x8A, 0xEF
		};

		private bool isFirstByte;
		private byte[] hashValue;
		private byte[] permutationTable;
		private bool disposed;
#if !NET6_0_OR_GREATER

		// Required for .NET Standard 2.0 or older frameworks
		private bool finalized;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="Pearson" /> class with a default 8-bit hash size.
		/// </summary>
		public Pearson()
		{
			this.permutationTable = (byte[])DefaultTable.Clone();
			this.HashSizeValue = 8;
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
		/// Gets or sets the 256-byte permutation table used by the Pearson algorithm.
		/// </summary>
		/// <exception cref="CryptographicException">Thrown if modified after hashing has begun.</exception>
		/// <exception cref="ArgumentException">Thrown if the table is not 256 bytes or contains duplicate values.</exception>
		public byte[] Table
		{
			get
			{
				ThrowIfDisposed();
				return this.permutationTable.Copy();
			}

			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfInvalidState();
				if (value == null || value.Length != 256 || value.Distinct().Count() != 256)
					throw new ArgumentException("Table must contain 256 unique bytes.", nameof(value));

				this.permutationTable = value.Copy();
			}
		}

		/// <summary>
		/// Gets or sets the size, in bits, of the hash.
		/// </summary>
		/// <exception cref="CryptographicException">Thrown if modified after hashing has started.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if size is outside valid bounds.</exception>
		/// <exception cref="ArgumentException">Thrown if the hash size is not divisible by 8.</exception>
		public new int HashSize
		{
			get
			{
				this.ThrowIfDisposed();

				return this.HashSizeValue;
			}

			set
			{
				this.ThrowIfDisposed();
				this.ThrowIfInvalidState();
				ThrowHelper.ThrowIfOutOfRange(value, MinHashSize, MaxHashSize);
				ThrowHelper.ThrowIfNotPositiveMultipleOf(value, 8);

				this.HashSizeValue = value;
				this.Initialize();
			}
		}

		/// <inheritdoc />
		public override void Initialize()
		{
			this.ThrowIfDisposed();
#if !NET6_0_OR_GREATER
            this.State = 0;
            this.finalized = false;
#endif
			this.hashValue = new byte[this.HashSizeValue / 8];
			this.isFirstByte = true;
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (this.disposed) return;

			if (disposing)
			{
				if (this.permutationTable is not null)
				{
					CryptographicOperations.ZeroMemory(this.permutationTable);
					this.permutationTable.Clear();
				}

				if (this.hashValue is not null)
				{
					CryptographicOperations.ZeroMemory(this.hashValue);
					this.hashValue.Clear();
				}
			}

			this.disposed = true;
			base.Dispose(disposing);
		}

		/// <inheritdoc />
		/// <summary>
		/// Processes a block of data by feeding it into the <see cref="Pearson" /> algorithm.
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
			byte[] table = this.permutationTable;

			// Iterate through each byte of the input segment
			for (int i = ibStart; i < ibStart + cbSize; i++)
			{
				byte b = array[i];

				// Update each byte of the hash output
				for (int j = 0; j < this.hashValue.Length; j++)
				{
					// If this is the first byte being hashed, initialize each hash byte using (byte + position) mod 256 as index into the
					// permutation permutationTable. Otherwise, XOR current hash value with the input byte and rehash via permutationTable.
					this.hashValue[j] = this.isFirstByte
						? table[(b + j) & 0xFF]                    // first byte: seed each hash byte
						: table[this.hashValue[j] ^ b];            // all other bytes: chain each with XOR
				}

				// After the first input byte is processed, all subsequent updates should follow XOR mode
				this.isFirstByte = false;
			}
		}

		/// <inheritdoc />
		/// <summary>
		/// Finalizes the <see cref="Pearson" /> hash computation after all input data has been processed, and returns the resulting hash value.
		/// </summary>
		/// <returns>
		/// A byte array containing the Pearson result. The length depends on the <see cref="HashAlgorithm.HashSize" /> setting, typically 1
		/// byte (8 bits), but may be longer if an extended variant is used.
		/// </returns>
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
			return this.hashValue;
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
				throw new ObjectDisposedException(nameof(Elf64));
#endif
		}
	}
}