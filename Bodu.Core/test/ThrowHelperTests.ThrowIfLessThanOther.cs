// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfLessThanOther.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfLessThanOther_WhenValueIsLess_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfLessThanOther(1, 2));
		}

		[TestMethod]
		public void ThrowIfLessThanOther_WhenValueIsEqualOrGreater_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfLessThanOther(2, 2);
			ThrowHelper.ThrowIfLessThanOther(3, 2);
		}
	}
}