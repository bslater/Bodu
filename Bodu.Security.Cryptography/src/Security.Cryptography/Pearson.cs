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

		/// <summary>
		/// The minimum allowable hash size in bits.
		/// </summary>
		public const int MinHashSize = 8;

		/// <summary>
		/// The maximum allowable hash size in bits.
		/// </summary>
		public const int MaxHashSize = 2048;

		private static readonly Lazy<byte[]> PearsonTable = new(() => new byte[256]
		{
			0x01, 0x87, 0x99, 0x17, 0x1C, 0x42, 0x5D, 0x73, 0x67, 0x4A, 0x23, 0x21, 0x24, 0x0C, 0x8A, 0xB9,
			0xE3, 0x4B, 0xC0, 0x2E, 0xA6, 0x5F, 0x4C, 0xF9, 0x0B, 0xDE, 0x1D, 0xB0, 0x6C, 0xA1, 0x1B, 0x88,
			0xC8, 0x09, 0xFA, 0x2F, 0x89, 0x38, 0xA5, 0x7D, 0x61, 0x3C, 0x90, 0x41, 0xD9, 0xA0, 0x0E, 0x5C,
			0xBC, 0x27, 0xF8, 0x92, 0x45, 0x40, 0x62, 0xA9, 0xB8, 0x0D, 0x44, 0x54, 0x26, 0xB2, 0x7B, 0x7F,
			0xEE, 0x8C, 0xC1, 0x86, 0xA2, 0xB3, 0x3A, 0x0F, 0x74, 0x50, 0xC2, 0xFF, 0x6F, 0xEA, 0x69, 0x20,
			0x36, 0xE1, 0xE8, 0x99, 0x2D, 0x6A, 0xBF, 0x60, 0x2A, 0xD4, 0x25, 0xCA, 0x91, 0xC4, 0x55, 0x30,
			0xA4, 0xB1, 0xE5, 0x28, 0x33, 0x8E, 0xD3, 0x31, 0x6D, 0xFD, 0x78, 0xC9, 0x12, 0xF6, 0xF5, 0x37,
			0xDC, 0x0A, 0x4F, 0xC6, 0x3B, 0xD2, 0xEF, 0xA3, 0x46, 0x22, 0x4E, 0x3D, 0x57, 0xAA, 0x72, 0x63,
			0xA7, 0x5E, 0x43, 0xE0, 0x93, 0x39, 0x07, 0x6B, 0xF2, 0xE6, 0xCE, 0xB7, 0x95, 0x94, 0xD5, 0xF7,
			0xD7, 0x76, 0xB6, 0xBE, 0xC5, 0x34, 0x64, 0xF4, 0x10, 0x83, 0xCD, 0xF3, 0xAD, 0x66, 0xA8, 0x3F,
			0xE9, 0x9B, 0x19, 0x58, 0x6E, 0xA1, 0x18, 0x8B, 0xA0, 0x03, 0x70, 0x97, 0x9F, 0xBA, 0xC7, 0x15,
			0x7E, 0x53, 0x32, 0x56, 0xDB, 0xD0, 0xF1, 0xB4, 0x71, 0x68, 0xE4, 0x84, 0x77, 0xFE, 0xBB, 0x11,
			0xC3, 0x13, 0xE2, 0x59, 0xD8, 0xD6, 0x9A, 0xB5, 0xC1, 0xEB, 0x5B, 0x02, 0x00, 0x7A, 0x98, 0x75,
			0xDF, 0x96, 0x29, 0xD1, 0x48, 0xE7, 0xAF, 0x47, 0x06, 0x9D, 0x1E, 0x1A, 0x5A, 0x14, 0xAC, 0x49,
			0x16, 0x79, 0xBA, 0x8D, 0xB0, 0x2C, 0x35, 0x85, 0xDD, 0x7C, 0x52, 0x9C, 0x80, 0x1F, 0x08, 0xD1,
			0x8F, 0x2B, 0x9E, 0xED, 0x17, 0xC8, 0xD9, 0xC0, 0xF0, 0x81, 0x1C, 0x26, 0x0E, 0xEC, 0x04, 0x35
		});

		private static readonly Lazy<byte[]> AESSBoxTable = new(() => new byte[256]
		{
			0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5, 0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,
			0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0, 0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,
			0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc, 0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,
			0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a, 0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,
			0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0, 0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,
			0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b, 0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
			0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85, 0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,
			0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5, 0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,
			0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17, 0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,
			0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88, 0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,
			0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c, 0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,
			0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9, 0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,
			0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6, 0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,
			0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e, 0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,
			0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94, 0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
			0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68, 0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16,
		});

		private static readonly Lazy<byte[]> CRC32HighByteTable = new(() => new byte[256]
		{
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40
		});

		private static readonly Lazy<byte[]> SHA256ConstantsTable = new(() => new byte[256]
		{
			0x6A, 0xBB, 0x3C, 0x5A, 0x51, 0xA0, 0x98, 0x7E, 0xCF, 0x90, 0x59, 0xAB, 0x3E, 0xB2, 0x44, 0x56,
			0xC6, 0x3B, 0xE9, 0x3E, 0xE5, 0x56, 0xC1, 0xCE, 0xBF, 0xCA, 0xC6, 0xA4, 0x84, 0x4F, 0xBD, 0x78,
			0xE0, 0x1F, 0x4A, 0xFC, 0x5B, 0xDC, 0xF1, 0x18, 0x27, 0xA1, 0x9C, 0x26, 0xB6, 0xD3, 0x43, 0x5D,
			0x72, 0x50, 0x1E, 0x0B, 0x7F, 0xD6, 0x57, 0x3D, 0x24, 0x73, 0x9F, 0x62, 0x58, 0x41, 0xC8, 0xE0,
			0xCC, 0xF4, 0xD1, 0x58, 0x9E, 0x8D, 0x2C, 0x1B, 0x19, 0x5A, 0x01, 0x73, 0x2B, 0xB5, 0x08, 0xA3,
			0xF4, 0x52, 0x16, 0x2A, 0x5C, 0xB7, 0x55, 0x80, 0xA2, 0xB4, 0xC3, 0xA5, 0xB1, 0x01, 0x20, 0x02,
			0xD2, 0xC4, 0x54, 0xFE, 0x12, 0xB2, 0x6F, 0x8A, 0x2E, 0xA7, 0x79, 0x3E, 0x37, 0xB0, 0xDA, 0xC7,
			0x5A, 0xE9, 0xBD, 0xDB, 0x91, 0xB6, 0x0D, 0x54, 0x65, 0x3C, 0xFF, 0x5B, 0xAD, 0xB2, 0x64, 0xF5,
			0x5A, 0x3C, 0xC6, 0xD0, 0x6F, 0xA9, 0xF5, 0x62, 0xA3, 0x7F, 0xBF, 0x3A, 0x06, 0x12, 0x7F, 0x45,
			0xB8, 0x65, 0xF8, 0x1B, 0x8C, 0x38, 0x71, 0xAA, 0xB3, 0x3D, 0xDF, 0xE0, 0xAB, 0xC2, 0x11, 0x07,
			0xB9, 0x5F, 0x26, 0x91, 0xDC, 0x38, 0xFC, 0x64, 0x90, 0xC9, 0xA2, 0x3C, 0x8E, 0x4E, 0xA3, 0x97,
			0x1F, 0xC2, 0x97, 0xF3, 0x31, 0x5F, 0x6F, 0xF0, 0xB1, 0x85, 0xE8, 0x73, 0x7A, 0xE4, 0xD0, 0x10,
			0x16, 0xA7, 0x89, 0x07, 0x1A, 0xE2, 0xBE, 0x24, 0x4B, 0x98, 0x67, 0x92, 0xB1, 0xF4, 0x10, 0x18,
			0x96, 0xF5, 0xF2, 0xA5, 0x07, 0xF2, 0xC2, 0x10, 0x97, 0x13, 0x9B, 0x2C, 0xB3, 0xB2, 0x49, 0xC5,
			0x6A, 0x3B, 0xC9, 0x51, 0x0B, 0xD3, 0xBA, 0x20, 0xFA, 0xEF, 0x2D, 0x44, 0x4C, 0x99, 0x1A, 0x3D,
			0xAB, 0xC7, 0x76, 0xE4, 0x23, 0xA0, 0xB0, 0x01, 0x69, 0x75, 0x1A, 0x89, 0x91, 0x57, 0x14, 0xC7
		});

		private bool isFirstByte;
		private byte[] hashValue;
		private byte[] permutationTable;
		private bool disposed = false;
		private PearsonTableType tableType;
#if !NET6_0_OR_GREATER

		// Required for .NET Standard 2.0 or older frameworks
		private bool finalized;
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="Pearson" /> class with a default 8-bit hash size.
		/// </summary>
		public Pearson()
		{
			permutationTable = GetPermutationTable(PearsonTableType.Pearson);
			HashSizeValue = MinHashSize;
			hashValue = new byte[HashSizeValue / 8];
			isFirstByte = true;
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
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown PearsonTableType hashValue.")
			};

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

		/// <inheritdoc />
		public override void Initialize()
		{
			ThrowIfDisposed();
#if !NET6_0_OR_GREATER
            State = 0;
            finalized = false;
#endif
			hashValue = new byte[HashSizeValue / 8];
			isFirstByte = true;
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (disposed) return;

			if (disposing)
			{
				if (permutationTable is not null)
				{
					CryptographicOperations.ZeroMemory(permutationTable);
					permutationTable.Clear();
				}

				if (hashValue is not null)
				{
					CryptographicOperations.ZeroMemory(hashValue);
					hashValue.Clear();
				}
			}

			disposed = true;
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
		/// Processes a block of data by feeding it into the <see cref="Pearson" /> algorithm.
		/// </summary>
		/// <param name="source">The input span containing the data to be hashed. This method updates the internal hash state accordingly.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void HashCore(ReadOnlySpan<byte> source)
		{
			ThrowIfDisposed();
			ThrowIfTableNotConfigured();

			ReadOnlySpan<byte> t = permutationTable.AsSpan();
			var v = hashValue;
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

			hashValue = v;
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
			ThrowIfTableNotConfigured();

#if !NET6_0_OR_GREATER
			if (finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);

			finalized = true;
			State = 2;
#endif
			return hashValue.ToArray();
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