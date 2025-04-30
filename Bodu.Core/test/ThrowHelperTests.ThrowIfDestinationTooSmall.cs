// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfDestinationTooSmall.cs" company="PlaceholderCompany">
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
		[DataRow(10, 5)] // destination too small
		[DataRow(5, 4)]
		public void ThrowIfDestinationTooSmall_Span_WhenDestinationTooSmall_ShouldThrowArgumentException(int sourceLength, int destinationLength)
		{
			var source = new byte[sourceLength];
			var destination = new byte[destinationLength];

			Assert.ThrowsExactly<ArgumentException>(() =>
			{
				ThrowHelper.ThrowIfDestinationTooSmall<byte, byte>(source.AsSpan(), destination.AsSpan());
			});
		}

		[DataTestMethod]
		[DataRow(5, 5)]
		[DataRow(3, 5)]
		[DataRow(0, 0)]
		public void ThrowIfDestinationTooSmall_Span_WhenDestinationSufficient_ShouldNotThrow(int sourceLength, int destinationLength)
		{
			var source = new int[sourceLength];
			var destination = new int[destinationLength];

			ThrowHelper.ThrowIfDestinationTooSmall<int, int>(source.AsSpan(), destination.AsSpan());
		}

		[DataTestMethod]
		[DataRow(5, 3)] // destination too small
		[DataRow(4, 2)]
		public void ThrowIfDestinationTooSmall_Array_WhenDestinationTooSmall_ShouldThrowArgumentException(int sourceLength, int destinationLength)
		{
			var source = new int[sourceLength];
			var destination = new byte[destinationLength];

			Assert.ThrowsExactly<ArgumentException>(() =>
			{
				ThrowHelper.ThrowIfDestinationTooSmall(source, destination);
			});
		}

		[DataTestMethod]
		[DataRow(5, 5)]
		[DataRow(3, 5)]
		[DataRow(0, 0)]
		public void ThrowIfDestinationTooSmall_Array_WhenDestinationSufficient_ShouldNotThrow(int sourceLength, int destinationLength)
		{
			var source = new int[sourceLength];
			var destination = new byte[destinationLength];

			ThrowHelper.ThrowIfDestinationTooSmall(source, destination);
		}
	}
}