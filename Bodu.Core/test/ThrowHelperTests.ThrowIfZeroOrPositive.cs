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
		[DataTestMethod]
		[DataRow(0)]
		[DataRow(1)]
		[DataRow(100)]
		[DataRow(int.MaxValue)]
		public void ThrowIfZeroOrPositive_WhenValueIsZeroOrPositive_ShouldThrow(int value)
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				ThrowHelper.ThrowIfZeroOrPositive(value);
			});
		}

		[DataTestMethod]
		[DataRow(-1)]
		[DataRow(-100)]
		[DataRow(int.MinValue)]
		public void ThrowIfZeroOrPositive_WhenValueIsNegative_ShouldNotThrow(int value)
		{
			ThrowHelper.ThrowIfZeroOrPositive(value);
		}
	}
}