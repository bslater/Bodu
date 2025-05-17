// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Adler32Tests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="SipHash128" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class SipHash128Tests
		: Security.Cryptography.SipHashTests<SipHash128>
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
		protected override SipHash128 CreateAlgorithm() => new SipHash128
		{
			Key = Enumerable.Range(0, 16).Select(x => (byte)x).ToArray(),
			CompressionRounds = 2,
			FinalizationRounds = 4,
		};

		/// <inheritdoc />
		protected override string ExpectedHash_ForEmptyByteArray => "A3817F04BA25A8E66DF67214C7550293";

		/// <inheritdoc />
		protected override byte[] GenerateUniqueKey()
		{
			byte[] key = new byte[SipHash.KeySize];
			CryptoUtilities.FillWithRandomNonZeroBytes(key);
			return key;
		}
	}
}