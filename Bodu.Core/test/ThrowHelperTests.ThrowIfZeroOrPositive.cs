// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfZeroOrPositive.cs" company="PlaceholderCompany">
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
		public void ThrowIfZeroOrPositive_WhenValueIsZero_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfZeroOrPositive(0));
		}

		[TestMethod]
		public void ThrowIfZeroOrPositive_WhenValueIsPositive_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfZeroOrPositive(5));
		}

		[TestMethod]
		public void ThrowIfZeroOrPositive_WhenValueIsNegative_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfZeroOrPositive(-1);
		}
	}
}