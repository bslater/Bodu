using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Security.Cryptography
{
	public partial class CrcTests
	{
		/// <summary>
		/// Verifies that TryComputeHashFrom with two-part input produces the same result as ComputeHash with the full input.
		/// </summary>
		[TestMethod]
		public void TryComputeHashFrom_WhenSplitInTwoParts_ShouldMatchComputeHash()
		{
			using var algorithm = this.CreateAlgorithm();

			byte[] input = CryptoTestUtilities.SimpleTextAsciiBytes;
			int splitIndex = input.Length / 2;

			ReadOnlySpan<byte> part1 = input.AsSpan(0, splitIndex);
			ReadOnlySpan<byte> part2 = input.AsSpan(splitIndex);

			// Full hash for comparison
			byte[] expected = algorithm.ComputeHash(input);

			// Feed first part using public API
			algorithm.Initialize();
			algorithm.TransformBlock(part1.ToArray(), 0, part1.Length, null, 0);

			// Finalize part1
			Span<byte> intermediate = stackalloc byte[algorithm.HashSize / 8];
			Assert.IsTrue(algorithm.TryFinalizeHash(intermediate, out _));

			// Resume from part1 + finalize with part2
			Span<byte> result = stackalloc byte[algorithm.HashSize / 8];
			bool success = algorithm.TryComputeHashFrom(intermediate, part2, result, out int written);

			Assert.IsTrue(success);
			Assert.AreEqual(result.Length, written);
			CollectionAssert.AreEqual(expected, result.ToArray());
		}

		/// <summary>
		/// Verifies that TryComputeHashFrom with byte-by-byte updates matches ComputeHash.
		/// </summary>
		[TestMethod]
		public void TryComputeHashFrom_WhenUsedByteByByte_ShouldMatchComputeHash()
		{
			using var algorithm = this.CreateAlgorithm();

			byte[] input = CryptoTestUtilities.SimpleTextAsciiBytes;
			byte[] expected = algorithm.ComputeHash(input);

			// Process the first byte using TransformBlock
			byte[] state = new byte[algorithm.HashSize / 8];
			algorithm.Initialize();
			algorithm.TransformBlock(input, 0, 1, null, 0);
			algorithm.TryFinalizeHash(state, out _);

			// Resume from finalized state, one byte at a time
			for (int i = 1; i < input.Length; i++)
			{
				byte[] next = new byte[algorithm.HashSize / 8];
				bool success = algorithm.TryComputeHashFrom(state, input.AsSpan(i, 1), next, out int written);
				Assert.IsTrue(success);
				Assert.AreEqual(next.Length, written);
				state = next;
			}

			CollectionAssert.AreEqual(expected, state);
		}

		/// <summary>
		/// Verifies that TryComputeHashFrom returns false if the destination span is too small.
		/// </summary>
		[TestMethod]
		public void TryComputeHashFrom_WhenDestinationTooSmall_ShouldReturnFalse()
		{
			using var algorithm = this.CreateAlgorithm();

			byte[] input = Encoding.ASCII.GetBytes("123456");
			byte[] hash = algorithm.ComputeHash(input);

			Span<byte> tooSmall = new byte[algorithm.HashSize / 8 - 1];
			bool result = algorithm.TryComputeHashFrom(hash, Encoding.ASCII.GetBytes("X"), tooSmall, out int written);

			Assert.IsFalse(result);
			Assert.AreEqual(0, written);
		}

		/// <summary>
		/// Verifies that TryComputeHashFrom throws when the previous hash size is invalid.
		/// </summary>
		//[TestMethod]
		//public void TryComputeHashFrom_WhenPreviousHashLengthInvalid_ShouldThrow()
		//{
		//	using var algorithm = this.CreateAlgorithm();

		// Span<byte> invalid = stackalloc byte[1]; // Too short Span<byte> data = stackalloc byte[] { 0x01 }; Span<byte> destination =
		// stackalloc byte[algorithm.HashSize / 8];

		//	Assert.ThrowsException<ArgumentException>(() =>
		//	{
		//		algorithm.TryComputeHashFrom(invalid, data, destination, out _);
		//	});
		//}
	}
}