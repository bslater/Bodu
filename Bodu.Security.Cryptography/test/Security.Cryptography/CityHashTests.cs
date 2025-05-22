// ---------------------------------------------------------------------------------------------------------------
// <copyright file="CityHashTests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="CityHash" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class CityHashTests<TTest, TAlgorithm>
		: Security.Cryptography.HashAlgorithmTests<CityHashTests<TTest, TAlgorithm>, CityHashBase<TAlgorithm>, SingleHashVariant>
		where TTest : CityHashTests<TTest, TAlgorithm>
		where TAlgorithm : CityHashBase<TAlgorithm>, new()
	{
		public override IEnumerable<SingleHashVariant> GetHashAlgorithmVariants() => new[]
		{
			SingleHashVariant.Default
		};

		/// <inheritdoc />
		protected override CityHashBase CreateAlgorithm() => new CityHash();

		protected override CityHashBase CreateAlgorithm(SingleHashVariant variant) => new CityHash();

		protected override IReadOnlyList<string> GetExpectedHashesForIncrementalInput(SingleHashVariant variant) => new[]
		{
			"AAAAAAAA",
			"2E00F5AE",
			"E245A8A4",
			"58889A1A",
			"41256D43",
			"35D4123D",
			"87EF8D8C",
			"40836C38",
			"970BC723",
			"5A3E14A2",
			"85E418C9",
			"4E2D7A9C",
			"50181E2A",
			"70885464",
			"59B8F2C7",
			"1D01A1F7",
		};

		protected override IReadOnlyDictionary<string, string> GetExpectedHashesForNamedInputs(SingleHashVariant variant) => new Dictionary<string, string>
		{
			["Empty"] = "AAAAAAAA",
			["ABC"] = "37889B1A",
			["Zeros_16"] = "B1DD413D",
			["QuickBrownFox"] = "38FEEFDF",
		};
	}
}