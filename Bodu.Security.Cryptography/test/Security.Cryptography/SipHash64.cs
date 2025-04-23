// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Adler32Tests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="SipHash64" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class SipHash64Tests
		: Security.Cryptography.SipHashTests<SipHash64>
	{
		private const int ExpectedKeySize = 16;

		/// <inheritdoc />
		protected override int ExpectedInputBlockSize => 8;

		/// <inheritdoc />
		protected override int ExpectedOutputBlockSize => 8;

		/// <inheritdoc />
		protected override int MaximumLegalKeyLength => ExpectedKeySize;

		/// <inheritdoc />
		protected override int MinimumLegalKeyLength => ExpectedKeySize;

		/// <inheritdoc />
		protected override IReadOnlyList<int> ValidKeyLengths => new[] { ExpectedKeySize };

		/// <inheritdoc />
		protected override SipHash64 CreateAlgorithm() => new SipHash64
		{
			Key = Enumerable.Range(0, 16).Select(x => (byte)x).ToArray(),
			CompressionRounds = 2,
			FinalizationRounds = 4,
		};

		/// <inheritdoc />
		protected override string ExpectedHash_ForEmptyByteArray => "310E0EDD47DB6F72";

		/// <inheritdoc />
		protected override byte[] GenerateUniqueKey()
		{
			byte[] key = new byte[SipHash.KeySize];
			CryptoUtilities.FillWithRandomNonZeroBytes(key);
			return key;
		}
	}
}