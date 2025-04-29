// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfLessThanOrEqual.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfLessThanOrEqual_WhenValueIsLessOrEqual_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfLessThanOrEqual(2, 2));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfLessThanOrEqual(1, 2));
		}

		[TestMethod]
		public void ThrowIfLessThanOrEqual_WhenValueIsGreater_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfLessThanOrEqual(3, 2);
		}
	}
}