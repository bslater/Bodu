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
		/// Verifies that hashing the entire input at once produces the same result as hashing in two parts using <see cref="Crc.ComputeHashFrom" />.
		/// </summary>
		[TestMethod]
		public void ComputeHashFrom_WhenSplitInTwoParts_ShouldMatchComputeHash()
		{
			using var algorithm = this.CreateAlgorithm();

			byte[] input = CryptoTestUtilities.SimpleTextAsciiBytes;
			int splitIndex = input.Length / 2;

			byte[] part1 = input.Take(splitIndex).ToArray();
			byte[] part2 = input.Skip(splitIndex).ToArray();

			// Compute full hash in one go
			byte[] expected = algorithm.ComputeHash(input);

			// Compute hash in two steps
			byte[] intermediate = algorithm.ComputeHash(part1);
			byte[] actual = algorithm.ComputeHashFrom(intermediate, part2);

			CollectionAssert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Verifies that byte-by-byte incremental hashing using <see cref="Crc.ComputeHashFrom" /> produces the same result as hashing the
		/// full input in one go.
		/// </summary>
		[TestMethod]
		public void ComputeHashFrom_WhenUsedByteByByte_ShouldMatchComputeHash()
		{
			using var algorithm = this.CreateAlgorithm();

			byte[] input = CryptoTestUtilities.SimpleTextAsciiBytes;
			byte[] expected = algorithm.ComputeHash(input);

			byte[] state = algorithm.ComputeHash(new byte[] { input[0] });

			for (int i = 1; i < input.Length; i++)
			{
				state = algorithm.ComputeHashFrom(state, new byte[] { input[i] });
			}

			CollectionAssert.AreEqual(expected, state);
		}

		/// <summary>
		/// Verifies that ComputeHashFrom throws when the previous hash length is invalid.
		/// </summary>
		[TestMethod]
		public void ComputeHashFrom_WhenPreviousHashLengthIsInvalid_ShouldThrow()
		{
			using var algorithm = this.CreateAlgorithm();

			byte[] invalidPreviousHash = new byte[2]; // too short
			byte[] newData = CryptoTestUtilities.SimpleTextAsciiBytes;

			Assert.ThrowsException<ArgumentException>(() =>
			{
				_ = algorithm.ComputeHashFrom(invalidPreviousHash, newData);
			});
		}

		/// <summary>
		/// Verifies that ComputeHashFrom using a partial slice of new data produces the correct final hash.
		/// </summary>
		[TestMethod]
		public void ComputeHashFrom_WhenUsingOffsetAndLength_ShouldMatchComputeHash()
		{
			using var algorithm = this.CreateAlgorithm();

			byte[] input = CryptoTestUtilities.SimpleTextAsciiBytes;
			int splitIndex = input.Length / 2;

			byte[] part1 = input.Take(splitIndex).ToArray();
			byte[] part2 = input.Skip(splitIndex).ToArray();

			byte[] expected = algorithm.ComputeHash(input);

			byte[] intermediate = algorithm.ComputeHash(part1);
			byte[] actual = algorithm.ComputeHashFrom(intermediate, part2, 0, part2.Length);

			CollectionAssert.AreEqual(expected, actual);
		}
	}
}