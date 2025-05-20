// -----------------------------------------------------------------------
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
		public void AlgorithmName_WhenUsingCustomRounds_ShouldReturnCorrectlyFormattedString(SipHashVariant variant)
		{
			using var algorithm = this.CreateAlgorithm(variant);

			Assert.AreEqual($"SipHash-{algorithm.CompressionRounds}-{algorithm.FinalizationRounds}-{algorithm.HashSize}", algorithm.AlgorithmName);
		}

		[TestMethod]
		public void AlgorithmName_WhenUsingCustomRounds_ShouldReturnCorrectlyFormattedString()
		{
			using var algorithm = new TAlgorithm
			{
				CompressionRounds = 3,
				FinalizationRounds = 5
			};

			Assert.AreEqual($"SipHash-{algorithm.CompressionRounds}-{algorithm.FinalizationRounds}-{algorithm.HashSize}", algorithm.AlgorithmName);
		}
	}
}