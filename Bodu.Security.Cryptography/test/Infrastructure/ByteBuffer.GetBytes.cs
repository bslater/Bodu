using Bodu.Security.Cryptography;

namespace Bodu.Infrastructure
{
    [TestClass]
    public class GetBytes
    {
        /// <summary>
        /// Verifies that the full buffer is returned after filling.
        /// </summary>
        [TestMethod]
        public void WhenBufferIsFull_ShouldReturnBuffer()
        {
            var buffer = new ByteBuffer(2);
            buffer.Add(new byte[] { 1, 2 }, 0, 2);
            var result = buffer.GetBytes();
            CollectionAssert.AreEqual(new byte[] { 1, 2 }, result);
        }

        /// <summary>
        /// Verifies that calling GetRandomBytes on an incomplete buffer throws InvalidOperationException.
        /// </summary>
        [TestMethod]
        public void WhenBufferNotFull_ShouldThrowExactly()
        {
            var buffer = new ByteBuffer(3);
            buffer.Add(new byte[] { 1 }, 0, 1);
            Assert.ThrowsExactly<InvalidOperationException>(() => buffer.GetBytes());
        }

        /// <summary>
        /// Verifies that calling GetRandomBytes resets the buffer.
        /// </summary>
        [TestMethod]
        public void AfterCall_ShouldResetIndex()
        {
            var buffer = new ByteBuffer(1);
            buffer.Add(new byte[] { 1 }, 0, 1);
            buffer.GetBytes();
            Assert.IsTrue(buffer.IsEmpty);
        }

        /// <summary>
        /// Verifies that GetRandomBytes cannot be called twice without refilling the buffer.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenGetBytesCalledTwice_ShouldThrowExactly()
        {
            var buffer = new ByteBuffer(2);
            buffer.Add(new byte[] { 1, 2 }, 0, 2);
            var _ = buffer.GetBytes(); // OK
            buffer.GetBytes();
        }
    }
}