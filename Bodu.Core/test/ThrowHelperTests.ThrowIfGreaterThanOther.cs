// Auto-generated test stub for ThrowIfGreaterThanOther
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfGreaterThanOther_WhenValueIsGreater_ShouldThrow()
		{
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfGreaterThanOther(5, 3));
		}

		[TestMethod]
		public void ThrowIfGreaterThanOther_WhenValueIsLessOrEqual_ShouldNotThrow()
		{
			ThrowHelper.ThrowIfGreaterThanOther(2, 3);
			ThrowHelper.ThrowIfGreaterThanOther(3, 3);
		}
	}
}