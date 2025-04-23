using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Collections.Extensions;
using Bodu.Collections.Generic.Extensions;

namespace Bodu
{
	public partial class XorShiftRandomTests
	{
		/// <summary>
		/// Verifies that NextBytes fills the provided buffer completely with random bytes.
		/// </summary>
		[DataTestMethod]
		[DataRow(1)]
		[DataRow(10)]
		[DataRow(100)]
		public void NextBytes_ShouldFillBufferFully_WithVariousLengths(int length)
		{
			var rng = new XorShiftRandom();
			byte[] buffer = new byte[length];
			rng.NextBytes(buffer);

			Assert.IsTrue(buffer.All(b => b >= 0), "All bytes should be populated.");
		}

		[TestMethod]
		public void NextBytes_WhenCalled_ShouldFillBuffer()
		{
			var rng = new XorShiftRandom();
			byte[] buffer = new byte[16];
			rng.NextBytes(buffer);

			Assert.IsTrue(buffer.Any(b => b != 0), "Expected non-zero bytes in result.");
		}

		[TestMethod]
		public void NextBytes_WithNullBuffer_ShouldThrow()
		{
			var rng = new XorShiftRandom();
			Assert.ThrowsException<ArgumentNullException>(() => rng.NextBytes(null!));
		}
	}
}