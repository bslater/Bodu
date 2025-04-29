// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfGreaterThanOrEqual.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfGreaterThanOrEqual_WhenValueIsGreaterOrEqual_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfGreaterThanOrEqual(5, 5));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfGreaterThanOrEqual(6, 5));
		}

		[TestMethod]
		public void ThrowIfGreaterThanOrEqual_WhenValueIsLess_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfGreaterThanOrEqual(4, 5);
		}
	}
}