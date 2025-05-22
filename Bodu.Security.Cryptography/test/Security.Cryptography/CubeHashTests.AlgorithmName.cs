// -----------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Bodu.Infrastructure;

using System.Text;

namespace Bodu.Security.Cryptography
{
	public partial class CubeHashTests
	{
		[DataTestMethod]
		[DynamicData(nameof(HashAlgorithmVariants), DynamicDataSourceType.Method)]
		public void AlgorithmName_WhenUsingCustomRounds_ShouldReturnCorrectlyFormattedString(CubeHashVariants variant)
		{
			using var algorithm = this.CreateAlgorithm(variant);

			Assert.AreEqual($"CubeHash{algorithm.InitializationRounds}+{algorithm.Rounds}/{algorithm.TransformBlockSize}+{algorithm.FinalizationRounds}-{algorithm.HashSize}", algorithm.AlgorithmName);
		}

		[TestMethod]
		public void AlgorithmName_WhenUsingCustomRounds_ShouldReturnCorrectlyFormattedString()
		{
			using var algorithm = new CubeHash
			{
				InitializationRounds = 3,
				Rounds = 5
			};

			Assert.AreEqual($"CubeHash{algorithm.InitializationRounds}+{algorithm.Rounds}/{algorithm.TransformBlockSize}+{algorithm.FinalizationRounds}-{algorithm.HashSize}", algorithm.AlgorithmName);
		}
	}
}