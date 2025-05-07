using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bodu.Collections.Generic.Concurrent
{
	[TestClass]
	public partial class ConcurrentCircularBufferTests
	{
		public TestContext TestContext { get; set; }

		private const int DefaultCapacity = 16;

		private sealed class TestItem
		{
			public int Value { get; set; }

			public TestItem(int value) => Value = value;

			public override string ToString() => $"Item({Value})";
		}

		private static void AssertBufferContainsExactlyValues(
			ConcurrentCircularBuffer<TestItem> buffer,
			params int[] expectedValues)
		{
			var snapshot = buffer.ToArray();
			Assert.AreEqual(expectedValues.Length, snapshot.Length, "Buffer item count mismatch.");

			for (int i = 0; i < expectedValues.Length; i++)
			{
				Assert.IsNotNull(snapshot[i], $"Item at index {i} was null.");
				Assert.AreEqual(expectedValues[i], snapshot[i].Value, $"Item at index {i} did not match expected value.");
			}
		}

		private static void AssertBufferContainsOnlyValuesInRange(
			ConcurrentCircularBuffer<TestItem> buffer,
			int expectedCount,
			int minInclusive,
			int maxInclusive)
		{
			var snapshot = buffer.ToArray();

			Assert.AreEqual(expectedCount, snapshot.Length, $"Expected buffer to contain {expectedCount} items.");

			foreach (var item in snapshot)
			{
				Assert.IsNotNull(item, "Buffer contained a null item.");
				Assert.IsTrue(item.Value >= minInclusive && item.Value <= maxInclusive,
					$"Item value {item.Value} was outside the expected range [{minInclusive}, {maxInclusive}].");
			}
		}
	}
}