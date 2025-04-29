// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfZeroOrNegative.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfZeroOrNegative_WhenValueIsZeroOrNegative_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfZeroOrNegative(0));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfZeroOrNegative(-1));
		}

		[TestMethod]
		public void ThrowIfZeroOrNegative_WhenValueIsPositive_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfZeroOrNegative(1);
		}
	}
}