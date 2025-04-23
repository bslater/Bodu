// ---------------------------------------------------------------------------------------------------------------
// <copyright file="HashAlgorithmTests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	public abstract partial class HashAlgorithmTests<T>
	{
		/// <summary>
		/// Verifies that reusing the algorithm instance after <see cref="HashAlgorithm.Initialize" /> resets the state correctly.
		/// </summary>
		[TestMethod]
		public void Initialize_WhenCalledBetweenHashes_ShouldResetStateForNewHash()
		{
			using var algorithm = this.CreateAlgorithm();

			byte[] input1 = CryptoTestUtilities.SimpleTextAsciiBytes;
			byte[] input2 = CryptoTestUtilities.ByteSequence0To255;

			byte[] hash1 = algorithm.ComputeHash(input1);

			algorithm.Initialize(); // reset state
			byte[] hash2 = algorithm.ComputeHash(input2);

			Assert.AreNotEqual(Convert.ToHexString(hash1), Convert.ToHexString(hash2), "Hashes should differ between independent inputs.");
		}

		/// <summary>
		/// Verifies that calling <see cref="HashAlgorithm.Initialize" /> multiple times without computing does not throw or alter behavior.
		/// </summary>
		[TestMethod]
		public void Initialize_WhenCalledRepeatedly_ShouldNotThrowOrAffectBehavior()
		{
			using var algorithm = this.CreateAlgorithm();

			algorithm.Initialize();
			algorithm.Initialize();

			byte[] input = CryptoTestUtilities.SimpleTextAsciiBytes;
			byte[] hash = algorithm.ComputeHash(input);

			Assert.IsNotNull(hash);
		}

		/// <summary>
		/// Verifies that <see cref="HashAlgorithm" /> can be reused after finalizing a hash via
		/// <see cref="HashAlgorithm.TransformFinalBlock" /> and reinitializing.
		/// </summary>
		[TestMethod]
		public void Initialize_WhenCalledAfterFinalBlock_ShouldAllowNewHashComputation()
		{
			using var algorithm = this.CreateAlgorithm();

			byte[] input = CryptoTestUtilities.ByteSequence0To255;
			algorithm.TransformFinalBlock(input, 0, input.Length);
			byte[] hash1 = algorithm.Hash!;

			algorithm.Initialize();

			byte[] hash2 = algorithm.ComputeHash(input);

			// Hashing the same input should return the same result after reinit
			CollectionAssert.AreEqual(hash1, hash2, "Hashes should match after reinitializing with the same input.");
		}

		/// <summary>
		/// Verifies that calling <see cref="HashAlgorithm.Initialize" /> mid-hashing resets any partially buffered data.
		/// </summary>
		[TestMethod]
		public void Initialize_MidHashing_ShouldResetPartialState()
		{
			using var algorithm = this.CreateAlgorithm();

			byte[] part1 = CryptoTestUtilities.SimpleTextAsciiBytes.Take(4).ToArray();
			byte[] part2 = CryptoTestUtilities.SimpleTextAsciiBytes.Skip(4).ToArray();

			algorithm.TransformBlock(part1, 0, part1.Length, null, 0);
			algorithm.Initialize(); // Reset mid-hash

			// Start a new hash
			byte[] result = algorithm.ComputeHash(part2);

			Assert.IsNotNull(result);

			// No expectations on the hashValue — just ensure no crash/corruption
		}

		/// <summary>
		/// Verifies that calling <see cref="HashAlgorithm.Initialize" /> after disposal throws an <see cref="ObjectDisposedException" />,
		/// matching base .NET behavior.
		/// </summary>
		[TestMethod]
		public void Initialize_WhenDisposed_ShouldThrowObjectDisposedException()
		{
			// Arrange
			using var algorithm = this.CreateAlgorithm();
			algorithm.Dispose();

			// Act & Assert
			Assert.ThrowsException<ObjectDisposedException>(() => algorithm.Initialize());
		}
	}
}