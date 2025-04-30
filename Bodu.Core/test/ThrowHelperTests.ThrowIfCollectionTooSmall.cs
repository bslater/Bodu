// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfCollectionTooSmall.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[DataTestMethod]
		[DynamicData(nameof(GetTooSmallCollectionTestData), DynamicDataSourceType.Method)]
		public void ThrowIfCollectionTooSmall_WhenCollectionTooSmall_ShouldThrowArgumentException(ICollection<int> collection, int minimumCount)
		{
			Assert.ThrowsExactly<ArgumentException>(() =>
			{
				ThrowHelper.ThrowIfCollectionTooSmall<int>(collection, minimumCount);
			});
		}

		[DataTestMethod]
		[DynamicData(nameof(GetSufficientCollectionTestData), DynamicDataSourceType.Method)]
		public void ThrowIfCollectionTooSmall_WhenCollectionIsSufficient_ShouldNotThrow(ICollection<int> collection, int minimumCount)
		{
			ThrowHelper.ThrowIfCollectionTooSmall<int>(collection, minimumCount);
		}

		private static IEnumerable<object[]> GetTooSmallCollectionTestData()
		{
			yield return new object[] { new List<int>(), 1 };
			yield return new object[] { new List<int> { 1 }, 2 };
			yield return new object[] { new int[] { 1, 2 }, 3 };
		}

		private static IEnumerable<object[]> GetSufficientCollectionTestData()
		{
			yield return new object[] { new List<int> { 1 }, 1 };
			yield return new object[] { new List<int> { 1, 2 }, 2 };
			yield return new object[] { new int[] { 1, 2, 3 }, 2 };
			yield return new object[] { new int[] { }, 0 };
		}
	}
}