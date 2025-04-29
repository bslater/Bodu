// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfNotOfType.cs" company="PlaceholderCompany">
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
		public void ThrowIfNotOfType_WhenWrongType_ShouldThrow()
		{
			object value = "string";
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfNotOfType<int>(value));
		}

		[TestMethod]
		public void ThrowIfNotOfType_WhenCorrectType_ShouldNotThrow()
		{
			object value = 42;
			ThrowHelper.ThrowIfNotOfType<int>(value);
		}

		[TestMethod]
		public void ThrowIfNotOfType_WhenNullAndTypeIsNonNullable_ShouldThrow()
		{
			object? value = null;
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfNotOfType<int>(value));
		}

		[TestMethod]
		public void ThrowIfNotOfType_WhenNullAndTypeIsReferenceType_ShouldNotThrow()
		{
			object? value = null;
			ThrowHelper.ThrowIfNotOfType<string>(value);
		}
	}
}