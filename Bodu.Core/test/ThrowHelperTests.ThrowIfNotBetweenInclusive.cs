// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfNotBetweenInclusive.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfNotBetweenInclusive_WhenValueIsOutOfBounds_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfNotBetweenInclusive(5, 6, 10));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfNotBetweenInclusive(11, 6, 10));
		}

		[TestMethod]
		public void ThrowIfNotBetweenInclusive_WhenValueIsWithinBounds_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfNotBetweenInclusive(6, 6, 10);
			ThrowHelper.ThrowIfNotBetweenInclusive(8, 6, 10);
			ThrowHelper.ThrowIfNotBetweenInclusive(10, 6, 10);
		}
	}
}