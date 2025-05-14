// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfNotPositiveMultipleOf.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[DataTestMethod]
		[DataRow(0, 2)]   // Zero is not positive
		[DataRow(-2, 2)]  // Negative value
		[DataRow(7, 2)]   // Not a multiple
		[DataRow(5, 3)]   // Not a multiple
		public void ThrowIfNotPositiveMultipleOf_WhenValueIsZeroNegativeOrNotMultiple_ShouldThrow(int value, int factor)
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				ThrowHelper.ThrowIfNotPositiveMultipleOf(value, factor);
			});
		}

		[DataTestMethod]
		[DataRow(2, 1)]
		[DataRow(4, 2)]
		[DataRow(9, 3)]
		[DataRow(10, 5)]
		public void ThrowIfNotPositiveMultipleOf_WhenValueIsPositiveAndMultiple_ShouldNotThrow(int value, int factor)
		{
			ThrowHelper.ThrowIfNotPositiveMultipleOf(value, factor);
		}
	}
}