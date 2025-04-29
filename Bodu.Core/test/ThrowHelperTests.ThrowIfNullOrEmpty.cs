// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfNullOrEmpty.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfNullOrEmpty_WhenValueIsNull_ShouldThrowExactly()
		{
			string? value = null;
			Assert.ThrowsExactly<ArgumentNullException>(() => ThrowHelper.ThrowIfNullOrEmpty(value!));
		}

		[TestMethod]
		public void ThrowIfNullOrEmpty_WhenValueIsEmpty_ShouldThrowExactly()
		{
			string value = string.Empty;
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfNullOrEmpty(value));
		}

		[TestMethod]
		public void ThrowIfNullOrEmpty_WhenValueIsValid_ShouldNotThrow()
		{
			string value = "test";
			ThrowHelper.ThrowIfNullOrEmpty(value);
		}
	}
}