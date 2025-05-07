using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void Enumerator_WhenSnapshotTakenDuringConcurrentEnqueue_ShouldNotThrow()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(20);

			for (int i = 0; i < 10; i++)
				buffer.Enqueue(new TestItem(i));

			var reader = Task.Run(() =>
			{
				var snapshot = buffer.ToList(); // triggers enumeration
				Assert.IsTrue(snapshot.All(x => x != null));
			});

			var writer = Task.Run(() =>
			{
				for (int i = 10; i < 30; i++)
					buffer.TryEnqueue(new TestItem(i));
			});

			Task.WaitAll(reader, writer);
		}

		[TestMethod]
		public void Enumerator_WhenConcurrentDequeueOccurs_ShouldYieldStableSnapshot()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			for (int i = 0; i < 10; i++) buffer.Enqueue(new TestItem(i));

			var reader = Task.Run(() =>
			{
				var items = buffer.ToList();
				Assert.IsTrue(items.Count <= 10);
			});

			var remover = Task.Run(() =>
			{
				for (int i = 0; i < 10; i++)
					buffer.TryDequeue(out _);
			});

			Task.WaitAll(reader, remover);
		}

		[TestMethod]
		public void Enumerator_WhenBufferContainsNulls_ShouldYieldNullsInCorrectOrder()
		{
			var buffer = new ConcurrentCircularBuffer<string>(3);
			buffer.Enqueue("A");
			buffer.Enqueue(null);
			buffer.Enqueue("B");

			var items = buffer.ToArray();
			CollectionAssert.AreEqual(new[] { "A", null, "B" }, items);
		}

		[TestMethod]
		public void Enumerator_WhenBufferIsEmpty_ShouldYieldNoItems()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);
			var items = buffer.ToList();
			Assert.AreEqual(0, items.Count);
		}

		[TestMethod]
		public void Enumerator_WhenWraparoundHasOccurred_ShouldPreserveFifoOrder()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			buffer.Enqueue(new TestItem(3));
			buffer.Dequeue();       // removes 1
			buffer.Enqueue(new TestItem(4)); // wraparound

			var result = buffer.ToArray().Select(x => x.Value).ToArray();
			CollectionAssert.AreEqual(new[] { 2, 3, 4 }, result);
		}

		[TestMethod]
		public void Enumerator_WhenConcurrentMutationsOccur_ShouldYieldPartialConsistentView()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(100);
			for (int i = 0; i < 100; i++) buffer.Enqueue(new TestItem(i));

			var enumeratorTask = Task.Run(() =>
			{
				var snapshot = buffer.ToArray();
				Assert.IsTrue(snapshot.Length <= buffer.Capacity);
				Assert.IsTrue(snapshot.All(x => x is TestItem or null));
			});

			var mutateTask = Task.Run(() =>
			{
				for (int i = 100; i < 200; i++)
				{
					buffer.TryDequeue(out _);
					buffer.TryEnqueue(new TestItem(i));
				}
			});

			Task.WaitAll(enumeratorTask, mutateTask);
		}
	}
}