using Bodu.Security.Cryptography;

namespace Bodu.Infrastructure
{
    [TestClass]
    public class Ctor
    {
        /// <summary>
        /// Verifies that the constructor sets the correct capacity.
        /// </summary>
        [TestMethod]
        public void WhenCalledWithPositiveCapacity_ShouldSetCapacity()
        {
            var buffer = new ByteBuffer(10);
            Assert.AreEqual(10, buffer.Capacity);
        }

        /// <summary>
        /// Verifies that the buffer is initially empty after construction.
        /// </summary>
        [TestMethod]
        public void WhenConstructed_ShouldBeEmpty()
        {
            var buffer = new ByteBuffer(5);
            Assert.IsTrue(buffer.IsEmpty);
        }

        /// <summary>
        /// Verifies that the constructor throws when called with negative capacity.
        /// </summary>
        [TestMethod]
        public void WhenCalledWithNegativeCapacity_ShouldThrowArgumentOutOfRangeException()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ByteBuffer(-1));
        }

        /// <summary>
        /// Verifies that a buffer with zero capacity is immediately full.
        /// </summary>
        [TestMethod]
        public void WhenCapacityIsZero_ShouldBeImmediatelyFull()
        {
            var buffer = new ByteBuffer(0);
            Assert.IsTrue(buffer.IsFull);
            Assert.IsTrue(buffer.IsEmpty); // also logically empty
        }
    }
}