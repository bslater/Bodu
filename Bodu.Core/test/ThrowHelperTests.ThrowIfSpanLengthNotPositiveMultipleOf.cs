// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfSpanLengthNotPositiveMultipleOf_ReadOnlySpan.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfSpanLengthNotPositiveMultipleOf_ReadOnlySpan_WhenInvalid_ShouldThrow()
		{
			var span = new int[5];
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfSpanLengthNotPositiveMultipleOf(new ReadOnlySpan<int>(span), 2));
		}

		[TestMethod]
		public void ThrowIfSpanLengthNotPositiveMultipleOf_ReadOnlySpan_WhenValid_ShouldNotThrow()
		{
			ReadOnlySpan<int> span = new int[6];
			ThrowHelper.ThrowIfSpanLengthNotPositiveMultipleOf(span, 3);
		}

		[TestMethod]
		public void ThrowIfSpanLengthNotPositiveMultipleOf_Span_WhenInvalid_ShouldThrow()
		{
			var span = new int[5];
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfSpanLengthNotPositiveMultipleOf(span.AsSpan(), 2));
		}

		[TestMethod]
		public void ThrowIfSpanLengthNotPositiveMultipleOf_Span_WhenValid_ShouldNotThrow()
		{
			Span<int> span = new int[6];
			ThrowHelper.ThrowIfSpanLengthNotPositiveMultipleOf(span, 3);
		}
	}
}