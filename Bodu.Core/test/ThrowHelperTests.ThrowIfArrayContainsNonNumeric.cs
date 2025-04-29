// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfArrayContainsNonNumeric.cs" company="PlaceholderCompany">
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
		public void ThrowIfArrayContainsNonNumeric_WhenArrayContainsNonNumeric_ShouldThrowExactly()
		{
			var array = new object[] { 1, 2.0, "string" };
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfArrayContainsNonNumeric(array));
		}

		[TestMethod]
		public void ThrowIfArrayContainsNonNumeric_WhenArrayIsNumeric_ShouldNotThrow()
		{
			var array = new object[] { 1, 2.0, 3m };
			ThrowHelper.ThrowIfArrayContainsNonNumeric(array);
		}
	}
}