// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfGreaterThan.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfGreaterThan_WhenValueIsGreater_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfGreaterThan(5, 3));
		}

		[TestMethod]
		public void ThrowIfGreaterThan_WhenValueIsLessOrEqual_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfGreaterThan(3, 3);
			ThrowHelper.ThrowIfGreaterThan(2, 3);
		}
	}
}