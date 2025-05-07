using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void CopyTo_WhenBufferHasElements_ShouldCopyToArrayCorrectly()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));

			var array = new TestItem[3];
			buffer.CopyTo(array, 0);

			Assert.AreEqual(1, array[0]?.Value);
			Assert.AreEqual(2, array[1]?.Value);
			Assert.IsNull(array[2]);
		}

		[TestMethod]
		public void CopyTo_WhenTargetArrayIsNull_ShouldThrowExactly()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(1);
			buffer.Enqueue(new TestItem(1));

			Assert.ThrowsException<ArgumentNullException>(() =>
			{
				buffer.CopyTo(null, 0);
			});
		}

		[TestMethod]
		public void CopyTo_WhenTargetIndexIsNegative_ShouldThrowExactly()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(2);
			buffer.Enqueue(new TestItem(1));

			var array = new TestItem[3];
			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			{
				buffer.CopyTo(array, -1);
			});
		}

		[TestMethod]
		public void CopyTo_WhenTargetArrayIsTooSmall_ShouldThrowExactly()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(2);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));

			var array = new TestItem[1];
			Assert.ThrowsException<ArgumentException>(() =>
			{
				buffer.CopyTo(array, 0);
			});
		}

		[TestMethod]
		public void CopyTo_WhenBufferContainsNull_ShouldIncludeNullInTargetArray()
		{
			var buffer = new ConcurrentCircularBuffer<string>(2);
			buffer.Enqueue(null);
			buffer.Enqueue("X");

			var array = new string[2];
			buffer.CopyTo(array, 0);

			Assert.IsNull(array[0]);
			Assert.AreEqual("X", array[1]);
		}

		[TestMethod]
		public void CopyTo_WhenConcurrentEnqueue_ShouldNotThrowAndCopyConsistently()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			for (int i = 0; i < 5; i++) buffer.Enqueue(new TestItem(i));

			var exceptions = new ConcurrentBag<Exception>();
			var copies = new ConcurrentBag<TestItem[]>();

			var writer = Task.Run(() =>
			{
				for (int i = 5; i < 50; i++)
				{
					buffer.TryEnqueue(new TestItem(i));
					Thread.SpinWait(10);
				}
			});

			var copier = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					try
					{
						var array = new TestItem[buffer.Capacity];
						buffer.CopyTo(array, 0);
						copies.Add(array);
					}
					catch (Exception ex)
					{
						exceptions.Add(ex);
					}
					Thread.SpinWait(5);
				}
			});

			Task.WaitAll(writer, copier);

			Assert.AreEqual(0, exceptions.Count, "CopyTo threw an exception under concurrency.");
			Assert.IsTrue(copies.All(copy => copy.Length == buffer.Capacity));
		}

		[TestMethod]
		public void CopyTo_WhenConcurrentClear_ShouldNotThrow()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);
			for (int i = 0; i < 5; i++) buffer.Enqueue(new TestItem(i));

			var exceptions = new ConcurrentBag<Exception>();

			var clearer = Task.Run(() =>
			{
				for (int i = 0; i < 50; i++)
				{
					buffer.Clear();
					Thread.SpinWait(10);
				}
			});

			var copier = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					try
					{
						var array = new TestItem[buffer.Capacity];
						buffer.CopyTo(array, 0);
					}
					catch (Exception ex)
					{
						exceptions.Add(ex);
					}
					Thread.SpinWait(5);
				}
			});

			Task.WaitAll(clearer, copier);

			Assert.AreEqual(0, exceptions.Count, "CopyTo threw during concurrent Clear.");
		}
	}
}