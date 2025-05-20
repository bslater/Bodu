﻿// -----------------------------------------------------------------------
// <copyright file="BKDR.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.Intrinsics.X86;

namespace Bodu.Security.Cryptography
{
	public enum BKDRVariant
	{
		Default,
		Seed31,
		Seed1313,
	}

	/// <summary>
	/// Contains unit tests for the <see cref="BKDR" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class BKDRTests
		: Security.Cryptography.HashAlgorithmTests<BKDRTests, BKDR, BKDRVariant>
	{
		public override IEnumerable<BKDRVariant> GetHashAlgorithmVariants() => new[]
		{
			BKDRVariant.Default,
			BKDRVariant.Seed31,
			BKDRVariant.Seed1313,
		};

		protected override BKDR CreateAlgorithm() => new BKDR();

		protected override BKDR CreateAlgorithm(BKDRVariant variant) =>
			variant switch
			{
				BKDRVariant.Default => this.CreateAlgorithm(),
				BKDRVariant.Seed31 => new BKDR
				{
					Seed = 31
				},
				BKDRVariant.Seed1313 => new BKDR
				{
					Seed = 1313
				},
				_ => throw new ArgumentOutOfRangeException(nameof(variant))
			};

		protected override IReadOnlyDictionary<string, string> GetExpectedHashesForNamedInputs(BKDRVariant variant) =>
			variant switch
			{
				BKDRVariant.Default => new Dictionary<string, string>
				{
					["Empty"] = "00000083",
					["ABC"] = "119EDDA3",
					["Zeros_16"] = "760E2E43",
					["QuickBrownFox"] = "FDAD9AD8",
					["Sequential_0_255"] = "752AC0AC",
				},
				BKDRVariant.Seed31 => new Dictionary<string, string>
				{
					["Empty"] = "0000001F",
					["ABC"] = "000F13C3",
					["Zeros_16"] = "C491E21F",
					["QuickBrownFox"] = "764D9FD4",
					["Sequential_0_255"] = "B7D06C60",
				},
				BKDRVariant.Seed1313 => new Dictionary<string, string>
				{
					["Empty"] = "00000521",
					["ABC"] = "03CEDDC7",
					["Zeros_16"] = "62687721",
					["QuickBrownFox"] = "373C737A",
					["Sequential_0_255"] = "DA826D62",
				},
				_ => throw new ArgumentOutOfRangeException(nameof(variant))
			};

		protected override IReadOnlyList<string> GetExpectedHashesForIncrementalInput(BKDRVariant variant) =>
			variant switch
			{
				BKDRVariant.Default => new[]
				{
					"00000083",
					"00004309",
					"00224D9C",
					"118DB6D6",
					"FB848F85",
					"B4D57113",
					"8938DCBE",
					"3818F540",
					"B4C57FC7",
					"811062DD",
					"0B629720",
					"D373556A",
					"3404B549",
					"9E68C467",
					"0F9C80C2",
					"FD15E354",
				},
				BKDRVariant.Seed31 => new[]
				{
					"0000001F",
					"000003C1",
					"00007460",
					"000E17A2",
					"01B4DCA1",
					"34E6B783",
					"67F038E2",
					"9616E364",
					"2CC58923",
					"6BEB9B45",
					"1187CD64",
					"1F71DF26",
					"CECA05A5",
					"0A76AF07",
					"445F31E6",
					"47870AE8",
				},
				BKDRVariant.Seed1313 => new[]
				{
					"00000521",
					"001A4E41",
					"86EB5B62",
					"FD1FB1A4",
					"3F8E1A27",
					"F7D4220B",
					"17029A70",
					"045A1876",
					"5217753D",
					"0A504DE5",
					"E5DF838E",
					"FF61BB58",
					"D441DE63",
					"A5D599CF",
					"8C89DEBC",
					"CF1F624A",
				},
				_ => throw new ArgumentOutOfRangeException(nameof(variant))
			};
	}
}