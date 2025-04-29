// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfArrayIsNotSingleDimension.cs" company="PlaceholderCompany">
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
		public void ThrowIfArrayIsNotSingleDimension_WhenArrayIsMultiDimensional_ShouldThrowExactly()
		{
			var array = new int[2, 2];
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfArrayIsNotSingleDimension(array));
		}

		[TestMethod]
		public void ThrowIfArrayIsNotSingleDimension_WhenArrayIsSingleDimension_ShouldNotThrow()
		{
			var array = new int[5];
			ThrowHelper.ThrowIfArrayIsNotSingleDimension(array);
		}
	}
}