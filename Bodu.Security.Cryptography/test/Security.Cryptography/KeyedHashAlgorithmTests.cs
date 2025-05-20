using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu.Security.Cryptography
{
	public abstract partial class KeyedHashAlgorithmTests<TTest, TAlgorithm, TVariant>
		: Security.Cryptography.HashAlgorithmTests<TTest, TAlgorithm, TVariant>
		where TTest : HashAlgorithmTests<TTest, TAlgorithm, TVariant>, new()
		where TAlgorithm : KeyedHashAlgorithm, new()
		where TVariant : Enum
	{
		/// <summary>
		/// Generates a unique and valid key for use with the current <see cref="KeyedHashAlgorithm" /> implementation.
		/// </summary>
		/// <returns>A non-null, non-empty <see cref="byte" /> array representing a valid cryptographic key.</returns>
		/// <remarks>
		/// This method is used by test cases to verify hash consistency and key-dependent behavior across multiple executions.
		/// Implementations should ensure that each invocation returns a key appropriate for the tested algorithm.
		/// </remarks>
		protected abstract byte[] GenerateUniqueKey();

		/// <summary>
		/// Gets the list of valid key lengths supported by the algorithm. Concrete test classes must override this to provide supported sizes.
		/// </summary>
		protected abstract IReadOnlyList<int> ValidKeyLengths { get; }

		protected static void AssertValidKeyLength(TAlgorithm algorithm, int length)
		{
			byte[] key = Enumerable.Repeat((byte)0xAA, length).ToArray();
			algorithm.Key = key;

			Assert.AreEqual(length, algorithm.Key.Length, $"Key of length {length} was not accepted as expected.");
		}
	}
}