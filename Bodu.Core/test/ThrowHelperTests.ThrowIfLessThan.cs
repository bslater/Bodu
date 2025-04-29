// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfLessThan.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfLessThan_WhenValueIsLess_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfLessThan(2, 5));
		}

		[TestMethod]
		public void ThrowIfLessThan_WhenValueIsEqualOrGreater_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfLessThan(5, 5);
			ThrowHelper.ThrowIfLessThan(6, 5);
		}

		[TestMethod]
		public void ThrowIfLessThan_WhenNullableIsNullAndThrowIfNull_ShouldThrowExactly()
		{
			int? value = null;
			Assert.ThrowsExactly<ArgumentNullException>(() => ThrowHelper.ThrowIfLessThan(value, 5, true));
		}

		[TestMethod]
		public void ThrowIfLessThan_WhenNullableIsNullAndThrowIfNullIsFalse_ShouldNotThrow()
		{
			int? value = null;
			ThrowHelper.ThrowIfLessThan(value, 5, false);
		}

		[TestMethod]
		public void ThrowIfLessThan_WhenNullableIsLess_ShouldThrow()
		{
			int? value = 2;
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfLessThan(value, 5, false));
		}

		[TestMethod]
		public void ThrowIfLessThan_WhenNullableIsGreaterOrEqual_ShouldNotThrow()
		{
			int? value = 5;
			ThrowHelper.ThrowIfLessThan(value, 5, false);
		}
	}
}