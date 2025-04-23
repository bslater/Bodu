using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Collections.Extensions;
using Bodu.Collections.Generic.Extensions;

namespace Bodu
{
	public partial class XorShiftRandomTests
	{
		[DataTestMethod]
		[DataRow(int.MinValue)]
		[DataRow(0)]
		[DataRow(int.MaxValue)]
		public void Constructor_WhenValidRange_ShouldCreateInstance(int seed)
		{
			var rng = new XorShiftRandom(seed);
			Assert.IsNotNull(rng);
		}

		[TestMethod]
		public void Constructor_WhenCalledWithoutSeed_ShouldCreateInstance()
		{
			var rng = new XorShiftRandom();
			Assert.IsNotNull(rng);
		}
	}
}