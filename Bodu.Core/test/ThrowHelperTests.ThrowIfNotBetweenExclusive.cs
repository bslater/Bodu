// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfNotBetweenExclusive.cs" company="PlaceholderCompany">
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
		[DataRow(5, 6, 10)]   // Below lower bound
		[DataRow(10, 6, 10)]  // At upper bound
		[DataRow(6, 6, 10)]   // At lower bound
		[DataRow(4, 4, 4)]    // All equal
		[DataRow(3, 4, 4)]    // Below equal bounds
		[DataRow(5, 5, 5)]    // Equal value and bounds
		public void ThrowIfNotBetweenExclusive_WhenValueIsNotStrictlyBetween_ShouldThrow(int value, int min, int max)
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				ThrowHelper.ThrowIfNotBetweenExclusive(value, min, max);
			});
		}

		[DataTestMethod]
		[DataRow(7, 6, 10)]   // Strictly between
		[DataRow(6.5, 6.0, 10.0)] // Floating point between
		public void ThrowIfNotBetweenExclusive_WhenValueIsStrictlyBetween_ShouldNotThrow(double value, double min, double max)
		{
			ThrowHelper.ThrowIfNotBetweenExclusive(value, min, max);
		}
	}
}