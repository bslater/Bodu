// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfPositive.cs" company="PlaceholderCompany">
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
		public void ThrowIfPositive_WhenValueIsPositive_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfPositive(1));
		}

		[TestMethod]
		public void ThrowIfPositive_WhenValueIsZeroOrNegative_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfPositive(0);
			ThrowHelper.ThrowIfPositive(-5);
		}
	}
}