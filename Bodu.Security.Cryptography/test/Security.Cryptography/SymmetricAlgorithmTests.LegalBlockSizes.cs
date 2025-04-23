﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Security.Cryptography
{
	public abstract partial class SymmetricAlgorithmTests<T>
	{
		/// <summary>
		/// Verifies that LegalBlockSizes returns a new instance each call.
		/// </summary>
		[TestMethod]
		public void LegalBlockSizes_WhenCalledMultipleTimes_ShouldReturnNewArrayInstances()
		{
			using T algorithm = CreateAlgorithm();
			Assert.AreNotSame(algorithm.LegalBlockSizes, algorithm.LegalBlockSizes);
		}

		/// <summary>
		/// Verifies that LegalBlockSizes define valid MinSize, MaxSize, and SkipSize values.
		/// </summary>
		[TestMethod]
		public void LegalBlockSizes_WhenDefined_ShouldHaveValidRanges()
		{
			var blockSizes = this.CreateAlgorithm().LegalBlockSizes;

			foreach (var blockSize in blockSizes)
			{
				Assert.IsTrue(blockSize.MinSize <= blockSize.MaxSize, "MinSize must be less than or equal to MaxSize.");
				Assert.IsTrue(blockSize.SkipSize >= 0, "SkipSize must be greater than or equal to zero.");
			}
		}

		/// <summary>
		/// Verifies that LegalBlockSizes do not overlap and are unique.
		/// </summary>
		[TestMethod]
		public void LegalBlockSizes_WhenDefined_ShouldHaveNonOverlappingValues()
		{
			var blockSizes = this.CreateAlgorithm().LegalBlockSizes;
			HashSet<int> uniqueSizes = new();

			foreach (var blockSize in blockSizes)
			{
				for (int size = blockSize.MinSize; size <= blockSize.MaxSize; size += blockSize.SkipSize == 0 ? int.MaxValue : blockSize.SkipSize)
				{
					Assert.IsTrue(uniqueSizes.Add(size), $"Duplicate or overlapping block size detected: {size}.");
				}
			}
		}
	}
}