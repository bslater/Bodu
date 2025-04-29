// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfCountExceedsAvailable.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfCountExceedsAvailable_WhenCountIsTooLarge_ShouldThrowExactly()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfCountExceedsAvailable(6, 5));
		}

		[TestMethod]
		public void ThrowIfCountExceedsAvailable_WhenCountIsNegative_ShouldThrowExactly()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfCountExceedsAvailable(-1, 5));
		}

		[TestMethod]
		public void ThrowIfCountExceedsAvailable_WhenCountIsValid_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfCountExceedsAvailable(3, 5);
		}
	}
}