// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfNullOrEmpty.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[DataTestMethod]
		[DataRow(null)]
		public void ThrowIfNullOrEmpty_WhenValueIsNull_ShouldThrowArgumentNullException(string? value)
		{
			Assert.ThrowsExactly<ArgumentNullException>(() =>
			{
				ThrowHelper.ThrowIfNullOrEmpty(value!);
			});
		}

		[DataTestMethod]
		[DataRow("")]
		public void ThrowIfNullOrEmpty_WhenValueIsEmpty_ShouldThrowArgumentException(string value)
		{
			Assert.ThrowsExactly<ArgumentException>(() =>
			{
				ThrowHelper.ThrowIfNullOrEmpty(value);
			});
		}

		[DataTestMethod]
		[DataRow("a")]
		[DataRow("test")]
		[DataRow("   ")] // Optional: consider whether whitespace-only is allowed
		public void ThrowIfNullOrEmpty_WhenValueIsNonEmpty_ShouldNotThrow(string value)
		{
			ThrowHelper.ThrowIfNullOrEmpty(value);
		}
	}
}