using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu.Security.Cryptography
{
	public abstract partial class KeyedHashAlgorithmTests<T>
	{
		/// <summary>
		/// Verifies that setting a null key throws an <see cref="ArgumentNullException" />.
		/// </summary>
		[TestMethod]
		public void Key_WhenSetToNull_ShouldThrowArgumentNullException()
		{
			using var algorithm = this.CreateAlgorithm();

			Assert.ThrowsException<ArgumentNullException>(() =>
			{
				algorithm.Key = null!;
			});
		}

		/// <summary>
		/// Verifies that setting an invalid key throws a <see cref="CryptographicException" />.
		/// </summary>
		[TestMethod]
		public void Key_WhenSetToInvalidKey_ShouldThrowCryptographicException()
		{
			using var algorithm = this.CreateAlgorithm();

			Assert.ThrowsException<CryptographicException>(() =>
			{
				algorithm.Key = new byte[0];
			});
		}

		/// <summary>
		/// Verifies the correct get/set behavior of the <see cref="KeyedHashAlgorithm.Key" /> property.
		/// </summary>
		[TestMethod]
		public void Key_WhenSetAndRetrieved_ShouldBehaveAsExpected()
		{
			using var algorithm = this.CreateAlgorithm();
			Assert.IsNotNull(algorithm.Key);

			byte[] key = Enumerable.Range(0, algorithm.Key.Length).Select(i => (byte)i).ToArray();
			algorithm.Key = key;
			CollectionAssert.AreEqual(key, algorithm.Key);
			Assert.AreNotSame(key, algorithm.Key);

			byte[] copy = algorithm.Key;
			CollectionAssert.AreEqual(copy, key);
			CollectionAssert.AreEqual(copy, algorithm.Key);
			Assert.AreNotSame(copy, key);
			Assert.AreNotSame(copy, algorithm.Key);

			copy[0]++;
			CollectionAssert.AreNotEqual(copy, key);
			CollectionAssert.AreNotEqual(copy, algorithm.Key);
		}

		/// <summary>
		/// Verifies that mutating the original key array after assignment does not affect the internal key stored by the algorithm.
		/// </summary>
		[TestMethod]
		public void Key_WhenOriginalKeyIsMutated_ShouldNotAffectInternalKey()
		{
			using var algorithm = this.CreateAlgorithm();
			byte[] key = Enumerable.Range(0, 16).Select(i => (byte)i).ToArray();
			algorithm.Key = key;

			// Mutate original key
			key[0] ^= 0xFF;

			// Assert internal key is unchanged
			Assert.AreNotEqual(key[0], algorithm.Key[0]);
		}

		/// <summary>
		/// Verifies that accessing the <see cref="KeyedHashAlgorithm.Key" /> property multiple times returns distinct array instances.
		/// </summary>
		[TestMethod]
		public void Key_WhenAccessedMultipleTimes_ShouldReturnDistinctInstances()
		{
			using var algorithm = this.CreateAlgorithm();
			var first = algorithm.Key;
			var second = algorithm.Key;

			Assert.AreEqual(first.Length, second.Length);
			Assert.AreEqual(Convert.ToHexString(first), Convert.ToHexString(second));
			Assert.AreNotSame(first, second);
		}

		/// <summary>
		/// Verifies that updating the key after a hash operation does not retroactively affect the result of the previous hash.
		/// </summary>
		[TestMethod]
		public void Key_WhenChangedAfterHash_ShouldNotAffectPreviousResult()
		{
			byte[] data = new byte[128];
			byte[] key1 = this.GenerateUniqueKey();
			byte[] key2 = this.GenerateUniqueKey();

			byte[] hash1, hash2;

			using var algorithm = this.CreateAlgorithm();
			algorithm.Key = key1;
			hash1 = algorithm.ComputeHash(data);

			algorithm.Key = key2;
			hash2 = algorithm.ComputeHash(data);

			Assert.AreNotEqual(Convert.ToHexString(hash1), Convert.ToHexString(hash2));
		}

		/// <summary>
		/// Verifies that reassigning the same key hashValue multiple times does not throw or alter behavior.
		/// </summary>
		[TestMethod]
		public void Key_WhenSetToSameValueMultipleTimes_ShouldNotThrow()
		{
			using var algorithm = this.CreateAlgorithm();
			byte[] key = this.GenerateUniqueKey();

			algorithm.Key = key;
			algorithm.Key = key; // Reassign
			Assert.IsNotNull(algorithm.Key);
		}

		/// <summary>
		/// Verifies that calling <see cref="HashAlgorithm.Initialize" /> does not reset the key unless explicitly cleared.
		/// </summary>
		[TestMethod]
		public void Key_WhenInitializeCalled_ShouldNotResetKey()
		{
			using var algorithm = this.CreateAlgorithm();
			byte[] originalKey = algorithm.Key;
			algorithm.Initialize();

			CollectionAssert.AreEqual(originalKey, algorithm.Key);
		}

		/// <summary>
		/// Verifies that using the same key with different inputs produces different hashes.
		/// </summary>
		[TestMethod]
		public void Key_WhenCallingComputeHashUsingSameKeyAndDifferentInputs_ShouldReturnDifferentHashes()
		{
			using var algorithm = this.CreateAlgorithm();

			byte[] key = this.GenerateUniqueKey();
			algorithm.Key = key;

			byte[] input1 = (byte[])CryptoTestUtilities.ByteSequence0To255.Clone();
			byte[] input2 = (byte[])CryptoTestUtilities.ByteSequence0To255.Clone();
			input2[0]++;

			byte[] hash1 = algorithm.ComputeHash(input1);
			byte[] hash2 = algorithm.ComputeHash(input2);

			Assert.AreNotEqual(Convert.ToHexString(hash1), Convert.ToHexString(hash2));
		}

		/// <summary>
		/// Verifies that modifying the key after a hash operation does not influence the algorithm internals.
		/// </summary>
		[TestMethod]
		public void Key_WhenModifiedAfterHashing_ShouldNotAffectInternalState()
		{
			using var algorithm = this.CreateAlgorithm();
			byte[] key = this.GenerateUniqueKey();
			algorithm.Key = key;

			byte[] data = Enumerable.Repeat((byte)0x42, 64).ToArray();
			byte[] hash1 = algorithm.ComputeHash(data);

			key[0] ^= 0xFF;
			byte[] hash2 = algorithm.ComputeHash(data);

			CollectionAssert.AreEqual(hash1, hash2);
		}

		/// <summary>
		/// Verifies that modifying the input array after setting it as a key does not mutate the internal copy.
		/// </summary>
		[TestMethod]
		public void Key_WhenInputArrayModified_ShouldNotMutateInternalKey()
		{
			using var algorithm = this.CreateAlgorithm();
			byte[] key = this.GenerateUniqueKey();
			algorithm.Key = key;

			byte[] snapshot = algorithm.Key.ToArray();

			key[0] ^= 0xFF;

			CollectionAssert.AreEqual(snapshot, algorithm.Key);
		}

		/// <summary>
		/// Gets the minimum legal key length in bytes for the algorithm. Returns null if not defined.
		/// </summary>
		protected abstract int MinimumLegalKeyLength { get; }

		/// <summary>
		/// Gets the maximum legal key length in bytes for the algorithm. Returns null if not defined.
		/// </summary>
		protected abstract int MaximumLegalKeyLength { get; }

		/// <summary>
		/// Verifies that assigning a key larger than the maximum legal length throws a CryptographicException.
		/// </summary>
		[TestMethod]
		public void Key_WhenAboveMaximumLength_ShouldThrow()
		{
			using var algorithm = this.CreateAlgorithm();
			byte[] tooLong = new byte[MaximumLegalKeyLength + 1];
			Assert.ThrowsException<CryptographicException>(() =>
			{
				algorithm.Key = tooLong;
			});
		}

		[TestMethod]
		public void Key_WhenBelowMinimumLength_ShouldThrow()
		{
			using var algorithm = this.CreateAlgorithm();
			byte[] tooShort = new byte[MinimumLegalKeyLength - 1];

			Assert.ThrowsException<CryptographicException>(() =>
			{
				algorithm.Key = tooShort;
			});
		}
	}
}