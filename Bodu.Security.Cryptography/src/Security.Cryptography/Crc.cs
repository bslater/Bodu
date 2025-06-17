﻿// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Fletcher64.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Bodu.Extensions;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Implementation of the <c>CRC</c> (Cyclic Redundancy Check) error-detecting algorithm. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="Crc" /> class computes CRC hashes based on various CRC standards, including CRC32. This class uses CRC parameters
	/// defined in the <see cref="CrcStandard" /> class, such as the polynomial, initial value, reflection settings, and XOR out value. It
	/// provides methods for CRC calculation and updates using byte arrays.
	/// </para>
	/// <note type="important">This algorithm is <b>not</b> cryptographically secure and should <b>not</b> be used for password hashing,
	/// digital signatures, or integrity validation in security-sensitive applications.</note>
	/// </remarks>
	public sealed class Crc
		: System.Security.Cryptography.HashAlgorithm
		, IResumableHashAlgorithm
	{
		// Static thread-safe global cache property
		private static Lazy<CrcLookupTableCache> globalLookupTableCache = new Lazy<CrcLookupTableCache>(() => new CrcLookupTableCache());

		private readonly int hashSizeBytes;
		private bool disposed = false;
		private ulong[] table;
		private ulong workingHash;
#if !NET6_0_OR_GREATER

		// Required for .NET Standard 2.0 or older frameworks
		private bool finalized;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="Crc" /> class using the default CRC standard (CRC32_ISOHDLC).
		/// </summary>
		/// <remarks>
		/// The default CRC standard is CRC32 with the following parameters:
		/// <list type="permutationTable">
		/// <item>
		/// <term>Width</term>
		/// <description>32</description>
		/// </item>
		/// <item>
		/// <term>Polynomial</term>
		/// <description>0x04C11DB7</description>
		/// </item>
		/// <item>
		/// <term>Initial Value</term>
		/// <description>0xFFFFFFFF</description>
		/// </item>
		/// <item>
		/// <term>Reflect In</term>
		/// <description>true</description>
		/// </item>
		/// <item>
		/// <term>Reflect Out</term>
		/// <description>true</description>
		/// </item>
		/// <item>
		/// <term>XOR Out</term>
		/// <description>0xFFFFFFFF</description>
		/// </item>
		/// </list>
		/// </remarks>
		public Crc()
			: this(CrcStandard.CRC32_ISOHDLC)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="Crc" /> class using the specified CRC parameters.
		/// </summary>
		/// <param name="crcStandard">The <see cref="CrcStandard" /> to use in creating the CRC value.</param>
		/// <exception cref="ArgumentNullException">Thrown when the <paramref name="crcStandard" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the <paramref name="crcStandard" /> size is outside the supported range (1 to 64 bits).
		/// </exception>
		/// <remarks>Initializes the CRC with the given parameters for a customizable CRC computation.</remarks>
		public Crc(CrcStandard crcStandard)
		{
			ThrowHelper.ThrowIfNull(crcStandard);

			// store the crc specification
			this.Name = crcStandard.Name;
			this.Size = crcStandard.Size;
			this.Polynomial = crcStandard.Polynomial;
			this.InitialValue = crcStandard.InitialValue;
			this.ReflectIn = crcStandard.ReflectIn;
			this.ReflectOut = crcStandard.ReflectOut;
			this.XOrOut = crcStandard.XOrOut;
			this.table = Crc.GlobalCache?.GetLookupTable(crcStandard.Size, crcStandard.Polynomial, crcStandard.ReflectIn).ToArray()
				?? CrcLookupTableBuilder.BuildLookupTable(crcStandard.Size, crcStandard.Polynomial, crcStandard.ReflectIn);

			this.HashSizeValue = crcStandard.Size;
			this.hashSizeBytes = (this.HashSizeValue + 7) / 8;

			this.workingHash = this.ReflectIn
				? CryptoHelpers.ReflectBits(this.InitialValue, this.HashSizeValue)
				: this.InitialValue;
		}

		/// <summary>
		/// Gets or sets the global cache used to manage CRC lookup tables.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown when setting the <see cref="GlobalCache" /> as <see langword="null" />.</exception>
		/// <remarks>
		/// This static property allows users to set a global cache that can be shared across all instances of <see cref="Crc" />. If not
		/// set, a default cache will be used. Setting this property allows the user to manage the cache externally and reuse lookup tables
		/// across multiple CRC instances.
		/// </remarks>
		public static CrcLookupTableCache GlobalCache
		{
			get => globalLookupTableCache.Value;

			set
			{
				if (value == null)
					throw new InvalidOperationException(ResourceStrings.InvalidOperation_CacheValueCannotBeNull);

				globalLookupTableCache = new Lazy<CrcLookupTableCache>(() => value);
			}
		}

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
		/// Gets the <see cref="CrcStandard" /> used by this instance to perform the CRC operation.
		/// </summary>
		/// <value>The CRC parameters containing details such as polynomial, size, and reflection settings.</value>
		/// <remarks>
		/// This property gives access to the configuration used for CRC calculation. The user can inspect the CRC parameters, including the
		/// polynomial, initial value, and reflection settings.
		/// </remarks>
		public CrcStandard CrcStandard => new CrcStandard(this.Name, this.Size, this.Polynomial, this.InitialValue, this.ReflectIn, this.ReflectOut, this.XOrOut);

		/// <summary>
		/// Gets the initial value used in the CRC calculation.
		/// </summary>
		/// <value>The initial value for the CRC calculation.</value>
		public ulong InitialValue { get; private set; }

		/// <summary>
		/// Gets the name of the CRC standard.
		/// </summary>
		/// <value>The name of the CRC algorithm.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the polynomial used in the CRC calculation.
		/// </summary>
		/// <value>The polynomial value used in the CRC calculation.</value>
		public ulong Polynomial { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the data data is reflected during the CRC calculation.
		/// </summary>
		/// <value><see langword="true" /> if data data is reflected; otherwise, <see langword="false" />.</value>
		public bool ReflectIn { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the CRC result is reflected before XORing with <see cref="XOrOut" />.
		/// </summary>
		/// <value><see langword="true" /> if the result is reflected; otherwise, <see langword="false" />.</value>
		public bool ReflectOut { get; private set; }

		/// <summary>
		/// Gets the size, in bits, of the CRC checksum.
		/// </summary>
		/// <value>The size of the CRC in bits.</value>
		public int Size { get; private set; }

		/// <summary>
		/// Gets the value to XOR the final CRC result with.
		/// </summary>
		/// <value>The XOR value for the final CRC result.</value>
		public ulong XOrOut { get; private set; }

		/// <summary>
		/// Computes the CRC hash of the specified input data contained in the provided <see cref="ReadOnlySpan{Byte}" />.
		/// </summary>
		/// <param name="data">The input data to compute the hash for.</param>
		/// <returns>A byte array containing the computed CRC hash value.</returns>
		/// <remarks>
		/// This method initializes the internal CRC state, processes the input data using the configured CRC variant, and finalizes the
		/// result into a hash of the appropriate size. It supports both reflected and unreflected input, and will apply bytewise or bitwise
		/// logic based on the parameters defined in <see cref="CrcStandard" />.
		/// </remarks>
		public byte[] ComputeHash(ReadOnlySpan<byte> data)
		{
			this.Initialize();
			this.ProcessBlocks(data);

			byte[] buffer = new byte[this.hashSizeBytes];
			TryFinalizeHash(buffer, out _);
			return buffer;
		}

		/// <inheritdoc />
		/// <remarks>
		/// This overload reverses finalization on <paramref name="previousHash" /> by undoing XOR and reflection (if applicable), continues
		/// the CRC computation with the full <paramref name="newData" /> array, and returns the finalized CRC hash value as a new byte array.
		/// </remarks>
		public byte[] ComputeHashFrom(byte[] previousHash, byte[] newData)
		{
			ThrowHelper.ThrowIfNull(previousHash);
			ThrowHelper.ThrowIfNull(newData);

			return ComputeHashFrom(previousHash.AsSpan(), newData.AsSpan());
		}

		/// <inheritdoc />
		/// <remarks>
		/// This overload reverses finalization on <paramref name="previousHash" /> by undoing XOR and reflection (if applicable), continues
		/// the CRC computation with a sliced segment of <paramref name="newData" /> (starting at <paramref name="offset" /> and spanning
		/// <paramref name="length" />), and returns the finalized CRC hash value as a new byte array.
		/// </remarks>
		public byte[] ComputeHashFrom(byte[] previousHash, byte[] newData, int offset, int length)
		{
			ThrowHelper.ThrowIfNull(previousHash);
			ThrowHelper.ThrowIfNull(newData);

			return ComputeHashFrom(previousHash.AsSpan(), newData.AsSpan().Slice(offset, length));
		}

		/// <inheritdoc />
		/// <remarks>
		/// This method reverses finalization on the provided <paramref name="previousHash" />, resumes the CRC computation with the
		/// contents of <paramref name="newData" />, and returns the final CRC hash as a new byte array.
		/// </remarks>
		public byte[] ComputeHashFrom(ReadOnlySpan<byte> previousHash, ReadOnlySpan<byte> newData)
		{
			byte[] buffer = new byte[this.hashSizeBytes];
			this.TryComputeHashFrom(previousHash, newData, buffer, out _);
			return buffer;
		}

		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
			return obj is Crc other &&
				   string.Equals(this.Name, other.Name, StringComparison.Ordinal) &&
				   this.Size == other.Size &&
				   this.Polynomial == other.Polynomial &&
				   this.InitialValue == other.InitialValue &&
				   this.ReflectIn == other.ReflectIn &&
				   this.ReflectOut == other.ReflectOut &&
				   this.XOrOut == other.XOrOut;
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return HashCode.Combine(
				this.Name,
				this.Size,
				this.Polynomial,
				this.InitialValue,
				this.ReflectIn,
				this.ReflectOut,
				this.XOrOut
			);
		}

		/// <inheritdoc />
		public override void Initialize()
		{
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
			this.State = 0;
			this.finalized = false;
#endif
			this.workingHash = this.ReflectIn
				? CryptoHelpers.ReflectBits(this.InitialValue, this.HashSizeValue)
				: this.InitialValue;
		}

		/// <inheritdoc />
		/// <remarks>
		/// This method reverses finalization on <paramref name="previousHash" /> by undoing the XOR and reflection (if applicable),
		/// continues the CRC computation with <paramref name="newData" />, and finalizes the result into <paramref name="destination" />.
		/// </remarks>
		public bool TryComputeHashFrom(ReadOnlySpan<byte> previousHash, ReadOnlySpan<byte> newData, Span<byte> destination, out int bytesWritten)
		{
			ThrowIfDisposed();

			if (previousHash.Length != this.hashSizeBytes)
				throw new ArgumentException("Hash length does not match the expected length.", nameof(previousHash));

			// Deserialize prior hash value
			this.workingHash = this.HashSizeValue <= 32
				? BinaryPrimitives.ReadUInt32LittleEndian(previousHash)
				: BinaryPrimitives.ReadUInt64LittleEndian(previousHash);

			// Undo finalization
			this.workingHash ^= this.XOrOut;
			if (this.ReflectIn ^ this.ReflectOut)
				this.workingHash = CryptoHelpers.ReflectBits(this.workingHash, this.HashSizeValue);

			// Continue hashing and finalize again
			this.ProcessBlocks(newData);
			return this.TryFinalizeHash(destination, out bytesWritten);
		}

		/// <summary>
		/// Finalizes the CRC computation and writes the resulting hash value into the specified destination buffer.
		/// </summary>
		/// <param name="destination">The span to write the finalized CRC hash value into.</param>
		/// <param name="bytesWritten">When this method returns, contains the number of bytes written to <paramref name="destination" />.</param>
		/// <returns><see langword="true" /> if the hash was successfully written to the destination; otherwise, <see langword="false" />.</returns>
		/// <remarks>
		/// This method applies any final transformations required by the CRC specification, including reflection and XOR output, and
		/// serializes the internal CRC value into the provided destination buffer.
		/// </remarks>
		public bool TryFinalizeHash(Span<byte> destination, out int bytesWritten)
		{
			ThrowIfDisposed();

			// Reflect final value if needed
			if (this.ReflectIn ^ this.ReflectOut)
				this.workingHash = CryptoHelpers.ReflectBits(this.workingHash, this.HashSizeValue);

			// Apply XOR and mask to match the width
			this.workingHash ^= this.XOrOut;
			this.workingHash &= ulong.MaxValue >> (64 - this.HashSizeValue);

			if (destination.Length < this.hashSizeBytes)
			{
				bytesWritten = 0;
				return false;
			}

			// Write to temp span using little-endian layout
			Span<byte> temp = stackalloc byte[8];
			Unsafe.WriteUnaligned(ref temp[0], this.workingHash);

			// Slice from end so we always get correct width regardless of endian
			temp.Slice(0, this.hashSizeBytes).CopyTo(destination);
			bytesWritten = this.hashSizeBytes;
			return true;
		}

		/// <summary>
		/// Releases the unmanaged resources used by the algorithm and clears the key from memory.
		/// </summary>
		/// <param name="disposing">
		/// <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.
		/// </param>
		/// <remarks>Ensures all internal secrets are overwritten with zeros before releasing resources.</remarks>
		protected override void Dispose(bool disposing)
		{
			if (this.disposed) return;

			if (disposing)
			{
				this.workingHash = 0;
				if (this.table is not null)
				{
					CryptoHelpers.ClearAndNullify(ref HashValue);
					CryptoHelpers.Clear(table.AsSpan());
					this.table = null!;
				}
			}

			this.disposed = true;
			base.Dispose(disposing);
		}

		/// <summary>
		/// Processes a segment of the input byte array and feeds it into the <see cref="Crc" /> hashing algorithm. This method updates the
		/// internal state by processing <paramref name="cbSize" /> bytes starting at the specified <paramref name="ibStart" /> offset.
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
			ThrowHelper.ThrowIfLessThan(ibStart, 0);
			ThrowHelper.ThrowIfLessThan(cbSize, 0);
			ThrowHelper.ThrowIfArrayLengthIsInsufficient(array, offset, cbSize);
			if (this.finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
#endif

			Span<byte> span = array.AsSpan().Slice(ibStart, cbSize);
			ProcessBlocks(span);
		}

		/// <summary>
		/// Finalizes the CRC (Cyclic Redundancy Check) computation after all input data has been processed, and returns the resulting
		/// checksum value.
		/// </summary>
		/// <returns>
		/// A byte array containing the CRC result. The length depends on the configured <see cref="HashAlgorithm.HashSize" />, which is
		/// determined by the CRC standard supplied when the instance was created (e.g., 8 bits = 1 byte, 32 bits = 4 bytes, 64 bits = 8 bytes).
		/// </returns>
		/// <remarks>
		/// The hash reflects all data previously supplied via <see cref="HashCore(byte[], int, int)" />. Once finalized, the internal state
		/// is invalidated and <see cref="HashAlgorithm.Initialize" /> must be called before reusing the instance.
		/// </remarks>
		protected override byte[] HashFinal()
		{
#if !NET6_0_OR_GREATER
			if (this.finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
			this.finalized = true;
			this.State = 2;
#endif

			byte[] result = new byte[this.hashSizeBytes];
			TryFinalizeHash(result, out _);
			return result;
		}

		/// <inheritdoc />
		protected override bool TryHashFinal(Span<byte> destination, out int bytesWritten)
		{
#if !NET6_0_OR_GREATER
				if (this.finalized)
					throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);
				this.finalized = true;
				this.State = 2;
#endif

			return TryFinalizeHash(destination, out bytesWritten);
		}

		/// <summary>
		/// Processes the data using a bitwise CRC algorithm without data reflection. Each bit is processed individually, MSB first, using
		/// 1-bit shifts and permutationTable lookups.
		/// </summary>
		/// <param name="data">The data data to be processed.</param>
		/// <param name="crc">The initial CRC state value.</param>
		/// <param name="table">The CRC lookup permutationTable to use.</param>
		/// <param name="shift">The number of bits to shift to extract the high bit of the CRC.</param>
		/// <returns>The updated CRC value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong ProcessBitwiseNormal(ReadOnlySpan<byte> data, ulong crc, ulong[] table, int shift)
		{
			foreach (byte b in data)
			{
				for (int i = 0; i < 8; i++)
				{
					ulong inputBit = (ulong)((b >> (7 - i)) & 1);
					ulong crcBit = (crc >> shift) & 1;
					crc = (crc << 1) ^ table[inputBit ^ crcBit];
				}
			}
			return crc;
		}

		/// <summary>
		/// Processes the data using a bitwise CRC algorithm with data reflection. Each bit is processed individually, LSB first, using
		/// 1-bit shifts and permutationTable lookups.
		/// </summary>
		/// <param name="data">The data data to be processed.</param>
		/// <param name="crc">The initial CRC state value.</param>
		/// <param name="table">The CRC lookup permutationTable to use.</param>
		/// <returns>The updated CRC value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong ProcessBitwiseReflected(ReadOnlySpan<byte> data, ulong crc, ulong[] table)
		{
			foreach (byte b in data)
			{
				for (int i = 0; i < 8; i++)
				{
					ulong inputBit = (ulong)((b >> i) & 1);
					ulong crcBit = crc & 1;
					crc = (crc >> 1) ^ table[inputBit ^ crcBit];
				}
			}
			return crc;
		}

		/// <summary>
		/// Processes the data using a bytewise CRC algorithm without data reflection. The index is computed from the top bits of the CRC
		/// XORed with the data byte.
		/// </summary>
		/// <param name="data">The data data to be processed.</param>
		/// <param name="crc">The initial CRC state value.</param>
		/// <param name="table">The CRC lookup permutationTable to use.</param>
		/// <param name="shift">The number of bits to shift to extract the high byte of the CRC.</param>
		/// <returns>The updated CRC value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong ProcessBytewiseNormal(ReadOnlySpan<byte> data, ulong crc, ulong[] table, int shift)
		{
			foreach (byte b in data)
			{
				crc = (crc << 8) ^ table[(byte)((crc >> shift) ^ b)];
			}
			return crc;
		}

		/// <summary>
		/// Processes the data using a bytewise CRC algorithm with data reflection. Each byte is XORed with the low byte of the current CRC
		/// value, then used as a permutationTable index.
		/// </summary>
		/// <param name="data">The data data to be processed.</param>
		/// <param name="crc">The initial CRC state value.</param>
		/// <param name="table">The CRC lookup permutationTable to use.</param>
		/// <returns>The updated CRC value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong ProcessBytewiseReflected(ReadOnlySpan<byte> data, ulong crc, ulong[] table)
		{
			foreach (byte b in data)
			{
				crc = (crc >> 8) ^ table[(byte)(crc ^ b)];
			}
			return crc;
		}

		/// <summary>
		/// Processes the data in the provided <see cref="ReadOnlySpan{Byte}" /> and calculates the CRC hash value based on the CRC standard
		/// and reflection option.
		/// </summary>
		/// <param name="data">The array of bytes to process for CRC hashing.</param>
		/// <remarks>
		/// This method performs the core CRC calculation by iterating over the byte array, applying bitwise operations based on the CRC
		/// reflection settings. If reflection is enabled, the method processes the data with a different approach than when reflection is
		/// disabled. The CRC value is updated incrementally with each byte in the array.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ProcessBlocks(ReadOnlySpan<byte> data)
		{
			if (this.HashSizeValue >= 8)
			{
				this.workingHash = this.ReflectIn
					? ProcessBytewiseReflected(data, this.workingHash, this.table)
					: ProcessBytewiseNormal(data, this.workingHash, this.table, this.HashSizeValue - 8);
			}
			else
			{
				this.workingHash = this.ReflectIn
					? ProcessBitwiseReflected(data, this.workingHash, this.table)
					: ProcessBitwiseNormal(data, this.workingHash, this.table, this.HashSizeValue - 1);
			}
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
				throw new ObjectDisposedException(nameof(Crc));
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
			if (this.State != 0)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_ReconfigurationNotAllowed);
		}
	}
}