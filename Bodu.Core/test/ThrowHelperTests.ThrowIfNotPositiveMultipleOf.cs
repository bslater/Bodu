// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfNotPositiveMultipleOf.cs" company="PlaceholderCompany">
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
		public void ThrowIfNotPositiveMultipleOf_WhenValueIsNotPositiveOrMultiple_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfNotPositiveMultipleOf(0, 2));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfNotPositiveMultipleOf(7, 2));
		}

		[TestMethod]
		public void ThrowIfNotPositiveMultipleOf_WhenValueIsPositiveMultiple_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfNotPositiveMultipleOf(4, 2);
		}
	}
}