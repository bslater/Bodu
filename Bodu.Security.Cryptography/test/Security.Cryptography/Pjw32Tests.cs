// ---------------------------------------------------------------------------------------------------------------
// <copyright file="JSHashTests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="Pjw32" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class Pjw32Tests
		: Security.Cryptography.HashAlgorithmTests<Pjw32Tests, Pjw32, SingleTestVariant>
	{
		public override IEnumerable<SingleTestVariant> GetHashAlgorithmVariants() => new[]
		{
			SingleTestVariant.Default
		};

		/// <inheritdoc />
		protected override Pjw32 CreateAlgorithm() => new Pjw32();

		protected override Pjw32 CreateAlgorithm(SingleTestVariant variant) => new Pjw32();

		protected override IReadOnlyList<string> GetExpectedHashesForIncrementalInput(SingleTestVariant variant) => new[]
		{
			"00000000",
			"00000000",
			"00000001",
			"00000012",
			"00000123",
			"00001234",
			"00012345",
			"00123456",
			"01234567",
			"02345679",
			"0345679B",
			"045679B9",
			"05679B9F",
			"0679B9F9",
			"079B9F9B",
			"09B9F9B9",
		};

		protected override IReadOnlyDictionary<string, string> GetExpectedHashesForNamedInputs(SingleTestVariant variant) => new Dictionary<string, string>
		{
			["Empty"] = "00000000",
			["ABC"] = "00004563",
			["Zeros_16"] = "00000000",
			["QuickBrownFox"] = "021B6694",
		};
	}
}