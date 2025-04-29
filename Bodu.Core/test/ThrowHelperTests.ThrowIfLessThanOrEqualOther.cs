// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfLessThanOrEqualOther.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfLessThanOrEqualOther_WhenValueIsLessOrEqual_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfLessThanOrEqualOther(2, 2));
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfLessThanOrEqualOther(1, 2));
		}

		[TestMethod]
		public void ThrowIfLessThanOrEqualOther_WhenValueIsGreater_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfLessThanOrEqualOther(3, 2);
		}
	}
}