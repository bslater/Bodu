// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfArrayLengthNotPositiveMultipleOf.cs" company="PlaceholderCompany">
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
		public void ThrowIfArrayLengthNotPositiveMultipleOf_WhenNotMultiple_ShouldThrowExactly()
		{
			var array = new int[5];
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfArrayLengthNotPositiveMultipleOf(array, 2));
		}

		[TestMethod]
		public void ThrowIfArrayLengthNotPositiveMultipleOf_WhenMultiple_ShouldNotThrow()
		{
			var array = new int[6];
			ThrowHelper.ThrowIfArrayLengthNotPositiveMultipleOf(array, 3);
		}
	}
}