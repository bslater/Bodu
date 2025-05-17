// -----------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Bodu.Infrastructure;

using System.Text;

namespace Bodu.Security.Cryptography
{
	public abstract partial class SipHashTests<T>
	{
		[DataTestMethod]
		[DataRow(-1, -1)]
		[DataRow(3, 5)]
		public void AlgorithmName_WhenUsingCustomRounds_ShouldReturnCorrectlyFormattedString(int c, int d)
		{
			using var algorithm = new T();
			if (c > 0 & d > 0)
			{
				algorithm.CompressionRounds = 3;
				algorithm.FinalizationRounds = 5;
			}

			Assert.AreEqual($"SipHash-{algorithm.CompressionRounds}-{algorithm.FinalizationRounds}-{algorithm.HashSize}", algorithm.AlgorithmName);
		}
	}
}