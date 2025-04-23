namespace Bodu.Collections.Generic
{
	public partial class CircularBufferTests
	{
		/// <summary>
		/// Verifies that Count tracks the current number of elements.
		/// </summary>
		[TestMethod]
		public void Count_WhenItemsAreEnqueuedAndDequeued_ShouldReflectCorrectItemCount()
		{
			var buffer = new CircularBuffer<char>(3);
			Assert.AreEqual(0, buffer.Count);

			buffer.Enqueue('a');
			Assert.AreEqual(1, buffer.Count);

			buffer.Enqueue('b');
			buffer.Dequeue();
			Assert.AreEqual(1, buffer.Count);
		}
	}
}