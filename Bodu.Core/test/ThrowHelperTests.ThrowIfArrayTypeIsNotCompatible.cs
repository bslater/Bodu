// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfArrayTypeIsNotCompatible.cs" company="PlaceholderCompany">
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
		public void ThrowIfArrayTypeIsNotCompatible_WhenArrayIsWrongType_ShouldThrowExactly()
		{
			Array array = new string[5];
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfArrayTypeIsNotCompatible<int>(array));
		}

		[TestMethod]
		public void ThrowIfArrayTypeIsNotCompatible_WhenArrayIsCorrectType_ShouldNotThrow()
		{
			Array array = new int[5];
			ThrowHelper.ThrowIfArrayTypeIsNotCompatible<int>(array);
		}
	}
}