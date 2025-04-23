using Bodu.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Security.Cryptography
{
	public partial class SimpleSummingHashAlgorithmTests
	{
		// <summary>
		/// Verifies that the constructor sets <see cref="HashSize"/> to 32 bits (4 bytes). </summary>
		[TestMethod]
		public void Ctor_ShouldSetHashSizeTo32Bits()
		{
			using var algorithm = new SimpleSummingHashAlgorithm();
			Assert.AreEqual(32, algorithm.HashSize);
		}

		/// <summary>
		/// Verifies that the constructor initializes <see cref="SimpleSummingHashAlgorithm.BytesProcessed" /> to zero.
		/// </summary>
		[TestMethod]
		public void Ctor_ShouldSetBytesProcessedToZero()
		{
			using var algorithm = new SimpleSummingHashAlgorithm();
			Assert.AreEqual(0, algorithm.BytesProcessed);
		}

		/// <summary>
		/// Verifies that the initial hash result after construction is the 4-byte zero array.
		/// </summary>
		[TestMethod]
		public void Ctor_ShouldReturnZeroHash_WhenNoInputProcessed()
		{
			using var algorithm = new SimpleSummingHashAlgorithm();
			var result = algorithm.ComputeHash(Array.Empty<byte>());
			CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, result);
		}
	}
}