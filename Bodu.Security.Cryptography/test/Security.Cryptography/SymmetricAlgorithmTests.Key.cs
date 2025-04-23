﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Security.Cryptography
{
	public abstract partial class SymmetricAlgorithmTests<T>
	{
		/// <summary>
		/// Validates that the Key property is not null upon algorithm creation.
		/// </summary>
		[TestMethod]
		public void Key_WhenAccessed_ShouldNotBeNull()
		{
			using T algorithm = this.CreateAlgorithm();
			Assert.IsNotNull(algorithm.Key);
		}

		/// <summary>
		/// Validates that accessing the Key after disposing the algorithm returns a different hashValue.
		/// </summary>
		[TestMethod]
		public void Key_WhenAccessedAfterDispose_ShouldReturnDifferentValue()
		{
			T algorithm = this.CreateAlgorithm();
			byte[] ivBeforeDispose = algorithm.Key;
			algorithm.Dispose();
			byte[] ivAfterDispose = algorithm.Key;
			CollectionAssert.AreNotEqual(ivBeforeDispose, ivAfterDispose);
		}

		/// <summary>
		/// Validates that setting the Key to null throws an ArgumentNullException.
		/// </summary>
		[TestMethod]
		public void Key_WhenSetToNull_ShouldThrowArgumentNullException()
		{
			using T algorithm = this.CreateAlgorithm();
			Assert.ThrowsException<ArgumentNullException>(() => algorithm.Key = null);
		}

		/// <summary>
		/// Validates that setting an invalid Key size throws a CryptographicException.
		/// </summary>
		[TestMethod]
		public void Key_WhenSetToInvalidSize_ShouldThrowCryptographicException()
		{
			using T algorithm = this.CreateAlgorithm();
			byte[] invalidKey = new byte[algorithm.BlockSize - 1];
			Assert.ThrowsException<CryptographicException>(() => algorithm.Key = invalidKey);
		}

		/// <summary>
		/// Verifies that setting Key returns the same hashValue on subsequent get.
		/// </summary>
		[TestMethod]
		public void Key_WhenSet_ShouldReturnSameValueOnGet()
		{
			using T algorithm = this.CreateAlgorithm();
			byte[] iv = new byte[algorithm.BlockSize / 8];
			CryptoUtilities.FillWithRandomNonZeroBytes(iv);

			algorithm.Key = iv;
			CollectionAssert.AreEqual(iv, algorithm.Key);
		}

		/// <summary>
		/// Verifies that the Key property returns a defensive copy (not the same reference).
		/// </summary>
		[TestMethod]
		public void Key_WhenSet_ShouldReturnDefensiveCopy()
		{
			using T algorithm = this.CreateAlgorithm();
			byte[] iv = new byte[algorithm.BlockSize / 8];
			CryptoUtilities.FillWithRandomNonZeroBytes(iv);

			algorithm.Key = iv;
			Assert.AreNotSame(iv, algorithm.Key);
		}

		/// <summary>
		/// Verifies that modifying a retrieved Key does not affect the internal state.
		/// </summary>
		[TestMethod]
		public void Key_WhenModifiedAfterGet_ShouldNotAffectInternalState()
		{
			using T algorithm = this.CreateAlgorithm();
			byte[] iv = new byte[algorithm.BlockSize / 8];
			CryptoUtilities.FillWithRandomNonZeroBytes(iv);

			algorithm.Key = iv;
			byte[] ivCopy = algorithm.Key;
			ivCopy[0]++; // mutate

			CollectionAssert.AreNotEqual(ivCopy, algorithm.Key);
		}

		/// <summary>
		/// Verifies that GenerateKey produces a different Key from the previous one.
		/// </summary>
		[TestMethod]
		public void GenerateKey_WhenCalled_ShouldChangeKey()
		{
			using T algorithm = this.CreateAlgorithm();
			byte[] initialKey = algorithm.Key;

			algorithm.GenerateKey();
			CollectionAssert.AreNotEqual(initialKey, algorithm.Key);
		}
	}
}