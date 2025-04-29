// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfDestinationTooSmall_Span.cs" company="PlaceholderCompany">
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
		public void ThrowIfDestinationTooSmall_Span_WhenDestinationTooSmall_ShouldThrow()
		{
			var source = new byte[10];
			var destination = new byte[5];
			try
			{
				ThrowHelper.ThrowIfDestinationTooSmall<byte, byte>(source.AsSpan(), destination.AsSpan());
				Assert.Fail("Expected ArgumentException not thrown.");
			}
			catch (ArgumentException ex)
			{
				Assert.IsTrue(ex.Message.Contains("destination span"));
			}
		}

		[TestMethod]
		public void ThrowIfDestinationTooSmall_Span_WhenDestinationSufficient_ShouldNotThrow()
		{
			ReadOnlySpan<int> source = new int[5];
			Span<int> destination = new int[5];
			ThrowHelper.ThrowIfDestinationTooSmall(source, destination);
		}

		[TestMethod]
		public void ThrowIfDestinationTooSmall_Array_WhenDestinationTooSmall_ShouldThrow()
		{
			int[] source = new int[5];
			byte[] destination = new byte[3];
			Assert.ThrowsExactly<ArgumentException>(() => ThrowHelper.ThrowIfDestinationTooSmall(source, destination));
		}

		[TestMethod]
		public void ThrowIfDestinationTooSmall_Array_WhenDestinationSufficient_ShouldNotThrow()
		{
			int[] source = new int[5];
			byte[] destination = new byte[5];
			ThrowHelper.ThrowIfDestinationTooSmall(source, destination);
		}
	}
}