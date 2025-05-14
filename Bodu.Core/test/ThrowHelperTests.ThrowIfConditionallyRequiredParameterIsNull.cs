// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfConditionallyRequiredParameterIsNull.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[DataTestMethod]
		[DataRow(null, true, true)]
		public void ThrowIfConditionallyRequiredParameterIsNull_WhenConditionMatchesAndValueIsNull_ShouldThrowArgumentException(string? value, bool condition, bool matchValue)
		{
			Assert.ThrowsExactly<ArgumentException>(() =>
			{
				ThrowHelper.ThrowIfConditionallyRequiredParameterIsNull(value, condition, matchValue);
			});
		}

		[DataTestMethod]
		[DataRow(null, false, true)]
		[DataRow(null, true, false)]
		[DataRow("ok", true, true)]
		[DataRow("ok", false, true)]
		public void ThrowIfConditionallyRequiredParameterIsNull_WhenConditionDoesNotMatchOrValueIsNotNull_ShouldNotThrow(string? value, bool condition, bool matchValue)
		{
			ThrowHelper.ThrowIfConditionallyRequiredParameterIsNull(value, condition, matchValue);
		}
	}
}