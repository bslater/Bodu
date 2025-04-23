using Bodu.Security.Cryptography;

namespace Bodu.Infrastructure
{
    [TestClass]
    public class IsEmpty
    {
        /// <summary>
        /// Verifies that a newly initialized buffer is empty.
        /// </summary>
        [TestMethod]
        public void WhenInitialized_ShouldReturnTrue()
        {
            var buffer = new ByteBuffer(4);
            Assert.IsTrue(buffer.IsEmpty);
        }

        /// <summary>
        /// Verifies that adding bytes makes the buffer no longer empty.
        /// </summary>
        [TestMethod]
        public void WhenBytesAdded_ShouldReturnFalse()
        {
            var buffer = new ByteBuffer(4);
            buffer.Add(new byte[] { 1 }, 0, 1);
            Assert.IsFalse(buffer.IsEmpty);
        }

        /// <summary>
        /// Verifies that calling Initialize resets the buffer to empty.
        /// </summary>
        [TestMethod]
        public void WhenBufferReset_ShouldReturnTrue()
        {
            var buffer = new ByteBuffer(4);
            buffer.Add(new byte[] { 1 }, 0, 1);
            buffer.Initialize();
            Assert.IsTrue(buffer.IsEmpty);
        }

        /// <summary>
        /// Verifies that a full buffer is not considered empty.
        /// </summary>
        [TestMethod]
        public void WhenBufferIsFull_ShouldReturnFalse()
        {
            var buffer = new ByteBuffer(2);
            buffer.Add(new byte[] { 1, 2 }, 0, 2);
            Assert.IsFalse(buffer.IsEmpty);
        }
    }
}