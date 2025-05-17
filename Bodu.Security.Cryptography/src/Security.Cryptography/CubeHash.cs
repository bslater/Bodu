// ---------------------------------------------------------------------------------------------------------------
// <copyright file="CubeHash.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes the hash for the input data by using the <see cref="CubeHash" /> hash algorithm. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="CubeHash" /> algorithm is a cryptographic hash function submitted to the NIST SHA-3 hash competition by Daniel J.
	/// Bernstein. This implementation supports parameterized configuration to allow flexibility in hash size, round counts, and input block size.
	/// </para>
	/// <para>See also: <a href="https://en.wikipedia.org/wiki/CubeHash">https://en.wikipedia.org/wiki/CubeHash</a></para>
	/// </remarks>
	public sealed class CubeHash
		: System.Security.Cryptography.HashAlgorithm
	{
		/// <summary>
		/// The maximum allowable size of the computed hash, in bits.
		/// </summary>
		public const int MaxHashSize = 512;

		/// <summary>
		/// The maximum allowable size of the input block, in bytes.
		/// </summary>
		public const int MaxInputBlockSize = 128;

		/// <summary>
		/// The minimum allowable size of the computed hash, in bits.
		/// </summary>
		public const int MinHashSize = 8;

		/// <summary>
		/// The minimum allowable size of the input block, in bytes.
		/// </summary>
		public const int MinInputBlockSize = 1;

		/// <summary>
		/// The minimum number of rounds permitted for initialization, processing, or finalization.
		/// </summary>
		public const int MinRounds = 1;

		/// <summary>
		/// The maximum number of rounds permitted for initialization, processing, or finalization.
		/// </summary>
		public const int MaxRounds = 4096;

		// Internal algorithm parameters
		private int finalizationRounds;

		private int initializationRounds;
		private int inputBlockSizeBits;
		private int bitLength; // tracks number of bits consumed
		private int rounds;
		private uint[] state;
		private bool disposed;

#if !NET6_0_OR_GREATER
        private bool finalized; // flag to block reuse in older .NET
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="CubeHash" /> class with default parameters.
		/// </summary>
		public CubeHash()
		{
			this.HashSize = 512;
			this.TransformBlockSize = 32;
			this.Rounds = this.InitializationRounds = 16;
			this.FinalizationRounds = 32;
			this.bitLength = 0;
			this.state = new uint[32];
			this.InitializeVectors();
		}

		/// <inheritdoc />
		/// <summary>
		/// Gets the input block size, in bytes, used by consumers of the <see cref="CubeHash" /> algorithm, such as <see cref="System.Security.Cryptography.CryptoStream" />.
		/// </summary>
		/// <remarks>
		/// This value reflects the configured <see cref="TransformBlockSize" />, which determines how many bytes are accumulated before a
		/// transformation round is triggered internally. While this value does not impact the correctness of the hash, feeding data in
		/// aligned blocks may improve performance in stream-based scenarios.
		/// </remarks>
		public override int InputBlockSize => this.TransformBlockSize;

		/// <inheritdoc />
		/// <summary>
		/// Gets the output block size, in bytes, of the final computed hash value.
		/// </summary>
		/// <remarks>
		/// This is equal to the configured <see cref="HashSize" /> divided by 8. For example, a 512-bit hash will produce an output block
		/// of 64 bytes. This value corresponds to the full digest returned after <see cref="HashAlgorithm.HashFinal" /> is called.
		/// </remarks>
		public override int OutputBlockSize => this.HashSize / 8;

		/// <summary>
		/// Gets or sets the number of finalization rounds applied after all input has been processed.
		/// </summary>
		/// <remarks>
		/// Finalization rounds provide additional mixing of the internal state to ensure that the final hash output is highly sensitive to
		/// every bit of input data. Increasing this value strengthens final-state diffusion.
		/// </remarks>
		/// <exception cref="ObjectDisposedException">Instance has been disposed and its members are accessed.</exception>
		/// <exception cref="CryptographicUnexpectedOperationException">The hash computation has already started.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Value is less than <see cref="MinRounds" /> or greater than <see cref="MaxRounds" />.</exception>
		public int FinalizationRounds
		{
			get
			{
				ThrowIfDisposed();
				return this.finalizationRounds;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				ThrowHelper.ThrowIfOutOfRange(value, MinRounds, MaxRounds);
				this.finalizationRounds = value;
				this.Initialize();
			}
		}

		/// <summary>
		/// Gets or sets the size, in bits, of the final computed hash output.
		/// </summary>
		/// <remarks>
		/// The hash size determines the length of the digest returned by the algorithm. Valid values must be between
		/// <see cref="MinHashSize" /> and <see cref="MaxHashSize" />, and divisible by 8. Larger sizes increase output strength.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Value is not within range <see cref="MinHashSize" /> to <see cref="MaxHashSize" />, or is not a multiple of 8.
		/// </exception>
		/// <exception cref="ObjectDisposedException">Instance has been disposed and its members are accessed.</exception>
		/// <exception cref="CryptographicUnexpectedOperationException">The hash computation has already started.</exception>
		public new int HashSize
		{
			get
			{
				ThrowIfDisposed();
				return this.HashSizeValue;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				ThrowHelper.ThrowIfOutOfRange(value, MinHashSize, MaxHashSize);
				ThrowHelper.ThrowIfNotPositiveMultipleOf(value, 8);
				this.HashSizeValue = value;
				this.Initialize();
			}
		}

		/// <summary>
		/// Gets or sets the number of initialization rounds to run before processing input data.
		/// </summary>
		/// <remarks>
		/// Initialization rounds mix the initial state of the algorithm before the first input byte is processed. Increasing this value
		/// enhances initial diffusion but increases computation time.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Value is less than <see cref="MinRounds" /> or greater than <see cref="MaxRounds" />.</exception>
		/// <exception cref="ObjectDisposedException">Instance has been disposed and its members are accessed.</exception>
		/// <exception cref="CryptographicUnexpectedOperationException">The hash computation has already started.</exception>
		public int InitializationRounds
		{
			get
			{
				ThrowIfDisposed();
				return this.initializationRounds;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				ThrowHelper.ThrowIfOutOfRange(value, MinRounds, MaxRounds);
				this.initializationRounds = value;
				this.Initialize();
			}
		}

		/// <summary>
		/// Gets or sets the size, in bytes, of the input block used by the CubeHash algorithm to determine when to perform a state transformation.
		/// </summary>
		/// <remarks>
		/// Unlike <see cref="HashAlgorithm.InputBlockSize" />, which is advisory, this property directly affects the output of the hash
		/// function. When the number of accumulated input bytes reaches <c>TransformBlockSize</c>, a transformation round is triggered.
		/// Modifying this value changes the frequency of internal state updates, impacting both performance and security characteristics.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Value is not within range <see cref="MinInputBlockSize" /> to <see cref="MaxInputBlockSize" />.</exception>
		/// <exception cref="ObjectDisposedException">Instance has been disposed and its members are accessed.</exception>
		/// <exception cref="CryptographicUnexpectedOperationException">The hash computation has already started.</exception>
		public int TransformBlockSize
		{
			get
			{
				ThrowIfDisposed();
				return this.inputBlockSizeBits / 8;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				ThrowHelper.ThrowIfOutOfRange(value, MinInputBlockSize, MaxInputBlockSize);
				this.inputBlockSizeBits = value * 8;
				this.Initialize();
			}
		}

		/// <summary>
		/// Gets or sets the number of transformation rounds applied to each full input block.
		/// </summary>
		/// <remarks>
		/// A higher number of rounds provides greater mixing of the state per block, which improves security at the cost of speed.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Value is less than <see cref="MinRounds" /> or greater than <see cref="MaxRounds" />.</exception>
		/// <exception cref="ObjectDisposedException">Instance has been disposed and its members are accessed.</exception>
		/// <exception cref="CryptographicUnexpectedOperationException">The hash computation has already started.</exception>
		public int Rounds
		{
			get
			{
				ThrowIfDisposed();
				return this.rounds;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				ThrowHelper.ThrowIfOutOfRange(value, MinRounds, MaxRounds);
				this.rounds = value;
				this.Initialize();
			}
		}

		/// <summary>
		/// Gets the descriptive name of the current configuration.
		/// </summary>
		/// <remarks>This value includes initialization, round, and hash size parameters.</remarks>
		public string Name => $"CubeHash{InitializationRounds}+{Rounds}/{TransformBlockSize}+{FinalizationRounds}-{HashSize}";

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
			this.bitLength = 0;
			this.InitializeVectors();
		}

		/// <inheritdoc />
		/// <summary>
		/// Processes a block of data by feeding it into the <see cref="CubeHash" /> algorithm.
		/// </summary>
		/// <param name="array">The byte array containing the data to be hashed.</param>
		/// <param name="ibStart">The offset at which to start processing in the byte array.</param>
		/// <param name="cbSize">The length of the data to process.</param>
		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			ThrowHelper.ThrowIfNull(array);
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
            ThrowHelper.ThrowIfLessThan(offset, 0);
            ThrowHelper.ThrowIfLessThan(length, 0);
            ThrowHelper.ThrowIfArrayLengthIsInsufficient(array, offset, length);
            if (this.finalized)
                throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
#endif
			ProcessInput(array.AsSpan(ibStart, cbSize));
		}

		/// <summary>
		/// Processes input bytes into the internal state using 32-bit little-endian packing.
		/// </summary>
		/// <param name="data">Input bytes to process.</param>
		private void ProcessInput(ReadOnlySpan<byte> data)
		{
			foreach (byte b in data)
			{
				// XOR byte into current 32-bit word based on bit offset
				this.state[this.bitLength / 32] ^= (uint)b << (8 * ((this.bitLength / 8) % 4));
				if ((this.bitLength += 8) == this.inputBlockSizeBits)
				{
					this.PerformRounds(this.rounds);
					this.bitLength = 0;
				}
			}
		}

		/// <inheritdoc />
		/// <summary>
		/// Finalizes the <see cref="CubeHash" /> cryptographic hash computation after all input data has been processed, and returns the
		/// resulting hash value.
		/// </summary>
		/// <returns>
		/// A byte array containing the CubeHash result. The length depends on the <see cref="HashAlgorithm.HashSize" /> setting (e.g., 256
		/// bits = 32 bytes).
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

			// Add padding bit to current word
			uint pad = (uint)(128 >> (this.bitLength % 8));
			pad <<= 8 * ((this.bitLength / 8) % 4);
			this.state[this.bitLength / 32] ^= pad;
			this.PerformRounds(this.rounds);

			// Finalization flag
			this.state[31] ^= 1U;
			this.PerformRounds(this.finalizationRounds);

			Span<byte> hash = stackalloc byte[this.HashSize / 8];
			for (int i = 0; i < hash.Length; i++)
			{
				hash[i] = (byte)(this.state[i / 4] >> (8 * (i % 4)));
			}

			return hash.ToArray();
		}

		/// <summary>
		/// Initializes the CubeHash internal state vector with parameters.
		/// </summary>
		private void InitializeVectors()
		{
			this.state = new uint[32];
			this.state[0] = (uint)this.HashSize / 8;
			this.state[1] = (uint)this.TransformBlockSize;
			this.state[2] = (uint)this.Rounds;
			this.PerformRounds(this.initializationRounds);
		}

		/// <summary>
		/// Executes the specified number of CubeHash transformation rounds on the state vector.
		/// </summary>
		/// <param name="rounds">The number of rounds to perform.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void PerformRounds(int rounds)
		{
			Span<uint> stateSpan = this.state;
			Span<uint> temp = stackalloc uint[16];

			for (int r = 0; r < rounds; r++)
			{
				// Steps 1–10 from CubeHash specification
				for (int i = 0; i < 16; i++) stateSpan[i + 16] += stateSpan[i];
				for (int i = 0; i < 16; i++) temp[i ^ 8] = stateSpan[i];
				for (int i = 0; i < 16; i++) stateSpan[i] = (temp[i] << 7) | (temp[i] >> (32 - 7));
				for (int i = 0; i < 16; i++) stateSpan[i] ^= stateSpan[i + 16];
				for (int i = 0; i < 16; i++) temp[i ^ 2] = stateSpan[i + 16];
				temp.CopyTo(stateSpan.Slice(16));
				for (int i = 0; i < 16; i++) stateSpan[i + 16] += stateSpan[i];
				for (int i = 0; i < 16; i++) temp[i ^ 4] = stateSpan[i];
				for (int i = 0; i < 16; i++) stateSpan[i] = (temp[i] << 11) | (temp[i] >> (32 - 11));
				for (int i = 0; i < 16; i++) stateSpan[i] ^= stateSpan[i + 16];
				for (int i = 0; i < 16; i++) temp[i ^ 1] = stateSpan[i + 16];
				temp.CopyTo(stateSpan.Slice(16));
			}
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (this.disposed) return;
			if (disposing)
			{
				this.finalizationRounds = this.initializationRounds = this.rounds = this.inputBlockSizeBits = this.bitLength = 0;
				if (this.state != null)
				{
					this.state.AsSpan().Clear();
					this.state = null!;
				}
			}

			this.disposed = true;
			base.Dispose(disposing);
		}

		/// <summary>
		/// Throws an exception if the object has already been disposed.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfDisposed()
		{
#if NET8_0_OR_GREATER
			ObjectDisposedException.ThrowIf(this.disposed, this);
#else
			if (this.disposed)
				throw new ObjectDisposedException(nameof(CubeHash));
#endif
		}

		/// <summary>
		/// Throws an exception if algorithm configuration is attempted after state mutation.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfInvalidState()
		{
			if (this.State != 0)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_ReconfigurationNotAllowed);
		}
	}
}