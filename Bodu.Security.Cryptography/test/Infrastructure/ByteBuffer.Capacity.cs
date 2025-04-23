using Bodu.Security.Cryptography;

namespace Bodu.Infrastructure
{
    [TestClass]
    public class Capacity
    {
        /// <summary>
        /// Verifies that the buffer's Capacity property returns the correct size.
        /// </summary>
        [TestMethod]
        public void ShouldReturnSpecifiedCapacity()
        {
            var buffer = new ByteBuffer(8);
            Assert.AreEqual(8, buffer.Capacity);
        }
    }
}