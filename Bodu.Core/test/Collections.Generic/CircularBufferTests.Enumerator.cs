namespace Bodu.Collections.Generic
{
	public partial class CircularBufferTests
	{
		/// <summary>
		/// Verifies that enumerator iterates over buffer elements in FirstInFirstOut (First-In,
		/// First-Out) order.
		/// </summary>
		[TestMethod]
		public void Enumerator_WhenBufferHasItems_ShouldIterateInFifoOrder()
		{
			var buffer = new CircularBuffer<int>(3);
			buffer.Enqueue(1);
			buffer.Enqueue(2);
			buffer.Enqueue(3);

			var items = buffer.ToList();
			CollectionAssert.AreEqual(new[] { 1, 2, 3 }, items);
		}

		/// <summary>
		/// Verifies that modifying the buffer during enumeration throws InvalidOperationException.
		/// </summary>
		[TestMethod]
		public void Enumerator_WhenModifiedDuringIteration_ShouldThrowExactly()
		{
			var buffer = new CircularBuffer<int>(3);
			buffer.Enqueue(1);
			buffer.Enqueue(2);

			var enumerator = buffer.GetEnumerator();
			Assert.ThrowsExactly<InvalidOperationException>(() =>
			{
				while (enumerator.MoveNext())
				{
					buffer.Enqueue(3); // Modification during iteration
				}
			});
		}

		/// <summary>
		/// Verifies that enumeration remains consistent after multiple wraparounds and mutations.
		/// </summary>
		[TestMethod]
		public void Enumerator_AfterMultipleStateChanges_ShouldYieldCorrectSequence()
		{
			var buffer = new CircularBuffer<int>(3);
			buffer.Enqueue(10);
			buffer.Enqueue(20);
			buffer.Enqueue(30);
			buffer.Dequeue();         // 10 removed
			buffer.Enqueue(40);       // wrap
			buffer.Dequeue();         // 20 removed
			buffer.Enqueue(50);       // wrap again

			var result = buffer.ToList();

			CollectionAssert.AreEqual(new[] { 30, 40, 50 }, result);
		}

		/// <summary>
		/// Verifies that null values are preserved in enumeration in correct order.
		/// </summary>
		[TestMethod]
		public void Enumerator_WhenBufferContainsNulls_ShouldYieldNullsInCorrectOrder()
		{
			var buffer = new CircularBuffer<string>(3);
			buffer.Enqueue("X");
			buffer.Enqueue(null);
			buffer.Enqueue("Y");

			var result = buffer.ToArray();
			CollectionAssert.AreEqual(new[] { "X", null, "Y" }, result);
		}

		/// <summary>
		/// Verifies that enumerator yields no elements when the buffer is empty.
		/// </summary>
		[TestMethod]
		public void Enumerator_WhenBufferIsEmpty_ShouldYieldNoItems()
		{
			var buffer = new CircularBuffer<string>(5);
			var items = buffer.ToList();
			Assert.AreEqual(0, items.Count);
		}

		/// <summary>
		/// Verifies that enumeration returns correct order of elements after the buffer wraps around.
		/// </summary>
		[TestMethod]
		public void Enumerator_WhenBufferIsWrapped_ShouldYieldCorrectOrder()
		{
			var buffer = new CircularBuffer<int>(3);
			buffer.Enqueue(1);
			buffer.Enqueue(2);
			buffer.Enqueue(3);
			buffer.Dequeue();       // remove 1
			buffer.Enqueue(4);      // wraps around

			var result = buffer.ToList();
			CollectionAssert.AreEqual(new[] { 2, 3, 4 }, result);
		}

		/// <summary>
		/// Verifies that enumerator yields null values if they are present in the buffer.
		/// </summary>
		[TestMethod]
		public void Enumerator_WhenBufferContainsNulls_ShouldYieldNullValues()
		{
			var buffer = new CircularBuffer<string>(2);
			buffer.Enqueue("A");
			buffer.Enqueue(null);

			var result = buffer.ToArray();
			Assert.AreEqual("A", result[0]);
			Assert.IsNull(result[1]);
		}
	}
}