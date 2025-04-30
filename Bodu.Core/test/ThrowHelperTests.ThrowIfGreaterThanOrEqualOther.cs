// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfGreaterThanOrEqualOther.cs" company="PlaceholderCompany">
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
		[DataRow(5, 5)]
		[DataRow(6, 5)]
		[DataRow(1, 0)]
		[DataRow(0, 0)]
		[DataRow(int.MaxValue, int.MaxValue)]
		public void ThrowIfGreaterThanOrEqualOther_WhenValueIsGreaterThanOrEqualToOther_ShouldThrowArgumentException(int value, int other)
		{
			Assert.ThrowsExactly<ArgumentException>(() =>
			{
				ThrowHelper.ThrowIfGreaterThanOrEqualOther(value, other);
			});
		}

		[DataTestMethod]
		[DataRow(-1, 0)]
		[DataRow(4, 5)]
		[DataRow(int.MinValue, int.MaxValue)]
		public void ThrowIfGreaterThanOrEqualOther_WhenValueIsLessThanOther_ShouldNotThrow(int value, int other)
		{
			ThrowHelper.ThrowIfGreaterThanOrEqualOther(value, other);
		}
	}
}