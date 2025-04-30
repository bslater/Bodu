// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfSpanLengthIsInsufficient_ReadOnlySpan.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[DataTestMethod]
		[DataRow(5, 2, 5)]   // span.Length = 5; offset = 2; count = 5 => insufficient
		[DataRow(4, 0, 5)]   // span.Length = 4; offset = 0; count = 5 => insufficient
		public void ThrowIfSpanLengthIsInsufficient_ReadOnlySpan_WhenInsufficient_ShouldThrow(int spanLength, int offset, int count)
		{
			var span = new int[spanLength];
			Assert.ThrowsExactly<ArgumentException>(() =>
			{
				ThrowHelper.ThrowIfSpanLengthIsInsufficient(new ReadOnlySpan<int>(span), offset, count);
			});
		}

		[DataTestMethod]
		[DataRow(10, 2, 5)]   // span.Length = 10; offset + count = 7 <= 10
		[DataRow(6, 0, 6)]    // offset 0 + count 6 = 6 == span.Length
		[DataRow(5, 0, 0)]    // offset 0 + count 0 = 0 <= span.Length
		public void ThrowIfSpanLengthIsInsufficient_ReadOnlySpan_WhenSufficient_ShouldNotThrow(int spanLength, int offset, int count)
		{
			ReadOnlySpan<int> span = new int[spanLength];
			ThrowHelper.ThrowIfSpanLengthIsInsufficient(span, offset, count);
		}

		[DataTestMethod]
		[DataRow(5, 2, 5)]
		[DataRow(4, 1, 4)]
		public void ThrowIfSpanLengthIsInsufficient_Span_WhenInsufficient_ShouldThrow(int spanLength, int offset, int count)
		{
			var span = new int[spanLength];
			Assert.ThrowsExactly<ArgumentException>(() =>
			{
				ThrowHelper.ThrowIfSpanLengthIsInsufficient(span.AsSpan(), offset, count);
			});
		}

		[DataTestMethod]
		[DataRow(10, 2, 5)]
		[DataRow(6, 0, 6)]
		[DataRow(7, 3, 4)]
		public void ThrowIfSpanLengthIsInsufficient_Span_WhenSufficient_ShouldNotThrow(int spanLength, int offset, int count)
		{
			Span<int> span = new int[spanLength];
			ThrowHelper.ThrowIfSpanLengthIsInsufficient(span, offset, count);
		}
	}
}