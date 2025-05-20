﻿using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu.Security.Cryptography
{
	public abstract partial class KeyedHashAlgorithmTests<TTest, TAlgorithm, TVariant>
	{
		/// <summary>
		/// Verifies that the algorithm's key remains unchanged after hashing.
		/// </summary>
		[TestMethod]
		public void ComputeHash_WhenHashing_ShouldPreserveKey()
		{
			byte[] key = this.GenerateUniqueKey();
			byte[] data = new byte[256];

			using var algorithm = this.CreateAlgorithm();
			algorithm.Key = key;

			_ = algorithm.ComputeHash(data);

			// Validate key is unchanged
			Assert.AreEqual(Convert.ToHexString(key), Convert.ToHexString(algorithm.Key), "Key was not preserved after hashing.");
		}

		/// <summary>
		/// Verifies that ComputeHash returns the same result when called multiple times with the same key and input.
		/// </summary>
		[TestMethod]
		public void ComputeHash_WhenSameKeyAndInputUsed_ShouldReturnIdenticalResults()
		{
			byte[] key = this.GenerateUniqueKey();
			byte[] data = new byte[256];

			using var algorithm = this.CreateAlgorithm();
			algorithm.Key = key;

			byte[] hash1 = algorithm.ComputeHash(data);
			byte[] hash2 = algorithm.ComputeHash(data);

			CollectionAssert.AreEqual(hash1, hash2, "Hashes differ when using the same key and input.");
		}

		/// <summary>
		/// Verifies that ComputeHash returns different results when different keys are used with the same input.
		/// </summary>
		[TestMethod]
		public void ComputeHash_WhenDifferentKeysUsed_ShouldReturnDifferentResults()
		{
			byte[] key1 = this.GenerateUniqueKey();
			byte[] key2 = this.GenerateUniqueKey();
			byte[] data = new byte[256];

			byte[] hash1, hash2;

			using var algorithm1 = this.CreateAlgorithm();
			algorithm1.Key = key1;
			hash1 = algorithm1.ComputeHash(data);

			using var algorithm2 = this.CreateAlgorithm();
			algorithm2.Key = key2;
			hash2 = algorithm2.ComputeHash(data);

			CollectionAssert.AreNotEqual(hash1, hash2);
		}
	}
}