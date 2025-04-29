// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfEnumValueIsUndefined.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bodu
{
	public enum TestEnum
	{
		A = 0,
		B = 1,
	}

	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfEnumValueIsUndefined_WhenValueIsUndefined_ShouldThrowExactly()
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ThrowHelper.ThrowIfEnumValueIsUndefined((TestEnum)99));
		}

		[TestMethod]
		public void ThrowIfEnumValueIsUndefined_WhenValueIsDefined_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(TestEnum.A);
		}
	}
}