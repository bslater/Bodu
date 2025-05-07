using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void Ctor_WhenUsedInParallel_ShouldInitializeCorrectly()
		{
			var results = new ConcurrentBag<ConcurrentCircularBuffer<TestItem>>();

			Parallel.For(0, 100, _ =>
			{
				var buffer = new ConcurrentCircularBuffer<TestItem>(5);
				buffer.Enqueue(new TestItem(1));
				results.Add(buffer);
			});

			Assert.AreEqual(100, results.Count);
			Assert.IsTrue(results.All(b => b.Capacity == 5));
			Assert.IsTrue(results.All(b => b.Count == 1));
		}

		[TestMethod]
		public void Ctor_WhenDefaultConstructorUsedConcurrently_ShouldHaveDefaultCapacity()
		{
			var results = new ConcurrentBag<ConcurrentCircularBuffer<TestItem>>();

			Parallel.For(0, 50, _ =>
			{
				var buffer = new ConcurrentCircularBuffer<TestItem>();
				buffer.Enqueue(new TestItem(1));
				results.Add(buffer);
			});

			Assert.AreEqual(50, results.Count);
			Assert.IsTrue(results.All(b => b.Count == 1));
			Assert.IsTrue(results.All(b => b.AllowOverwrite));
			Assert.IsTrue(results.All(b => b.Capacity > 0)); // Assuming DefaultCapacity is internal
		}

		[TestMethod]
		public void Ctor_WhenAllowOverwriteFalse_ShouldRespectFlagConcurrently()
		{
			var results = new ConcurrentBag<ConcurrentCircularBuffer<TestItem>>();

			Parallel.For(0, 20, _ =>
			{
				var buffer = new ConcurrentCircularBuffer<TestItem>(3, allowOverwrite: false);
				buffer.Enqueue(new TestItem(1));
				results.Add(buffer);
			});

			Assert.IsTrue(results.All(b => b.AllowOverwrite == false));
			Assert.IsTrue(results.All(b => b.Count == 1));
		}

		[TestMethod]
		public void Ctor_WhenEnumerableProvided_ShouldInitializeCorrectly()
		{
			var source = Enumerable.Range(1, 3).Select(i => new TestItem(i)).ToArray();

			var results = new ConcurrentBag<ConcurrentCircularBuffer<TestItem>>();

			Parallel.For(0, 30, _ =>
			{
				var buffer = new ConcurrentCircularBuffer<TestItem>(source);
				results.Add(buffer);
			});

			Assert.AreEqual(30, results.Count);
			Assert.IsTrue(results.All(b => b.Count == 3));
		}

		[TestMethod]
		public void Ctor_WhenEnumerableWithCapacity_ShouldTrimCorrectly()
		{
			var source = Enumerable.Range(1, 5).Select(i => new TestItem(i));

			var buffer = new ConcurrentCircularBuffer<TestItem>(source, 3);

			var values = buffer.ToArray().Select(x => x.Value).ToArray();
			CollectionAssert.AreEqual(new[] { 3, 4, 5 }, values);
		}

		[TestMethod]
		public void Ctor_WhenAllParametersUsed_ShouldRespectAllValues()
		{
			var source = Enumerable.Range(1, 3).Select(i => new TestItem(i));
			var buffer = new ConcurrentCircularBuffer<TestItem>(source, 3, false);

			Assert.AreEqual(3, buffer.Capacity);
			Assert.AreEqual(3, buffer.Count);
			Assert.IsFalse(buffer.AllowOverwrite);
		}

		[TestMethod]
		public void Ctor_WhenCollectionIsNull_ShouldThrowExactly()
		{
			Assert.ThrowsException<ArgumentNullException>(() =>
			{
				_ = new ConcurrentCircularBuffer<TestItem>((IEnumerable<TestItem>)null);
			});
		}

		[DataTestMethod]
		[DataRow(-1)]
		[DataRow(0)]
		public void Ctor_WhenCapacityIsInvalid_ShouldThrowExactly(int capacity)
		{
			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			{
				_ = new ConcurrentCircularBuffer<TestItem>(capacity);
			});
		}

		[TestMethod]
		public void Ctor_WhenCollectionExceedsCapacityAndOverwriteIsFalse_ShouldThrowExactly()
		{
			var source = Enumerable.Range(1, 5).Select(i => new TestItem(i));

			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				_ = new ConcurrentCircularBuffer<TestItem>(source, 3, allowOverwrite: false);
			});
		}

		[TestMethod]
		public void Ctor_WhenNegativeCapacityAndEmptyCollection_ShouldThrowExactly()
		{
			var empty = Array.Empty<TestItem>();
			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			{
				_ = new ConcurrentCircularBuffer<TestItem>(empty, -1);
			});
		}

		[TestMethod]
		public void Ctor_WhenNegativeCapacityAndOverwriteTrue_ShouldThrowExactly()
		{
			var empty = Array.Empty<TestItem>();
			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			{
				_ = new ConcurrentCircularBuffer<TestItem>(empty, -5, allowOverwrite: true);
			});
		}

		[TestMethod]
		public void Ctor_WhenNonArrayEnumerableUsed_ShouldInitializeWithToArray()
		{
			IEnumerable<TestItem> source = Enumerable.Range(1, 3).Select(x => new TestItem(x));
			var buffer = new ConcurrentCircularBuffer<TestItem>(source, 5);

			var values = buffer.ToArray().Select(x => x.Value).ToArray();
			CollectionAssert.AreEqual(new[] { 1, 2, 3 }, values);
		}
	}
}