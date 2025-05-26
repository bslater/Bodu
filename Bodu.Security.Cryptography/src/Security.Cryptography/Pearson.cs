﻿using Bodu.Extensions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes the hash for the input data using the <c>Pearson</c> hash algorithm. This variant applies a non-cryptographic
	/// permutation-based transformation using a 256-byte lookup table to produce compact hash values. This class cannot be inherited.
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
	/// <note type="important">This algorithm is <b>not</b> cryptographically secure and should <b>not</b> be used for password hashing,
	/// digital signatures, or integrity validation in security-sensitive applications.</note>
	/// </remarks>
	public sealed partial class Pearson
		: System.Security.Cryptography.HashAlgorithm
	{
		/// <summary>
		/// The maximum allowable hash size in bits.
		/// </summary>
		public const int MaxHashSize = 2048;

		/// <summary>
		/// The minimum allowable hash size in bits.
		/// </summary>
		public const int MinHashSize = 8;

		private bool disposed = false;

		private bool isFirstByte;

		private byte[] permutationTable;

		private PearsonTableType tableType;

		private byte[] workingHash;

		/// <summary>
		/// Initializes a new instance of the <see cref="Pearson" /> class with a default 8-bit hash size.
		/// </summary>
		public Pearson()
		{
			permutationTable = GetPermutationTable(PearsonTableType.Pearson);
			HashSizeValue = MinHashSize;
			workingHash = new byte[HashSizeValue / 8];
			isFirstByte = true;
		}

		/// <summary>
		/// Defines the available permutation table presets that can be used with the <see cref="Pearson" /> hashing algorithm.
		/// </summary>
		public enum PearsonTableType
		{
			/// <summary>
			/// Uses the original Pearson 1990 table as the default permutation.
			/// </summary>
			/// <remarks>
			/// This handcrafted 256-byte permutation was introduced by Peter K. Pearson in 1990. It provides well-distributed hashing for
			/// 8-bit and small hash sizes.
			/// </remarks>
			Pearson,

			/// <summary>
			/// Uses the AES S-box as the permutation table.
			/// </summary>
			/// <remarks>
			/// The AES S-box is a cryptographically strong substitution box used in the AES cipher. Its non-linear mapping provides high
			/// diffusion and is useful for experimentation.
			/// </remarks>
			AESSBox,

			/// <summary>
			/// Uses the high-byte lookup table from the CRC-32 algorithm.
			/// </summary>
			/// <remarks>
			/// This permutation uses the high-order byte of CRC-32 results for all 256 input values. It reflects CRC-style distribution
			/// patterns and can be useful for parity-style hashes.
			/// </remarks>
			CRC32HighByte,

			/// <summary>
			/// Uses a permutation derived from the SHA-256 constants.
			/// </summary>
			/// <remarks>
			/// This permutation is derived from the first 64 SHA-256 constants (K values), mapped and repeated to form a 256-byte table. It
			/// introduces pseudorandom distribution with properties inspired by secure hash initialization.
			/// </remarks>
			SHA256Constants,

			/// <summary>
			/// Indicates that a custom, user-defined permutation table is being used.
			/// </summary>
			/// <remarks>
			/// When this mode is active, the table must be manually set via the <c>Table</c> property on the Pearson instance. The custom
			/// table must contain 256 unique byte values.
			/// </remarks>
			UserDefined
		}

#if !NET6_0_OR_GREATER

		// Required for .NET Standard 2.0 or older frameworks
		private bool finalized;
#endif

		/// <summary>
		/// Gets a value indicating whether this transform instance can be reused after a hash operation is completed.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the transform supports multiple hash computations via <see cref="HashAlgorithm.Initialize" />;
		/// otherwise, <see langword="false" />.
		/// </value>
		/// <remarks>
		/// Reusable transforms allow the internal state to be reset for subsequent operations using the same instance. One-shot algorithms
		/// that clear sensitive key material after finalization typically return <see langword="false" />.
		/// </remarks>
		public override bool CanReuseTransform => true;

		/// <summary>
		/// Gets a value indicating whether this transform supports processing multiple blocks of data in a single operation.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if multiple input blocks can be transformed in sequence without intermediate finalization; otherwise, <see langword="false" />.
		/// </value>
		/// <remarks>
		/// Most hash algorithms and block ciphers support multi-block transformations for streaming input. If <see langword="false" />, the
		/// transform must be invoked one block at a time.
		/// </remarks>
		public override bool CanTransformMultipleBlocks => true;

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
				Initialize();
			}
		}

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
				return permutationTable.Copy();
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				if (value == null || value.Length != 256 || value.Distinct().Count() != 256)
					throw new ArgumentException("Table must contain 256 unique bytes.", nameof(value));

				permutationTable = value.Copy();
				tableType = PearsonTableType.UserDefined;
			}
		}

		/// <summary>
		/// Gets or sets the predefined permutation table type used by the Pearson hashing algorithm.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Setting this property to a predefined value (such as <see cref="PearsonTableType.Pearson" /> or
		/// <see cref="PearsonTableType.AESSBox" />) will automatically load the corresponding 256-byte permutation table.
		/// </para>
		/// <para>
		/// Setting this property to <see cref="PearsonTableType.UserDefined" /> indicates that the table must be explicitly provided via
		/// the <see cref="Table" /> property before hashing begins. Attempting to use the algorithm without setting a table will result in
		/// an exception.
		/// </para>
		/// </remarks>
		/// <exception cref="CryptographicException">Thrown if this property is changed after hashing has begun.</exception>
		/// <exception cref="InvalidOperationException">
		/// Thrown if <see cref="PearsonTableType.UserDefined" /> is assigned and no table is later provided.
		/// </exception>
		public PearsonTableType TableType
		{
			get
			{
				ThrowIfDisposed();
				return tableType;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();

				tableType = value;

				if (value != PearsonTableType.UserDefined)
				{
					permutationTable = GetPermutationTable(value);
				}
				else
				{
					// Reset the table to an empty placeholder until explicitly set by the user
					permutationTable = null!;
				}
			}
		}

		/// <inheritdoc />
		public override void Initialize()
		{
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
            State = 0;
            finalized = false;
#endif
			workingHash = new byte[HashSizeValue / 8];
			isFirstByte = true;
		}

		/// <summary>
		/// Releases the unmanaged resources used by the algorithm and clears the key from memory.
		/// </summary>
		/// <param name="disposing">
		/// <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.
		/// </param>
		/// <remarks>This override ensures all sensitive information is zero out to avoid leaking secrets before disposal.</remarks>
		protected override void Dispose(bool disposing)
		{
			if (disposed) return;

			if (disposing)
			{
				CryptoUtilities.ClearAndNullify(ref HashValue);
				CryptoUtilities.ClearAndNullify(ref permutationTable!);
				CryptoUtilities.ClearAndNullify(ref workingHash!);

				isFirstByte = false;
			}

			disposed = true;
			base.Dispose(disposing);
		}

		/// <summary>
		/// Processes a segment of the input byte array and feeds it into the <see cref="Pearson" /> hashing algorithm. This method updates
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
			ThrowIfTableNotConfigured();

#if !NET6_0_OR_GREATER
	ThrowHelper.ThrowIfLessThan(ibStart, 0);
	ThrowHelper.ThrowIfLessThan(cbSize, 0);
	ThrowHelper.ThrowIfArrayLengthIsInsufficient(array, ibStart, cbSize);
	if (finalized)
		throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
#endif

			// Delegate to the span-based overload
			HashCore(array.AsSpan(ibStart, cbSize));
		}

		/// <summary>
		/// Processes the entirety of the input <paramref name="source" /> and feeds it into the <see cref="Pearson" /> hashing algorithm.
		/// This method updates the internal hash state accordingly by consuming the entire input span.
		/// </summary>
		/// <param name="source">The input byte span containing the data to hash.</param>
		/// <exception cref="CryptographicUnexpectedOperationException">
		/// The hash algorithm has already been finalized and cannot accept more input data.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void HashCore(ReadOnlySpan<byte> source)
		{
			ThrowIfDisposed();
			ThrowIfTableNotConfigured();

			ReadOnlySpan<byte> t = permutationTable.AsSpan();
			var v = workingHash;
			int offset = 0;

			if (isFirstByte && source.Length > 0)
			{
				byte b = source[0];
				for (int j = 0; j < v.Length; j++)
					v[j] = t[(b + j) & 0xFF];

				isFirstByte = false;
				offset = 1;
			}

			for (int i = offset; i < source.Length; i++)
			{
				byte b = source[i];
				for (int j = 0; j < v.Length; j++)
					v[j] = t[v[j] ^ b];
			}

			workingHash = v;
		}

		/// <summary>
		/// Finalizes the hash computation and returns the resulting 32-bit <see cref="Pearson" /> hash. This method reflects all input
		/// previously processed via <see cref="HashAlgorithm.HashCore(byte[], int, int)" /> or
		/// <see cref="HashAlgorithm.HashCore(ReadOnlySpan{byte})" /> and produces a final, stable hash output.
		/// </summary>
		/// <returns>
		/// A 4-byte array representing the computed <c>Pearson</c> hash value. The result is encoded in the platform’s native byte order.
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
			ThrowIfTableNotConfigured();

#if !NET6_0_OR_GREATER
			if (finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);

			finalized = true;
			State = 2;
#endif
			return workingHash.ToArray();
		}

		/// <summary>
		/// Returns the predefined permutation table for the specified <see cref="PearsonTableType" />.
		/// </summary>
		/// <param name="type">The type of permutation table to retrieve.</param>
		/// <returns>A 256-byte permutation table.</returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown when <paramref name="type" /> is <see cref="PearsonTableType.UserDefined" />. In this case, the table must be explicitly
		/// provided via the <c>Table</c> property on the <see cref="Pearson" /> instance.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="type" /> is not a recognized value.</exception>
		private static byte[] GetPermutationTable(PearsonTableType type) =>
			type switch
			{
				PearsonTableType.Pearson => PearsonTable.Value.ToArray(),
				PearsonTableType.AESSBox => AESSBoxTable.Value.ToArray(),
				PearsonTableType.CRC32HighByte => CRC32HighByteTable.Value.ToArray(),
				PearsonTableType.SHA256Constants => SHA256ConstantsTable.Value.ToArray(),
				PearsonTableType.UserDefined => throw new InvalidOperationException(
					"UserDefined t type requires an explicit 256-byte permutation t to be set using the Table property."),
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown PearsonTableType workingHash.")
			};

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
				throw new ObjectDisposedException(nameof(Elf64));
#endif
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
		/// Throws an <see cref="CryptographicUnexpectedOperationException" /> if the permutation table has not been configured or is invalid.
		/// </summary>
		/// <exception cref="CryptographicUnexpectedOperationException">
		/// Thrown if <see cref="Table" /> is <c>null</c> or does not contain exactly 256 bytes. This typically indicates that the consumer
		/// must explicitly assign a table when using <see cref="PearsonTableType.UserDefined" />.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfTableNotConfigured()
		{
			if (permutationTable == null || permutationTable.Length != 256)
			{
				throw new CryptographicUnexpectedOperationException(
					$"A valid 256-byte permutation t must be set before hashing. " +
					$"Ensure that the {nameof(Table)} property is explicitly assigned if using {nameof(PearsonTableType.UserDefined)}.");
			}
		}
	}
}