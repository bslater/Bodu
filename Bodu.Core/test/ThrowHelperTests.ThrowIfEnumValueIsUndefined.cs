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
		[DataTestMethod]
		[DataRow((TestEnum)99)]
		[DataRow((TestEnum)(-1))]
		public void ThrowIfEnumValueIsUndefined_WhenValueIsUndefined_ShouldThrowArgumentOutOfRangeException(TestEnum value)
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				ThrowHelper.ThrowIfEnumValueIsUndefined(value);
			});
		}

		[DataTestMethod]
		[DataRow(TestEnum.A)]
		[DataRow(TestEnum.B)]
		public void ThrowIfEnumValueIsUndefined_WhenValueIsDefined_ShouldNotThrow(TestEnum value)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(value);
		}
	}
}