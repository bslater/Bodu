// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfArrayIsNotZeroBased.cs" company="PlaceholderCompany">
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
		public void ThrowIfArrayIsNotZeroBased_WhenArrayIsNotZeroBased_ShouldThrowExactly()
		{
			Array array = Array.CreateInstance(typeof(int), new int[] { 5 }, new int[] { 1 });
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfArrayIsNotZeroBased(array));
		}

		[TestMethod]
		public void ThrowIfArrayIsNotZeroBased_WhenArrayIsZeroBased_ShouldNotThrow()
		{
			var array = new int[5];
			ThrowHelper.ThrowIfArrayIsNotZeroBased(array);
		}
	}
}