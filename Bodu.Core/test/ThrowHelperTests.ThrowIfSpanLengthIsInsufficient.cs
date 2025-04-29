// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfSpanLengthIsInsufficient_ReadOnlySpan.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfSpanLengthIsInsufficient_ReadOnlySpan_WhenInsufficient_ShouldThrow()
		{
			var span = new int[5];
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfSpanLengthIsInsufficient(new ReadOnlySpan<int>(span), 2, 5));
		}

		[TestMethod]
		public void ThrowIfSpanLengthIsInsufficient_ReadOnlySpan_WhenSufficient_ShouldNotThrow()
		{
			ReadOnlySpan<int> span = new int[10];
			ThrowHelper.ThrowIfSpanLengthIsInsufficient(span, 2, 5);
		}

		[TestMethod]
		public void ThrowIfSpanLengthIsInsufficient_Span_WhenInsufficient_ShouldThrow()
		{
			var span = new int[5];
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfSpanLengthIsInsufficient(span.AsSpan(), 2, 5));
		}

		[TestMethod]
		public void ThrowIfSpanLengthIsInsufficient_Span_WhenSufficient_ShouldNotThrow()
		{
			Span<int> span = new int[10];
			ThrowHelper.ThrowIfSpanLengthIsInsufficient(span, 2, 5);
		}
	}
}