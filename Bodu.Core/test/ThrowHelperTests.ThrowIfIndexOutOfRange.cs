// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfIndexOutOfRange.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfIndexOutOfRange_WhenIndexIsGreaterThanOrEqualSize_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfIndexOutOfRange<int>(5, 5));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfIndexOutOfRange<int>(6, 5));
		}

		[TestMethod]
		public void ThrowIfIndexOutOfRange_WhenIndexIsWithinRange_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfIndexOutOfRange<int>(0, 5);
			ThrowHelper.ThrowIfIndexOutOfRange<int>(4, 5);
		}
	}
}