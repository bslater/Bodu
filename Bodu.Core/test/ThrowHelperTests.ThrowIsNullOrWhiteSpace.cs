// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIsNullOrWhiteSpace.cs" company="PlaceholderCompany">
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
		public void ThrowIsNullOrWhiteSpace_WhenNull_ShouldThrowExactly()
		{
			string? value = null;
			Assert.ThrowsExactly<ArgumentNullException>(() => ThrowHelper.ThrowIsNullOrWhiteSpace(value!));
		}

		[TestMethod]
		public void ThrowIsNullOrWhiteSpace_WhenEmpty_ShouldThrowExactly()
		{
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIsNullOrWhiteSpace(string.Empty));
		}

		[TestMethod]
		public void ThrowIsNullOrWhiteSpace_WhenWhitespace_ShouldThrowExactly()
		{
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIsNullOrWhiteSpace("   "));
		}

		[TestMethod]
		public void ThrowIsNullOrWhiteSpace_WhenValid_ShouldNotThrow()
		{
			ThrowHelper.ThrowIsNullOrWhiteSpace("Valid");
		}
	}
}