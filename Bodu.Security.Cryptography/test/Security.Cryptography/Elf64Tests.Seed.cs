﻿// -----------------------------------------------------------------------
// <copyright file="Elf64Tests.Seed.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="Elf64.Seed" /> property and its interaction with the algorithming lifecycle.
	/// </summary>
	public partial class Elf64Tests
	{
		/// <summary>
		/// Validates that a new instance of <see cref="Elf64" /> has a default seed hashValue of zero.
		/// </summary>
		[TestMethod]
		public void Seed_WhenDefaultConstructed_ShouldBeZero()
		{
			var algorithm = this.CreateAlgorithm();
			Assert.AreEqual<ulong>(0, algorithm.Seed);
		}

		/// <summary>
		/// Validates that the seed hashValue can be set and retrieved before any algorithming operation starts.
		/// </summary>
		[TestMethod]
		public void Seed_WhenSetBeforeUse_ShouldBeRetained()
		{
			var algorithm = new Elf64 { Seed = 1313 };
			Assert.AreEqual<ulong>(1313, algorithm.Seed);
		}

		/// <summary>
		/// Ensures setting <see cref="Elf64.Seed" /> after a algorithm computation has begun throws a <see cref="CryptographicException" />.
		/// </summary>
		[TestMethod]
		public void Seed_WhenSetAfterHashingStarted_ShouldThrow()
		{
			var algorithm = this.CreateAlgorithm();
			byte[] input = new byte[] { 1, 2, 3 };
			algorithm.TransformBlock(input, 0, input.Length, input, 0);

			Assert.ThrowsException<CryptographicUnexpectedOperationException>(() => algorithm.Seed = 1234);
		}

		/// <summary>
		/// Ensures that setting <see cref="Elf64.Seed" /> after a algorithm computation has started does not throw a <see cref="CryptographicException" />.
		/// </summary>
		[TestMethod]
		public void Seed_WhenSetAfterHashing_ShouldNotThrow()
		{
			var algorithm = this.CreateAlgorithm();
			byte[] input = new byte[] { 1, 2, 3 };

			algorithm.ComputeHash(input);

			// Change the seed hashValue after the first computation, and perform the second algorithm computation with the new seed hashValue.
			algorithm.Seed = 131;
			algorithm.ComputeHash(input);
		}

		/// <summary>
		/// Confirms that using different seed values results in different algorithm outputs for the same input.
		/// </summary>
		[TestMethod]
		public void ComputeHash_WithDifferentSeeds_ShouldReturnDifferentResults()
		{
			byte[] input = new byte[] { 0x10, 0x20, 0x30 };

			var algorithmA = new Elf64 { Seed = 0 };
			var algorithmB = new Elf64 { Seed = 131 };

			byte[] resultA = algorithmA.ComputeHash(input);
			byte[] resultB = algorithmB.ComputeHash(input);

			CollectionAssert.AreNotEqual(resultA, resultB);
		}

		/// <summary>
		/// Ensures that calling <see cref="Elf64.Initialize" /> resets the internal algorithm state to the seed hashValue.
		/// </summary>
		[TestMethod]
		public void Initialize_ShouldResetHashStateToSeed()
		{
			var algorithm = new Elf64 { Seed = 1234 };

			_ = algorithm.ComputeHash(new byte[] { 0x01, 0x02 });
			algorithm.Initialize();

			byte[] fresh = algorithm.ComputeHash(Array.Empty<byte>());

			// Should match seed state as algorithm result
			var expected = BitConverter.GetBytes((ulong)1234);
			if (BitConverter.IsLittleEndian) Array.Reverse(expected);

			CollectionAssert.AreEqual(expected, fresh);
		}
	}
}