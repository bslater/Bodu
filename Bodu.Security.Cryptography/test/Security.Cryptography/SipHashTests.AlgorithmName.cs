﻿// -----------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Bodu.Infrastructure;

using System.Text;

namespace Bodu.Security.Cryptography
{
	public abstract partial class SipHashTests<TTest, TAlgorithm>
	{
		[DataTestMethod]
		[DynamicData(nameof(HashAlgorithmVariants), DynamicDataSourceType.Method)]
		public void AlgorithmName_WhenUsingVariant_ShouldReturnCorrectlyFormattedString(SipHashVariant variant)
		{
			using var algorithm = this.CreateAlgorithm(variant);
			string expected = GetAlgorithmName(algorithm);

			Assert.AreEqual(expected, algorithm.AlgorithmName);
		}

		[TestMethod]
		public void AlgorithmName_WhenUsingCustomRounds_ShouldReturnCorrectlyFormattedString()
		{
			using var algorithm = new TAlgorithm
			{
				CompressionRounds = 3,
				FinalizationRounds = 5
			};
			string expected = GetAlgorithmName(algorithm);

			Assert.AreEqual(expected, algorithm.AlgorithmName);
		}

		private static string GetAlgorithmName(SipHash<TAlgorithm> algorithm) =>
			$"SipHash-{algorithm.CompressionRounds}-{algorithm.FinalizationRounds}-{algorithm.HashSize}";
	}
}