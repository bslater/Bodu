using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Security.Cryptography;
using Bodu.Testing.Security;

namespace Bodu.Tests.Security.Cryptography
{
	public abstract class BlockCipherModeTransformTests<TTransform>
		where TTransform : IBlockCipherModeTransform
	{
		protected const int BlockSize = 4;

		[TestMethod]
		public void Transform_WhenInputNotBlockAligned_ShouldThrow()
		{
			var cipher = new MonitoringBlockCipher(BlockSize);
			var iv = CryptoUtilities.GetRandomNonZeroBytes(BlockSize);
			var transform = CreateTransform(cipher, iv);

			var input = new byte[BlockSize + 1];
			var output = new byte[input.Length];

			Assert.ThrowsExactly<ArgumentException>(() =>
			{
				transform.Transform(input, output, encrypt: true);
			});
		}

		[TestMethod]
		public void Transform_WhenOutputTooSmall_ShouldThrow()
		{
			var cipher = new MonitoringBlockCipher(BlockSize);
			var iv = CryptoUtilities.GetRandomNonZeroBytes(BlockSize);
			var transform = CreateTransform(cipher, iv);

			var input = new byte[BlockSize];
			var output = new byte[BlockSize - 1];

			Assert.ThrowsExactly<ArgumentException>(() =>
			{
				transform.Transform(input, output, encrypt: true);
			});
		}

		[TestMethod]
		public void Transform_WhenRoundTripped_ShouldReturnOriginal()
		{
			var cipher = new MonitoringBlockCipher(BlockSize);
			var iv = CryptoUtilities.GetRandomNonZeroBytes(BlockSize);

			var transformEncrypt = CreateTransform(cipher, (byte[])iv.Clone());
			var transformDecrypt = CreateTransform(cipher, (byte[])iv.Clone());

			var plaintext = CryptoUtilities.GetRandomNonZeroBytes(BlockSize * 2);
			var ciphertext = new byte[plaintext.Length];
			var decrypted = new byte[plaintext.Length];

			transformEncrypt.Transform(plaintext, ciphertext, encrypt: true);
			transformDecrypt.Transform(ciphertext, decrypted, encrypt: false);

			CollectionAssert.AreEqual(plaintext, decrypted);
		}

		protected abstract TTransform CreateTransform(IBlockCipher cipher, byte[] iv);
	}
}