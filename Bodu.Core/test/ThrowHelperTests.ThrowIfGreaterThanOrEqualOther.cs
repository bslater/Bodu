// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfGreaterThanOrEqualOther.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfGreaterThanOrEqualOther_WhenValueIsGreaterOrEqual_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfGreaterThanOrEqualOther(5, 5));
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfGreaterThanOrEqualOther(6, 5));
		}

		[TestMethod]
		public void ThrowIfGreaterThanOrEqualOther_WhenValueIsLess_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfGreaterThanOrEqualOther(4, 5);
		}
	}
}