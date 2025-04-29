// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfArrayLengthIsInsufficient.cs" company="PlaceholderCompany">
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
		public void ThrowIfArrayLengthIsInsufficient_WhenArrayTooShort_ShouldThrowExactly()
		{
			var array = new int[5];
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfArrayLengthIsInsufficient(array, 2, 5));
		}

		[TestMethod]
		public void ThrowIfArrayLengthIsInsufficient_WhenArraySufficient_ShouldNotThrow()
		{
			var array = new int[10];
			ThrowHelper.ThrowIfArrayLengthIsInsufficient(array, 2, 5);
		}
	}
}