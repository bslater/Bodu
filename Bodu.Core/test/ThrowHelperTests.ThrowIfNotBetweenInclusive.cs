// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfNotBetweenInclusive.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[DataTestMethod]
		[DataRow(5, 6, 10)]    // Below lower bound
		[DataRow(11, 6, 10)]   // Above upper bound
		[DataRow(-1, 0, 1)]    // Below minimum
		public void ThrowIfNotBetweenInclusive_WhenValueIsOutsideInclusiveRange_ShouldThrow(int value, int min, int max)
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				ThrowHelper.ThrowIfNotBetweenInclusive(value, min, max);
			});
		}

		[DataTestMethod]
		[DataRow(6, 6, 10)]    // Equal to lower bound
		[DataRow(8, 6, 10)]    // Within bounds
		[DataRow(10, 6, 10)]   // Equal to upper bound
		[DataRow(1, 0, 1)]     // Equal to upper
		[DataRow(0, 0, 1)]     // Equal to lower
		public void ThrowIfNotBetweenInclusive_WhenValueIsWithinInclusiveRange_ShouldNotThrow(int value, int min, int max)
		{
			ThrowHelper.ThrowIfNotBetweenInclusive(value, min, max);
		}
	}
}