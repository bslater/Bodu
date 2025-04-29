using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bodu
{
	public sealed partial class ThrowHelperTests
	{
		[TestMethod]
		public void ThrowIfArrayOffsetOrCountInvalid_WhenIndexIsNegative_ShouldThrowExactly()
		{
			var array = new int[5];

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				ThrowHelper.ThrowIfArrayOffsetOrCountInvalid(array, -1, 2);
			});
		}

		[TestMethod]
		public void ThrowIfArrayOffsetOrCountInvalid_WhenIndexExceedsArrayLength_ShouldThrowExactly()
		{
			var array = new int[5];

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				ThrowHelper.ThrowIfArrayOffsetOrCountInvalid(array, 6, 2);
			});
		}

		[TestMethod]
		public void ThrowIfArrayOffsetOrCountInvalid_WhenCountIsNegative_ShouldThrowExactly()
		{
			var array = new int[5];

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				ThrowHelper.ThrowIfArrayOffsetOrCountInvalid(array, 2, -1);
			});
		}

		[TestMethod]
		public void ThrowIfArrayOffsetOrCountInvalid_WhenCountExceedsArrayLength_ShouldThrowExactly()
		{
			var array = new int[5];

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				ThrowHelper.ThrowIfArrayOffsetOrCountInvalid(array, 2, 10);
			});
		}

		[TestMethod]
		public void ThrowIfArrayOffsetOrCountInvalid_WhenSumOfIndexAndCountExceedsArrayLength_ShouldThrowExactly()
		{
			var array = new int[5];

			Assert.ThrowsExactly<ArgumentException>(() =>
			{
				ThrowHelper.ThrowIfArrayOffsetOrCountInvalid(array, 3, 3);
			});
		}

		[TestMethod]
		public void ThrowIfArrayOffsetOrCountInvalid_WhenParametersAreValid_ShouldNotThrow()
		{
			var array = new int[5];

			ThrowHelper.ThrowIfArrayOffsetOrCountInvalid(array, 1, 3);
		}
	}
}