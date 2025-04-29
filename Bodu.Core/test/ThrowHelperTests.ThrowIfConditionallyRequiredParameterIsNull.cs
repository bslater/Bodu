// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfConditionallyRequiredParameterIsNull.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfConditionallyRequiredParameterIsNull_WhenConditionMatchesAndValueIsNull_ShouldThrowExactly()
		{
			string? value = null;
			var condition = true;
			Assert.ThrowsExactly<ArgumentException>(() =>
			{
				ThrowHelper.ThrowIfConditionallyRequiredParameterIsNull(value, condition, true);
			});
		}

		[TestMethod]
		public void ThrowIfConditionallyRequiredParameterIsNull_WhenConditionDoesNotMatch_ShouldNotThrow()
		{
			string? value = null;
			var condition = false;
			ThrowHelper.ThrowIfConditionallyRequiredParameterIsNull(value, condition, true);
		}

		[TestMethod]
		public void ThrowIfConditionallyRequiredParameterIsNull_WhenConditionMatchesAndValueIsNotNull_ShouldNotThrow()
		{
			string? value = "ok";
			var condition = true;
			ThrowHelper.ThrowIfConditionallyRequiredParameterIsNull(value, condition, true);
		}
	}
}