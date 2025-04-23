using System.Security.Cryptography;

namespace Bodu.Infrastructure
{
	/// <summary>
	/// Represents a specific configuration or variation of a <see cref="HashAlgorithm" /> for use in parameterized testing.
	/// </summary>
	/// <remarks>
	/// This class encapsulates factory logic to create the algorithm instance, the expected hash outputs for standard inputs, and optional
	/// configuration values like input/output block size expectations. Used to validate correctness across different algorithm modes (e.g.
	/// modified vs original).
	/// </remarks>
	public sealed class HashAlgorithmVariant
	{
		/// <summary>
		/// Gets or sets the display name of the variant, typically indicating the algorithm mode or configuration.
		/// </summary>
		public string Name { get; init; } = string.Empty;

		/// <summary>
		/// Gets or sets the factory function that creates a new instance of the hash algorithm under test.
		/// </summary>
		public Func<HashAlgorithm> Factory { get; init; } = default!;

		/// <summary>
		/// Gets or sets the expected hash hashValue (in hexadecimal) when computing the hash of an empty byte array.
		/// </summary>
		public string ExpectedHash_ForEmptyByteArray { get; init; } = string.Empty;

		/// <summary>
		/// Gets or sets the expected hash hashValue (in hexadecimal) when computing the hash of a known ASCII string input.
		/// </summary>
		public string ExpectedHash_ForSimpleTextAsciiBytes { get; init; } = string.Empty;

		/// <summary>
		/// Gets or sets the expected hash hashValue (in hexadecimal) when computing the hash of the byte sequence 0 to 255.
		/// </summary>
		public string ExpectedHash_ForByteSequence0To255 { get; init; } = string.Empty;

		/// <summary>
		/// Gets or sets the list of expected hash values (in hexadecimal) when computing the hash of progressively longer prefixes of the
		/// byte sequence 0, 0..1, 0..2, ..., useful for incremental validation.
		/// </summary>
		public IReadOnlyList<string> ExpectedHash_ForInputPrefixes { get; init; } = Array.Empty<string>();

		public IReadOnlyList<HashAlgorithmTestCase> ExpectedHash_ForHashTestVectors { get; init; } = Array.Empty<HashAlgorithmTestCase>();

		/// <inheritdoc />
		public override string ToString() => Name;
	}

	public record class HashAlgorithmTestCase
	{
		public Func<HashAlgorithm> Factory { get; init; } = default!;

		public string InputHex { get; init; } = string.Empty;

		public string ExpectedHex { get; init; } = string.Empty;
	}
}