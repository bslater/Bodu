﻿using Bodu.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Security.Cryptography
{
	public partial class TigerTests
	{
		/// <summary>
		/// Verifies that setting <see cref="Pearson.Variant" /> to a valid predefined type allows computing a algorithm without error, and
		/// that the algorithm output is the correct length for the current <see cref="HashAlgorithm.HashSize" />.
		/// </summary>
		/// <param name="variant">The table type to test.</param>
		[DataTestMethod]
		[DynamicData(nameof(HashAlgorithmVariants), DynamicDataSourceType.Method)]
		public void Variant_Set_WhenValid_ShouldProduceExpectedHash(TigerVariant variant)
		{
			using var algorithm = this.CreateAlgorithm(variant);

			byte[] input = Encoding.ASCII.GetBytes("Test input");

			byte[] result = algorithm.ComputeHash(input);

			Assert.IsNotNull(result, $"Result should not be null for table variant {variant}.");
			Assert.AreEqual(algorithm.HashSize / 8, result.Length, $"Hash length should match expected size for variant {variant}.");
			Assert.IsTrue(result.Any(b => b != 0), $"Hash result should not be all zeros for variant {variant}.");
		}

		[TestMethod]
		public void Variant_Get_WhenDefault_ShouldReturnPearson()
		{
			using var algorithm = this.CreateAlgorithm();

			var type = algorithm.Variant;

			Assert.AreEqual(Bodu.Security.Cryptography.TigerHashingVariant.Tiger, type, "Default Variant should be Tiger.");
		}

		[TestMethod]
		public void Variant_Set_WhenHashingStarted_ShouldThrowExactly()
		{
			using var algorithm = this.CreateAlgorithm();
			algorithm.TransformBlock(new byte[] { 1, 2, 3 }, 0, 3, null, 0);

			Assert.ThrowsExactly<CryptographicUnexpectedOperationException>(() =>
			{
				algorithm.Variant = Bodu.Security.Cryptography.TigerHashingVariant.Tiger2;
			});
		}

		[TestMethod]
		public void Variant_Set_WhenDisposed_ShouldThrowExactly()
		{
			using var algorithm = this.CreateAlgorithm();
			algorithm.Dispose();

			Assert.ThrowsExactly<ObjectDisposedException>(() =>
			{
				algorithm.Variant = Bodu.Security.Cryptography.TigerHashingVariant.Tiger2;
			});
		}

		[TestMethod]
		public void Variant_Get_WhenDisposed_ShouldThrowExactly()
		{
			using var algorithm = this.CreateAlgorithm();
			algorithm.Dispose();

			Assert.ThrowsExactly<ObjectDisposedException>(() =>
			{
				var _ = algorithm.Variant;
			});
		}
	}
}