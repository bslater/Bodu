// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfInvalidStringComparison.cs" company="PlaceholderCompany">
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
		public void ThrowIfInvalidStringComparison_WhenInvalidValue_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfInvalidStringComparison((StringComparison)999));
		}

		[TestMethod]
		public void ThrowIfInvalidStringComparison_WhenValidValue_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfInvalidStringComparison(StringComparison.Ordinal);
			ThrowHelper.ThrowIfInvalidStringComparison(StringComparison.InvariantCultureIgnoreCase);
		}
	}
}