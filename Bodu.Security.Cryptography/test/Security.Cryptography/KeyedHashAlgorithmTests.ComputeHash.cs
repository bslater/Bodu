using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu.Security.Cryptography
{
	public abstract partial class KeyedHashAlgorithmTests<T>
	{
		/// <summary>
		/// Verifies that ComputeHash returns the same result when using the same key and input.
		/// </summary>
		[TestMethod]
		public void ComputeHash_WhenSameKeyUsed_ShouldReturnIdenticalResults()
		{
			byte[] key = this.GenerateUniqueKey();
			byte[] data = new byte[256];

			byte[] hash1, hash2;

			using (T algorithm = this.CreateAlgorithm())
			{
				algorithm.Key = key;
				hash1 = algorithm.ComputeHash(data);
				Assert.AreEqual(Convert.ToHexString(key), Convert.ToHexString(algorithm.Key));
				hash2 = algorithm.ComputeHash(data);
			}

			CollectionAssert.AreEqual(hash1, hash2);
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

			using (T algorithm = this.CreateAlgorithm())
			{
				algorithm.Key = key1;
				hash1 = algorithm.ComputeHash(data);
			}

			using (T algorithm = this.CreateAlgorithm())
			{
				algorithm.Key = key2;
				hash2 = algorithm.ComputeHash(data);
			}

			CollectionAssert.AreNotEqual(hash1, hash2);
		}
	}
}