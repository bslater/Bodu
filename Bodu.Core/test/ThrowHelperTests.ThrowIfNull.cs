// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfNull.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfNull_WhenValueIsNull_ShouldThrow()
		{
			object? value = null;
			Assert.ThrowsExactly<ArgumentNullException>(() => ThrowHelper.ThrowIfNull(value));
		}

		[TestMethod]
		public void ThrowIfNull_WhenValueIsNotNull_ShouldNotThrow()
		{
			object value = new object();
			ThrowHelper.ThrowIfNull(value);
		}

		[TestMethod]
		public void ThrowIfNull_WithMessage_WhenValueIsNull_ShouldThrowWithMessage()
		{
			object? value = null;
			var ex = Assert.ThrowsExactly<ArgumentNullException>(() => ThrowHelper.ThrowIfNull(value, "Custom message"));
			StringAssert.Contains(ex.Message, "Custom message");
		}

		[TestMethod]
		public void ThrowIfNull_WithMessage_WhenValueIsNotNull_ShouldNotThrow()
		{
			object value = new object();
			ThrowHelper.ThrowIfNull(value, "Custom message");
		}
	}
}