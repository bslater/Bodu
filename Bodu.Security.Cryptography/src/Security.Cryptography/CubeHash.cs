// ---------------------------------------------------------------------------------------------------------------
// <copyright file="CubeHash.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes the hash for the input data using the <c>CubeHash</c> hash algorithm. This variant applies a cryptographic permutation over
	/// a 1024-bit state using a configurable number of initialization, transformation, and finalization rounds. This class cannot be inherited.
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
		private bool disposed = false;
		private readonly uint[] scratch = new uint[16];
		private bool isInitializedStateCached = false;
		private uint[] initializedState;

#if !NET6_0_OR_GREATER
        private bool finalized; // flag to block reuse in older .NET
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="CubeHash" /> class with default parameters.
		/// </summary>
		public CubeHash()
		{
			HashSize = 512;
			TransformBlockSize = 32;
			Rounds = InitializationRounds = 16;
			FinalizationRounds = 32;
			bitLength = 0;
			state = initializedState = new uint[32];
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
		public override int InputBlockSize => TransformBlockSize;

		/// <inheritdoc />
		/// <summary>
		/// Gets the output block size, in bytes, of the final computed hash value.
		/// </summary>
		/// <remarks>
		/// This is equal to the configured <see cref="HashSize" /> divided by 8. For example, a 512-bit hash will produce an output block
		/// of 64 bytes. This value corresponds to the full digest returned after <see cref="HashAlgorithm.HashFinal" /> is called.
		/// </remarks>
		public override int OutputBlockSize => HashSize / 8;

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
				return finalizationRounds;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				ThrowHelper.ThrowIfOutOfRange(value, MinRounds, MaxRounds);
				finalizationRounds = value;
				isInitializedStateCached = false;
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
				return HashSizeValue;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				ThrowHelper.ThrowIfOutOfRange(value, MinHashSize, MaxHashSize);
				ThrowHelper.ThrowIfNotPositiveMultipleOf(value, 8);
				HashSizeValue = value;
				isInitializedStateCached = false;
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
				return initializationRounds;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				ThrowHelper.ThrowIfOutOfRange(value, MinRounds, MaxRounds);
				initializationRounds = value;
				isInitializedStateCached = false;
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
				return inputBlockSizeBits / 8;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				ThrowHelper.ThrowIfOutOfRange(value, MinInputBlockSize, MaxInputBlockSize);
				inputBlockSizeBits = value * 8;
				isInitializedStateCached = false;
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
				return rounds;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				ThrowHelper.ThrowIfOutOfRange(value, MinRounds, MaxRounds);
				rounds = value;
				isInitializedStateCached = false;
			}
		}

		/// <summary>
		/// Gets the full CubeHash algorithm name in the canonical submission format.
		/// </summary>
		/// <remarks>
		/// <para>Follows the CubeHash naming convention from the original submission: <c>CubeHashr+b/w+f-h</c>, where:</para>
		/// <list type="bullet">
		/// <item>
		/// <description><c>r</c> = number of initialization rounds</description>
		/// </item>
		/// <item>
		/// <description><c>b</c> = number of transformation rounds per block</description>
		/// </item>
		/// <item>
		/// <description><c>w</c> = block size in bytes</description>
		/// </item>
		/// <item>
		/// <description><c>f</c> = number of finalization rounds</description>
		/// </item>
		/// <item>
		/// <description><c>h</c> = hash size in bits</description>
		/// </item>
		/// </list>
		/// <para>Example: <c>CubeHash16+32/32+32-256</c></para>
		/// </remarks>
		public string AlgorithmName =>
			$"CubeHash{InitializationRounds}+{Rounds}/{TransformBlockSize}+{FinalizationRounds}-{HashSize}";

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
            State = 0;
            finalized = false;
#endif
			bitLength = 0;

			EnsureInitialized();
			InitializeVectors();
		}

		/// <summary>
		/// Processes a segment of the input byte array and feeds it into the <see cref="CubeHash" /> hashing algorithm. This method updates
		/// the internal state by processing <paramref name="cbSize" /> bytes starting at the specified <paramref name="ibStart" /> offset.
		/// </summary>
		/// <param name="array">The input byte array containing the data to hash.</param>
		/// <param name="ibStart">The zero-based index in <paramref name="array" /> at which to begin reading data.</param>
		/// <param name="cbSize">The number of bytes to process from <paramref name="array" />.</param>
		/// <exception cref="ArgumentNullException"><paramref name="array" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <para><paramref name="ibStart" /> is less than 0.</para>
		/// <para>-or-</para>
		/// <para><paramref name="cbSize" /> is less than 0.</para>
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="ibStart" /> and <paramref name="cbSize" /> specify a range that exceeds the length of <paramref name="array" />.
		/// </exception>
		/// <exception cref="CryptographicUnexpectedOperationException">
		/// The hash algorithm has already been finalized and cannot accept more input data.
		/// </exception>
		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			ThrowHelper.ThrowIfNull(array);
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
            ThrowHelper.ThrowIfLessThan(offset, 0);
            ThrowHelper.ThrowIfLessThan(length, 0);
            ThrowHelper.ThrowIfArrayLengthIsInsufficient(array, offset, length);
            if (finalized)
                throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
#endif
			EnsureInitialized();
			HashCore(array.AsSpan(ibStart, cbSize));
		}

		/// <summary>
		/// Processes the entirety of the input <paramref name="source" /> and feeds it into the <see cref="CubeHash" /> hashing algorithm.
		/// This method updates the internal hash state accordingly by consuming the entire input span.
		/// </summary>
		/// <param name="source">The input byte span containing the data to hash.</param>
		/// <exception cref="CryptographicUnexpectedOperationException">
		/// The hash algorithm has already been finalized and cannot accept more input data.
		/// </exception>
		protected override void HashCore(ReadOnlySpan<byte> source)
		{
			ThrowIfDisposed();
			EnsureInitialized();

			foreach (var b in source)
			{
				// XOR byte into current 32-bit word based on bit offset
				state[bitLength / 32] ^= (uint)b << (8 * ((bitLength / 8) % 4));
				if ((bitLength += 8) == inputBlockSizeBits)
				{
					PerformRounds(rounds);
					bitLength = 0;
				}
			}
		}

		/// <summary>
		/// Finalizes the hash computation and returns the resulting <see cref="CubeHash" /> hash in big-endian format. This method reflects
		/// all input previously processed via <see cref="HashAlgorithm.HashCore(byte[], int, int)" /> or
		/// <see cref="HashAlgorithm.HashCore(ReadOnlySpan{byte})" /> and produces a final, stable hash output.
		/// </summary>
		/// <returns>
		/// A byte array representing the computed hash value. The size of the array is determined by the algorithm�s configured
		/// <see cref="HashAlgorithm.HashSize" /> and is encoded in <b>big-endian</b> byte order.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method completes the internal state of the hashing algorithm and serializes the final hash value into a
		/// platform-independent format. It is invoked automatically by <see cref="HashAlgorithm.ComputeHash(byte[])" /> and related methods
		/// once all data has been processed.
		/// </para>
		/// <para>After this method returns, the internal state is considered finalized and the computed hash is stable.</para>
		/// <para>
		/// In .NET 6.0 and later, the algorithm is automatically reset by invoking <see cref="HashAlgorithm.Initialize" />, allowing the
		/// instance to be reused immediately.
		/// </para>
		/// <para>
		/// In earlier versions of .NET, the internal state is marked as finalized, and any subsequent calls to
		/// <see cref="HashAlgorithm.HashCore(byte[], int, int)" />, <see cref="HashAlgorithm.HashCore(ReadOnlySpan{byte})" />, or
		/// <see cref="HashAlgorithm.HashFinal" /> will throw a <see cref="CryptographicUnexpectedOperationException" />. To compute another
		/// hash, you must explicitly call <see cref="HashAlgorithm.Initialize" /> to reset the algorithm.
		/// </para>
		/// <para>
		/// Implementations should ensure all residual or pending data is processed and integrated into the final hash value before returning.
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
			EnsureInitialized();

			// Add padding bit to current word
			uint pad = (uint)(128 >> (bitLength % 8));
			pad <<= 8 * ((bitLength / 8) % 4);
			state[bitLength / 32] ^= pad;
			PerformRounds(rounds);

			// Finalization flag
			state[31] ^= 1U;
			PerformRounds(finalizationRounds);

			int byteLength = HashSize / 8;
			byte[] result = GC.AllocateUninitializedArray<byte>(byteLength);
			for (int i = 0; i < byteLength; i++)
			{
				result[i] = (byte)(state[i / 4] >> (8 * (i % 4)));
			}

			return result;
		}

		/// <summary>
		/// Initializes the CubeHash internal state vector with parameters.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureInitialized()
		{
			if (isInitializedStateCached)
				return;

			state = new uint[32];
			state[0] = (uint)HashSize / 8;
			state[1] = (uint)TransformBlockSize;
			state[2] = (uint)Rounds;
			PerformRounds(initializationRounds);
			initializedState = state.ToArray();
			isInitializedStateCached = true;
		}

		private void InitializeVectors()
		{
			state = initializedState.ToArray();
		}

		/// <summary>
		/// Executes the specified number of CubeHash transformation rounds on the state vector.
		/// </summary>
		/// <param name="rounds">The number of rounds to perform.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void PerformRounds(int rounds)
		{
			Span<uint> stateSpan = state;
			Span<uint> temp = scratch;

			for (int r = 0; r < rounds; r++)
			{
				// Step 1: Add lower and upper halves
				for (int i = 0; i < 16; i++)
					stateSpan[i + 16] += stateSpan[i];

				// Step 2: XOR permutation (XOR with 8)
				for (int i = 0; i < 16; i++)
					temp[i ^ 8] = stateSpan[i];

				// Step 3: Rotate left by 7
				for (int i = 0; i < 16; i++)
					stateSpan[i] = BitOperations.RotateLeft(temp[i], 7);

				// Step 4: XOR lower with upper again
				for (int i = 0; i < 16; i++)
					stateSpan[i] ^= stateSpan[i + 16];

				// Step 5: Second permutation (XOR with 2)
				for (int i = 0; i < 16; i++)
					temp[i ^ 2] = stateSpan[i + 16];
				temp.CopyTo(stateSpan.Slice(16));

				// Step 6: Add again
				for (int i = 0; i < 16; i++)
					stateSpan[i + 16] += stateSpan[i];

				// Step 7: Permute lower (XOR with 4)
				for (int i = 0; i < 16; i++)
					temp[i ^ 4] = stateSpan[i];

				// Step 8: Rotate left by 11
				for (int i = 0; i < 16; i++)
					stateSpan[i] = BitOperations.RotateLeft(temp[i], 11);

				// Step 9: Final XOR
				for (int i = 0; i < 16; i++)
					stateSpan[i] ^= stateSpan[i + 16];

				// Step 10: Final permutation (XOR with 1)
				for (int i = 0; i < 16; i++)
					temp[i ^ 1] = stateSpan[i + 16];

				temp.CopyTo(stateSpan.Slice(16));
			}
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (disposed) return;
			if (disposing)
			{
				finalizationRounds = initializationRounds = rounds = inputBlockSizeBits = bitLength = 0;
				if (state != null)
				{
					CryptoUtilities.ClearAndNullify(ref HashValue);
					CryptoUtilities.ClearAndNullify(ref state!);
					CryptoUtilities.ClearAndNullify(ref initializedState!);
					CryptoUtilities.Clear(scratch.AsSpan());
					isInitializedStateCached = false;
				}
			}

			disposed = true;
			base.Dispose(disposing);
		}

		/// <summary>
		/// Throws an exception if the object has already been disposed.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfDisposed()
		{
#if NET8_0_OR_GREATER
			ObjectDisposedException.ThrowIf(disposed, this);
#else
			if (disposed)
				throw new ObjectDisposedException(nameof(CubeHash));
#endif
		}

		/// <summary>
		/// Throws an exception if algorithm configuration is attempted after state mutation.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfInvalidState()
		{
			if (State != 0)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_ReconfigurationNotAllowed);
		}
	}
}