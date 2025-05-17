using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	public abstract partial class HashAlgorithmTests<T>
	{
		/// <summary>
		/// Verifies that repeated calls to <see cref="HashAlgorithm.ComputeHash(byte[])" /> with the same input produce the same result.
		/// </summary>
		[TestMethod]
		public void ComputeHash_ShouldBeDeterministic()
		{
			byte[] input = Enumerable.Range(0, 128).Select(i => (byte)(i % 256)).ToArray();
			using var algorithm = this.CreateAlgorithm();
			byte[] hash1 = algorithm.ComputeHash(input);
			byte[] hash2 = algorithm.ComputeHash(input);
			CollectionAssert.AreEqual(hash1, hash2);
		}

		/// <summary>
		/// Verifies that <see cref="HashAlgorithm.ComputeHash(byte[])" /> produces consistent output for repeated identical input.
		/// </summary>
		[TestMethod]
		public void ComputeHash_WhenReusedWithSameInput_ShouldProduceIdenticalHashResults()
		{
			using var algorithm = this.CreateAlgorithm();
			byte[] input = new byte[1024];
			CryptoUtilities.FillWithRandomNonZeroBytes(input);

			byte[] hashA = algorithm.ComputeHash(input);
			byte[] hashB = algorithm.ComputeHash(input);

			CollectionAssert.AreEqual(hashA, hashB);
		}

		/// <summary>
		/// Verifies that hashing a zero-length region in a buffer matches the empty input hash using <see cref="HashAlgorithm.ComputeHash(byte[], int, int)" />.
		/// </summary>
		[DataTestMethod]
		[DataRow(128, 128, 0)]
		[DataRow(128, 0, 0)]
		public void ComputeHash_ShouldReturnEmptyHash_WhenOffsetAndCountAreZero(int size, int offset, int count)
		{
			byte[] buffer = new byte[size];
			byte[] expected = Convert.FromHexString(this.ExpectedHash_ForEmptyByteArray);
			using var algorithm = this.CreateAlgorithm();

			byte[] actual = algorithm.ComputeHash(buffer, offset, count);
			CollectionAssert.AreEqual(expected, actual);

			buffer[0] = 0xFF;
			buffer[^1] = 0xFF;
			actual = algorithm.ComputeHash(buffer, offset, count);
			CollectionAssert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Verifies that hashing a segment with <see cref="HashAlgorithm.ComputeHash(byte[], int, int)" /> produces the same result as the
		/// full input.
		/// </summary>
		[TestMethod]
		public void ComputeHash_WhenOffsetAndCountReferToSameData_ShouldProduceIdenticalHash()
		{
			byte[] bufferA = new byte[256];
			byte[] bufferB = new byte[257];
			CryptoUtilities.FillWithRandomNonZeroBytes(bufferA);
			Array.Copy(bufferA, 0, bufferB, 1, bufferA.Length);

			using var algorithm = this.CreateAlgorithm();
			byte[] expected = algorithm.ComputeHash(bufferA);
			byte[] actual = algorithm.ComputeHash(bufferB, 1, bufferA.Length);
			CollectionAssert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Verifies that <see cref="HashAlgorithm.ComputeHash(byte[], int, int)" /> throws <see cref="ArgumentException" /> when the cound
		/// or segment is invalid.
		/// </summary>
		[DataTestMethod]
		[DataRow(0, 0, -1)]
		[DataRow(0, 1, 0)]
		[DataRow(3, 0, 4)]
		[DataRow(3, 1, 3)]
		[DataRow(3, 3, 1)]
		public void ComputeHash_WhenOffsetAndCountCombinationIsInvalid_ShouldThrowArgumentException(int size, int offset, int count)
		{
			byte[] buffer = new byte[size];
			using var algorithm = this.CreateAlgorithm();

			Assert.ThrowsException<ArgumentException>(() =>
			{
				algorithm.ComputeHash(buffer, offset, count);
			});
		}

		/// <summary>
		/// Verifies that <see cref="HashAlgorithm.ComputeHash(byte[])" /> throws <see cref="ArgumentNullException" /> when passed a null buffer.
		/// </summary>
		[TestMethod]
		public void ComputeHash_WithNullBuffer_ShouldThrowArgumentNullException()
		{
			byte[] buffer = null!;
			using var algorithm = this.CreateAlgorithm();

			Assert.ThrowsException<ArgumentNullException>(() =>
			{
				algorithm.ComputeHash(buffer);
			});
		}

		/// <summary>
		/// Verifies that <see cref="HashAlgorithm.ComputeHash(byte[], int, int)" /> throws <see cref="ArgumentNullException" /> when buffer
		/// is null.
		/// </summary>
		[TestMethod]
		public void ComputeHash_WithNullBufferAndRange_ShouldThrowArgumentNullException()
		{
			byte[] buffer = null!;
			using var algorithm = this.CreateAlgorithm();

			Assert.ThrowsException<ArgumentNullException>(() =>
			{
				algorithm.ComputeHash(buffer, 0, 0);
			});
		}

		/// <summary>
		/// Verifies that <see cref="HashAlgorithm.ComputeHash(Stream)" /> throws <see cref="ArgumentNullException" /> when passed a null stream.
		/// </summary>
		[TestMethod]
		public void ComputeHash_WithNullStream_ShouldThrowExactly()
		{
			using var algorithm = this.CreateAlgorithm();

			// Expected behavior differs based on .NET version
#if NETFRAMEWORK || NETCOREAPP3_1

			// For .NET Framework and earlier .NET Core versions (up to 3.1), it throws ArgumentNullException.
			Assert.ThrowsException<ArgumentNullException>(				() => {
			algorithm.ComputeHash((Stream)null!);
			});
#else

			// For .NET 5 and later, it throws NullReferenceException instead.
			Assert.ThrowsExactly<NullReferenceException>(() =>
			{
				algorithm.ComputeHash((Stream)null!);
			});
#endif
		}

		/// <summary>
		/// Verifies that <see cref="HashAlgorithm.ComputeHash(byte[], int, int)" /> throws <see cref="ArgumentOutOfRangeException" /> when
		/// offset is negative.
		/// </summary>
		[TestMethod]
		public void ComputeHash_WhenOffsetIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			byte[] buffer = new byte[0];
			using var algorithm = this.CreateAlgorithm();

			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			{
				algorithm.ComputeHash(buffer, -1, 0);
			});
		}

		/// <summary>
		/// Verifies that calling <see cref="HashAlgorithm.ComputeHash(byte[])" /> after streaming begins completes successfully, and does
		/// not depend on previous TransformBlock state.
		/// </summary>
		[TestMethod]
		public void ComputeHash_AfterTransformBlock_ShouldIgnorePriorStreaming()
		{
			using var algorithm = this.CreateAlgorithm();

			// Begin a partial stream operation
			algorithm.TransformBlock(CryptoTestUtilities.ByteSequence0To255, 0, 128, null, 0);

			// Call ComputeHash independently
			var hash = algorithm.ComputeHash(CryptoTestUtilities.ByteSequence0To255);

			Assert.IsNotNull(hash);
			Assert.AreEqual(algorithm.HashSize / 8, hash.Length);
		}
	}
}