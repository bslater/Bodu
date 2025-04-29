// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfCollectionTooSmall.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfCollectionTooSmall_WhenCollectionIsTooSmall_ShouldThrowExactly()
		{
			var collection = new List<int> { 1 };
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfCollectionTooSmall(collection, 2));
		}

		[TestMethod]
		public void ThrowIfCollectionTooSmall_WhenCollectionIsSufficient_ShouldNotThrow()
		{
			var collection = new List<int> { 1, 2 };
			ThrowHelper.ThrowIfCollectionTooSmall(collection, 2);
		}
	}
}