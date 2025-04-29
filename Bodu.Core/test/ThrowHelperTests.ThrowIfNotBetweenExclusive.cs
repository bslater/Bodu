// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfNotBetweenExclusive.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfNotBetweenExclusive_WhenValueIsNotBetween_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfNotBetweenExclusive(5, 6, 10));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfNotBetweenExclusive(10, 6, 10));
		}

		[TestMethod]
		public void ThrowIfNotBetweenExclusive_WhenValueIsStrictlyBetween_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfNotBetweenExclusive(7, 6, 10);
		}
	}
}