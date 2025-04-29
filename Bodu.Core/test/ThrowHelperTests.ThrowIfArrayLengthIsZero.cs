using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bodu.Core.Test
{
	public sealed partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfArrayLengthIsZero_WhenArrayLengthIsZero_ShouldThrowExactly()
		{
			// Arrange
			var array = new int[0];

			// Act and Assert
			Assert.ThrowsExactly<ArgumentException>(() =>
			{
				ThrowHelper.ThrowIfArrayLengthIsZero(array);
			});
		}

		[TestMethod]
		public void ThrowIfArrayLengthIsZero_WhenArrayLengthIsNonZero_ShouldNotThrow()
		{
			// Arrange
			var array = new int[1];

			// Act and Assert
			ThrowHelper.ThrowIfArrayLengthIsZero(array); // Should not throw
		}
	}
}