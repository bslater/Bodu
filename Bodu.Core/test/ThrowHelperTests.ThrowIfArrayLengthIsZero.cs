// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfArrayLengthIsZero.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bodu.Core.Test
{
	public sealed partial class ThrowHelperTests
	{
		[DataTestMethod]
		[DataRow(0)]
		public void ThrowIfArrayLengthIsZero_WhenArrayLengthIsZero_ShouldThrowExactly(int length)
		{
			var array = new int[length];
			Assert.ThrowsExactly<ArgumentException>(() =>
			{
				ThrowHelper.ThrowIfArrayLengthIsZero(array);
			});
		}

		[DataTestMethod]
		[DataRow(1)]
		[DataRow(5)]
		[DataRow(100)]
		public void ThrowIfArrayLengthIsZero_WhenArrayLengthIsNonZero_ShouldNotThrow(int length)
		{
			var array = new int[length];
			ThrowHelper.ThrowIfArrayLengthIsZero(array);
		}
	}
}