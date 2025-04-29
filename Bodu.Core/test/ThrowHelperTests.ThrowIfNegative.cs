// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfNegative.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfNegative_WhenValueIsNegative_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfNegative(-1));
		}

		[TestMethod]
		public void ThrowIfNegative_WhenValueIsZeroOrPositive_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfNegative(0);
			ThrowHelper.ThrowIfNegative(5);
		}
	}
}