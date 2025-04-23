// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ComputeHashAsync.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Security.Cryptography;
using Bodu.Infrastructure;

namespace Bodu.Security.Cryptography
{
	public abstract partial class HashAlgorithmTests<T>
	{
		/// <summary>
		/// Asserts that hashing a stream using <see cref="HashAlgorithm.ComputeHashAsync(Stream)" /> returns the expected hex result.
		/// </summary>
		/// <param name="variant">The test variant with algorithm factory and expected hash.</param>
		/// <param name="input">The stream content to hash.</param>
		/// <param name="expectedHex">The expected hash result in hex.</param>
		/// <returns>A task representing the asynchronous test execution.</returns>
		protected async Task AssertStreamHashMatchesAsync(HashAlgorithmVariant variant, byte[] input, string expectedHex)
		{
			await using var stream = new MemoryStream(input);
			using var algorithm = variant.Factory.Invoke();

			byte[] result = await algorithm.ComputeHashAsync(stream);
			string actual = Convert.ToHexString(result);

			Assert.AreEqual(expectedHex, actual, $"Hash mismatch for variant '{variant.Name}'.");
		}

		/// <summary>
		/// Asserts that hashing a stream using <see cref="HashAlgorithm.ComputeHashAsync(Stream)" /> returns the expected hex result.
		/// </summary>
		/// <param name="variant">The test variant with algorithm factory and expected hash.</param>
		/// <param name="input">The stream content to hash.</param>
		/// <param name="expectedHex">The expected hash result in hex.</param>
		/// <returns>A task representing the asynchronous test execution.</returns>
		protected async Task AssertHashMatchesInputPrefixesAsync(HashAlgorithmVariant variant)
		{
			using HashAlgorithm algorithm = variant.Factory.Invoke();
			int vectorCount = algorithm.HashSize / 2;
			byte[] input = Enumerable.Range(0, vectorCount).Select(i => (byte)i).ToArray();

			string[] expectedHex = variant.ExpectedHash_ForInputPrefixes as string[]
				?? variant.ExpectedHash_ForInputPrefixes.ToArray();

			Assert.AreEqual(vectorCount, expectedHex.Length, "Unexpected number of expected hash results.");

			for (int i = 0; i < vectorCount; i++)
			{
				// Create a stream using a prefix of the input
				await using var stream = new MemoryStream(input, 0, i + 1, writable: false);

				byte[] result = await algorithm.ComputeHashAsync(stream);
				string actual = Convert.ToHexString(result);

				Assert.AreEqual(expectedHex[i], actual, $"Hash mismatch for variant '{variant.Name}' at prefix length {i + 1}.");
			}
		}

		/// <summary>
		/// Verifies that repeated calls with the same stream data produce identical results.
		/// </summary>
		[TestMethod]
		public async Task ComputeHashAsync_WhenCalledTwiceWithSameInput_ShouldReturnIdenticalHashes()
		{
			byte[] input = Enumerable.Range(0, 128).Select(i => (byte)(i % 256)).ToArray();

			using var algorithm = this.CreateAlgorithm();
			using var stream1 = new MemoryStream(input);
			using var stream2 = new MemoryStream(input);

			byte[] hash1 = await algorithm.ComputeHashAsync(stream1);
			byte[] hash2 = await algorithm.ComputeHashAsync(stream2);

			CollectionAssert.AreEqual(hash1, hash2);
		}

		/// <summary>
		/// Verifies that ComputeHashAsync throws ArgumentNullException when the stream is null.
		/// </summary>
		[TestMethod]
		public async Task ComputeHashAsync_WhenStreamIsNull_ShouldThrowArgumentNullException()
		{
			using var algorithm = this.CreateAlgorithm();
			await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
			{
				await algorithm.ComputeHashAsync(null!);
			});
		}

		/// <summary>
		/// Verifies that ComputeHashAsync throws TaskCanceledException if the cancellation token is already canceled.
		/// </summary>
		[TestMethod]
		public async Task ComputeHashAsync_WhenCancelledTokenIsPassed_ShouldThrowTaskCanceledException()
		{
			using var algorithm = this.CreateAlgorithm();
			using var stream = new MemoryStream(new byte[4096]);
			using var cts = new CancellationTokenSource();
			cts.Cancel();

			await Assert.ThrowsExceptionAsync<TaskCanceledException>(async () =>
			{
				await algorithm.ComputeHashAsync(stream, cts.Token);
			});
		}

		/// <summary>
		/// Verifies that ComputeHashAsync throws ArgumentNullException when passed a null stream directly.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public async Task ComputeHashAsync_ComputeHashAsync_WithNullStream_ShouldThrowArgumentNullException()
		{
			using var algorithm = this.CreateAlgorithm();
			await algorithm.ComputeHashAsync(null!);
		}

		/// <summary>
		/// Verifies that ComputeHashAsync throws ObjectDisposedException when the algorithm is disposed.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ObjectDisposedException))]
		public async Task ComputeHashAsync_ComputeHashAsync_WhenAlgorithmDisposed_ShouldThrowObjectDisposedException()
		{
			var algorithm = this.CreateAlgorithm();
			algorithm.Dispose();

			using var stream = new MemoryStream(new byte[16]);
			await algorithm.ComputeHashAsync(stream);
		}

		/// <summary>
		/// Verifies that ComputeHashAsync throws TaskCanceledException when cancellation token is triggered.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(TaskCanceledException))]
		public async Task ComputeHashAsync_ComputeHashAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
		{
			using var algorithm = this.CreateAlgorithm();
			using var stream = new MemoryStream(new byte[4096]);
			using var cts = new CancellationTokenSource();
			cts.Cancel();

			await algorithm.ComputeHashAsync(stream, cts.Token);
		}

		/// <summary>
		/// Verifies that hashing an empty stream produces the expected result.
		/// </summary>
		[TestMethod]
		public async Task ComputeHashAsync_WhenStreamIsEmpty_ShouldReturnExpectedHash()
		{
			using var algorithm = this.CreateAlgorithm();
			using var stream = new MemoryStream(new byte[0]);

			byte[] result = await algorithm.ComputeHashAsync(stream);
			string actualHex = Convert.ToHexString(result);

			Assert.AreEqual(this.ExpectedHash_ForEmptyByteArray, actualHex);
		}

		/// <summary>
		/// Verifies that <see cref="HashAlgorithm.ComputeHashAsync" /> supports cancellation during slow stream processing.
		/// </summary>
		[TestMethod]
		public async Task ComputeHashAsync_WhenCancelled_ShouldThrowOperationCanceledException()
		{
			using var cancellationSource = new CancellationTokenSource(1000); // cancel after 1 second
			using var stream = new ThrottledIncrementingByteStream(10000); // delayed stream that is 10 seconds
			using var algorithm = this.CreateAlgorithm();

			await Task.WhenAny(
				Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
					algorithm.ComputeHashAsync(stream, cancellationSource.Token)),
				Task.Delay(5000)); // just in case
		}

		/// <summary>
		/// Verifies behavior when <see cref="HashAlgorithmExtensions.ComputeHashAsync" /> is cancelled before the await begins.
		/// </summary>
		[TestMethod]
		public async Task ComputeHashAsync_WhenCancelledBeforeAwait_ShouldThrowTaskCanceledException()
		{
			using var algorithm = this.CreateAlgorithm();
			var stream = new FixedLengthIncrementingStream(int.MaxValue);

			// create a canellation token and cancel it before computation starts
			using var cancellationTokenSource = new CancellationTokenSource();
#if NET6_0_OR_GREATER
			await cancellationTokenSource.CancelAsync();
#else
			cancellationTokenSource.Cancel();
#endif
			var task = algorithm.ComputeHashAsync(stream, cancellationTokenSource.Token);

			await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => task);

			Assert.IsTrue(task.IsCanceled, "Task should be marked as canceled.");
			Assert.AreEqual(0, stream.Position, "Stream position should not have advanced.");
			Assert.IsNull(algorithm.Hash, "Hash hashValue should be null.");
		}

		/// <summary>
		/// Verifies that computing a hash over a long input stream completes successfully.
		/// </summary>
		[TestMethod]
		public async Task ComputeHashAsync_WhenStreamIsLong_ShouldProduceResult()
		{
			const int length = 10 * 1024 * 1024; // 10 MB
			using var stream = new FixedLengthIncrementingStream(length);
			using var algorithm = this.CreateAlgorithm();

			byte[] result = await algorithm.ComputeHashAsync(stream);

			Assert.IsNotNull(result, "Hash result should not be null.");
			Assert.AreEqual(algorithm.HashSize / 8, result.Length, "Result should match expected hash length.");
		}

		/// <summary>
		/// Verifies that computing the hash on the same long input produces the same result each time.
		/// </summary>
		[TestMethod]
		public async Task ComputeHashAsync_WhenRepeatedOnSameInput_ShouldProduceSameHash()
		{
			const int length = 1 * 1024 * 1024; // 1 MB
			using var streamA = new FixedLengthIncrementingStream(length);
			using var streamB = new FixedLengthIncrementingStream(length);
			using var algorithmA = this.CreateAlgorithm();
			using var algorithmB = this.CreateAlgorithm();

			byte[] hashA = await algorithmA.ComputeHashAsync(streamA);
			byte[] hashB = await algorithmB.ComputeHashAsync(streamB);

			CollectionAssert.AreEqual(hashA, hashB, "Hashes from identical input should match.");
		}

		/// <summary>
		/// Verifies that all bytes in a long stream are processed when computing the hash. This is only applicable to algorithms that
		/// expose the <c>BytesProcessed</c> property.
		/// </summary>
		[TestMethod]
		public async Task ComputeHashAsync_WhenStreamIsLong_ShouldProcessAllBytes()
		{
			const int length = 1 * 1024 * 1024; // 1 MB
			using var stream = new FixedLengthIncrementingStream(length);
			using var algorithm = this.CreateAlgorithm();

			await algorithm.ComputeHashAsync(stream);

			Assert.AreEqual(length, stream.Position, "All bytes should have been processed.");
		}

		/// <summary>
		/// Verifies that computing the hash over various long stream sizes works consistently.
		/// </summary>
		/// <param name="length">The length of the test input stream.</param>
		[DataTestMethod]
		[DataRow(1_000)]
		[DataRow(100_000)]
		[DataRow(1_000_000)]
		[DataRow(10_000_000)]
		public async Task ComputeHashAsync_LongStream_WithVariousLengths_ShouldSucceed(int length)
		{
			using var stream = new FixedLengthIncrementingStream(length);
			using var algorithm = this.CreateAlgorithm();

			byte[] result = await algorithm.ComputeHashAsync(stream);

			Assert.IsNotNull(result);
			Assert.AreEqual(algorithm.HashSize / 8, result.Length);
		}

		/// <summary>
		/// Verifies that <see cref="HashAlgorithm.ComputeHashAsync" /> correctly interacts with a stream by tracking all read operations
		/// using <see cref="MonitoringStream" />. Ensures the total number of bytes read matches the expected length, and that multiple
		/// reads were issued (i.e., streaming behavior).
		/// </summary>
		[TestMethod]
		public async Task ComputeHashAsync_WithMonitoringStream_ShouldTrackAllReads()
		{
			const int length = 2 * 1024 * 1024; // 2 MB

			// Wrap a deterministic input stream with a monitoring stream to capture read behavior
			var baseStream = new FixedLengthIncrementingStream(length);
			var monitoredStream = new MonitoringStream(baseStream);

			// Compute the hash asynchronously
			using var algorithm = this.CreateAlgorithm();
			await algorithm.ComputeHashAsync(monitoredStream);

			// Verify the total number of bytes read matches the expected input length
			Assert.AreEqual(length, monitoredStream.Reads.Sum(r => r.Count), "Total bytes read should match stream length.");

			// Confirm that the algorithm read in multiple chunks (not one large buffer)
			Assert.IsTrue(monitoredStream.Reads.Count > 1, "Expected multiple read operations.");

			// Output captured read info for debugging/analysis
			Trace.WriteLine($"Read Count: {monitoredStream.Reads.Count}");
			Trace.WriteLine($"First Read: {monitoredStream.Reads.FirstOrDefault()}");
			Trace.WriteLine($"Last Read : {monitoredStream.Reads.LastOrDefault()}");
		}
	}
}