using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	public abstract partial class HashAlgorithmTests<T>
	{
		/// <summary>
		/// Verifies that hashing data via <see cref="HashAlgorithm.TransformBlock" /> and
		/// <see cref="HashAlgorithm.TransformFinalBlock(byte[], int, int)" /> produces the same result as using
		/// <see cref="HashAlgorithm.ComputeHash(byte[])" /> with the full input.
		/// </summary>
		[TestMethod]
		public void TransformBlockAndFinalBlock_WhenUsedWithSimpleInput_ShouldMatchComputeHash()
		{
			byte[] input = CryptoTestUtilities.SimpleTextAsciiBytes;

			using var algorithm = this.CreateAlgorithm();

			algorithm.TransformBlock(input, 0, input.Length - 1, null, 0);
			algorithm.TransformFinalBlock(input, input.Length - 1, 1);

			byte[] transformResult = algorithm.Hash!;
			byte[] computeHashResult = algorithm.ComputeHash(input);

			CollectionAssert.AreEqual(computeHashResult, transformResult, "Hash results should be identical between block-based and full ComputeHash execution.");
		}

		/// <summary>
		/// Verifies that hashing an empty input via <see cref="HashAlgorithm.TransformBlock" /> and/or
		/// <see cref="HashAlgorithm.TransformFinalBlock(byte[], int, int)" /> produces the correct known hash result for an empty input.
		/// </summary>
		[TestMethod]
		public void TransformBlockAndFinalBlock_WhenInputIsEmpty_ShouldProduceExpectedHash()
		{
			using var algorithm = this.CreateAlgorithm();
			byte[] expected = Convert.FromHexString(this.ExpectedHash_ForEmptyByteArray);

			// Case 1: TransformBlock followed by TransformFinalBlock
			algorithm.TransformBlock(CryptoTestUtilities.EmptyByteArray, 0, 0, null, 0);
			algorithm.TransformFinalBlock(CryptoTestUtilities.EmptyByteArray, 0, 0);
			CollectionAssert.AreEqual(expected, algorithm.Hash, "TransformBlock followed by TransformFinalBlock on empty input should match expected hash.");

			// Case 2: TransformFinalBlock alone
			algorithm.TransformFinalBlock(CryptoTestUtilities.EmptyByteArray, 0, 0);
			CollectionAssert.AreEqual(expected, algorithm.Hash, "TransformFinalBlock alone on empty input should match expected hash.");
		}

		/// <summary>
		/// Verifies that splitting the input between <see cref="HashAlgorithm.TransformBlock" /> and
		/// <see cref="HashAlgorithm.TransformFinalBlock(byte[], int, int)" /> still produces the correct and stable hash result.
		/// </summary>
		[TestMethod]
		public void TransformBlockAndFinalBlock_WhenInputIsSplitAcrossBlocks_ShouldProduceExpectedHash()
		{
			using var algorithm = this.CreateAlgorithm();

			// Ensure input size is at least twice the input block size for testing.
			int blockSize = algorithm.InputBlockSize;
			byte[] input = new byte[Math.Max(blockSize * 2, 8)];
			CryptoUtilities.FillWithRandomNonZeroBytes(input);  // Fill input with random bytes

			byte[] expected = algorithm.ComputeHash(input);
			algorithm.Initialize();

			// If HandlePartialBlocks is true, simulate processing partial blocks
			if (this.HandlePartialBlocks)
			{
				// Split the input based on the actual block size
				byte[] firstBlock = input.Take(blockSize).ToArray();
				byte[] secondBlock = input.Skip(blockSize).ToArray();

				algorithm.TransformBlock(firstBlock, 0, firstBlock.Length, null, 0);
				algorithm.TransformFinalBlock(secondBlock, 0, secondBlock.Length);
			}
			else
			{
				// Directly process the blocks without considering partial inputs
				algorithm.TransformFinalBlock(input, 0, input.Length);
			}

			// Verify that the result matches the expected hash
			CollectionAssert.AreEqual(expected, algorithm.Hash, "Split input should produce expected hash result.");

			// Verify subsequent accesses to Hash result in the same hashValue
			byte[] repeatedAccess = algorithm.Hash!;
			CollectionAssert.AreEqual(expected, repeatedAccess, "Multiple accesses to Hash should yield consistent result.");
		}

		/// <summary>
		/// Verifies that calling <see cref="HashAlgorithm.TransformBlock(byte[])" /> with partial input that is not a full block completes
		/// successfully, and produces the correct result independent of previous TransformBlock state.
		/// </summary>
		/// <remarks>
		/// This test ensures that the algorithm can correctly handle partial inputs (which are less than the block size) during streaming
		/// and finalize the hash with the correct result when TransformFinalBlock is called.
		/// </remarks>
		[TestMethod]
		public void TransformBlockAndFinalBlock_WithPartialBlockInputs_ShouldReturnExpectedHash()
		{
			using var algorithm = this.CreateAlgorithm();

			var input = CryptoTestUtilities.ByteSequence0To255;
			var expected = algorithm.ComputeHash(input);

			algorithm.Initialize();

			// If HandlePartialBlocks is true, simulate processing partial blocks
			if (this.HandlePartialBlocks)
			{
				int pos = 0;
				Random rnd = new Random(); // Ensures a good random seed initialization
				int size = Math.Max(algorithm.InputBlockSize - 1, 1);
				int len = input.Length - size;

				// Process the input in random-sized chunks that are not full blocks
				while (pos < len)
				{
					int bytes = rnd.Next(1, size); // Ensures chunk size is smaller than the block size
					algorithm.TransformBlock(input, pos, bytes, input, pos);
					pos += bytes;
				}

				algorithm.TransformFinalBlock(input, pos, input.Length - pos);
			}
			else
			{
				// Directly process the blocks without considering partial inputs
				algorithm.TransformFinalBlock(input, 0, input.Length);
			}

			// Compare the computed hash from the full input to the streamed result
			CollectionAssert.AreEqual(expected, algorithm.Hash);
		}

		/// <summary>
		/// Verifies that calling <see cref="HashAlgorithm.TransformFinalBlock(byte[], int, int)" /> twice reuses the algorithm by resetting
		/// its state, and does not throw.
		/// </summary>
		[TestMethod]
		public void TransformFinalBlock_WhenCalledTwice_ShouldResetAndProduceValidHashes()
		{
			using var algorithm = this.CreateAlgorithm();
			var buffer = CryptoTestUtilities.SimpleTextAsciiBytes;

			// First pass
			algorithm.TransformBlock(buffer, 0, buffer.Length - 1, null, 0);
			algorithm.TransformFinalBlock(buffer, buffer.Length - 1, 1);
			var firstHash = algorithm.Hash;

			// Second pass with different data
			var newBuffer = CryptoTestUtilities.ByteSequence0To255;
			algorithm.TransformBlock(newBuffer, 0, newBuffer.Length - 1, null, 0);
			algorithm.TransformFinalBlock(newBuffer, newBuffer.Length - 1, 1);
			var secondHash = algorithm.Hash;

			Assert.IsNotNull(firstHash);
			Assert.IsNotNull(secondHash);
			CollectionAssert.AreNotEqual(firstHash, secondHash, "Expected two different hashes after resetting via TransformFinalBlock.");
		}

		/// <summary>
		/// Verifies that calling <see cref="HashAlgorithm.TransformFinalBlock(byte[], int, int)" /> twice without calling
		/// <see cref="HashAlgorithm.Initialize" /> behaves according to the .NET version in use. For older versions, it throws
		/// <see cref="CryptographicUnexpectedOperationException" />, whereas in later versions, it does not throw.
		/// </summary>
		[TestMethod]
		public void TransformFinalBlock_WhenCalledTwice_ExpectedBehaviorBasedOnDotNetVersion()
		{
			using var algorithm = this.CreateAlgorithm();
			var buffer = CryptoTestUtilities.SimpleTextAsciiBytes;

			algorithm.TransformBlock(buffer, 0, buffer.Length - 1, null, 0);
			algorithm.TransformFinalBlock(buffer, buffer.Length - 1, 1);

			// Expected behavior differs based on .NET version
#if NETFRAMEWORK || NETCOREAPP3_1

    // For .NET Framework and earlier .NET Core versions (up to 3.1), the second call should throw.
    Assert.ThrowsException<CryptographicUnexpectedOperationException>(
        () => algorithm.TransformFinalBlock(buffer, 0, 0)
    );
#else

			// For .NET 5 and later, subsequent calls to TransformFinalBlock are allowed. In this case, we do not expect an exception.
			algorithm.TransformFinalBlock(buffer, 0, 0);
#endif
		}
	}
}