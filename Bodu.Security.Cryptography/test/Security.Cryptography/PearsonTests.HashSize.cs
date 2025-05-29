﻿using Bodu.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Security.Cryptography
{
	public partial class PearsonTests
		: Security.Cryptography.HashAlgorithmTests<PearsonTests, Pearson, SingleTestVariant>
	{
		[TestMethod]
		public void HashSize_Get_WhenDefault_ShouldReturn8()
		{
			using var hash = new Pearson();
			Assert.AreEqual(8, hash.HashSize, "Default hash size should be 8 bits.");
		}

		[DataTestMethod]
		[DataRow(8)]
		[DataRow(64)]
		[DataRow(128)]
		[DataRow(512)]
		[DataRow(2048)]
		public void HashSize_Set_WhenValid_ShouldUpdateSize(int bits)
		{
			using var hash = new Pearson
			{
				HashSize = bits
			};

			Assert.AreEqual(bits, hash.HashSize, $"HashSize should be set to {bits} bits.");
		}

		[DataTestMethod]
		[DataRow(8)]
		[DataRow(64)]
		[DataRow(128)]
		[DataRow(512)]
		[DataRow(2048)]
		public void ComputeHash_WhenHashSizeSet_ShouldReturnExpectedByteLength(int bits)
		{
			using var hash = new Pearson
			{
				HashSize = bits
			};

			byte[] input = Encoding.ASCII.GetBytes("abc");

			byte[] result = hash.ComputeHash(input);

			int expectedLength = bits / 8;
			Assert.AreEqual(expectedLength, result.Length, $"Expected hash length for {bits} bits is {expectedLength} bytes.");
		}

		[DataTestMethod]
		[DataRow(0)]
		[DataRow(7)]
		[DataRow(9)]
		[DataRow(2056)]
		[DataRow(-8)]
		public void HashSize_Set_WhenOutOfRange_ShouldThrow(int bits)
		{
			using var hash = new Pearson();
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				hash.HashSize = bits;
			});
		}

		[TestMethod]
		public void HashSize_Set_WhenHashingStarted_ShouldThrowExactly()
		{
			using var hash = new Pearson();
			_ = hash.TransformBlock(new byte[] { 1, 2, 3 }, 0, 3, null, 0);

			Assert.ThrowsExactly<CryptographicUnexpectedOperationException>(() =>
			{
				hash.HashSize = 64;
			});
		}

		[TestMethod]
		public void HashSize_Get_WhenDisposed_ShouldThrowExactly()
		{
			var hash = new Pearson();
			hash.Dispose();

			Assert.ThrowsExactly<ObjectDisposedException>(() =>
			{
				_ = hash.HashSize;
			});
		}

		[TestMethod]
		public void HashSize_Set_WhenDisposed_ShouldThrowExactly()
		{
			var hash = new Pearson();
			hash.Dispose();

			Assert.ThrowsExactly<ObjectDisposedException>(() =>
			{
				hash.HashSize = 64;
			});
		}
	}
}