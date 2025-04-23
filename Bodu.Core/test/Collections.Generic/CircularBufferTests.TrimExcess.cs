namespace Bodu.Collections.Generic
{
	public partial class CircularBufferTests
	{
		/// <summary>
		/// Verifies that TrimExcess reduces the internal capacity to match Count when the buffer is
		/// not full.
		/// </summary>
		[TestMethod]
		public void TrimExcess_WhenBufferIsNotFull_ShouldReduceCapacityToCount()
		{
			var buffer = new CircularBuffer<int>(10);
			buffer.Enqueue(1);
			buffer.Enqueue(2);
			buffer.TrimExcess();

			Assert.AreEqual(2, buffer.Capacity);
			Assert.AreEqual(buffer.Count, buffer.Capacity);
			CollectionAssert.AreEqual(new[] { 1, 2 }, buffer.ToArray());
		}

		/// <summary>
		/// Verifies that TrimExcess does not change capacity when the buffer is full.
		/// </summary>
		[TestMethod]
		public void TrimExcess_WhenBufferIsFull_ShouldNotChangeCapacity()
		{
			var buffer = new CircularBuffer<int>(3);
			buffer.Enqueue(1);
			buffer.Enqueue(2);
			buffer.Enqueue(3);
			buffer.TrimExcess();

			Assert.AreEqual(3, buffer.Capacity);
		}

		/// <summary>
		/// Verifies that TrimExcess on an empty buffer does not throw and preserves usability.
		/// </summary>
		[TestMethod]
		public void TrimExcess_WhenBufferIsEmpty_ShouldNotThrowAndRemainUsable()
		{
			var buffer = new CircularBuffer<int>(5);
			buffer.TrimExcess();

			Assert.AreEqual(0, buffer.Count);

			buffer.Enqueue(123);
			Assert.AreEqual(1, buffer.Count);
			Assert.AreEqual(123, buffer.Dequeue());
		}

		/// <summary>
		/// Verifies that TrimExcess does nothing when capacity already equals the current item count.
		/// </summary>
		[TestMethod]
		public void TrimExcess_WhenCapacityEqualsCount_ShouldDoNothing()
		{
			var buffer = new CircularBuffer<string>(3);
			buffer.Enqueue("X");
			buffer.Enqueue("Y");
			buffer.Enqueue("Z");

			buffer.TrimExcess();

			Assert.AreEqual(3, buffer.Capacity);
			CollectionAssert.AreEqual(new[] { "X", "Y", "Z" }, buffer.ToArray());
		}

		/// <summary>
		/// Verifies that TrimExcess never reduces capacity below the number of stored items.
		/// </summary>
		[TestMethod]
		public void TrimExcess_WhenCalled_ShouldEnsureCapacityNotLessThanCount()
		{
			var buffer = new CircularBuffer<int>(5);
			buffer.Enqueue(100);
			buffer.Enqueue(200);
			buffer.Dequeue(); // count = 1
			buffer.TrimExcess();

			Assert.IsTrue(buffer.Capacity >= buffer.Count);
		}

		/// <summary>
		/// Verifies that TrimExcess preserves all elements and their logical order.
		/// </summary>
		[TestMethod]
		public void TrimExcess_WhenCalled_ShouldPreserveAllElementsAndOrder()
		{
			var buffer = new CircularBuffer<int>(10);
			buffer.Enqueue(1);
			buffer.Enqueue(2);
			buffer.Enqueue(3);
			buffer.Dequeue();         // remove 1
			buffer.Enqueue(4);        // wrap

			buffer.TrimExcess();
			CollectionAssert.AreEqual(new[] { 2, 3, 4 }, buffer.ToArray());

			buffer.Dequeue();         // remove 2
			buffer.Dequeue();         // remove 3
			buffer.TrimExcess();      // capacity should reduce to 1
			buffer.Enqueue(5);        // wrap

			CollectionAssert.AreEqual(new[] { 5 }, buffer.ToArray());
		}
	}
}