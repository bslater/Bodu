﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Security.Cryptography;
using Bodu.Testing.Security;

namespace Bodu.Security.Cryptography
{
	public sealed partial class EcbModeTransformTests
	{
		[TestMethod]
		public void Transform_WhenDecrypting_ShouldDecryptEachBlockIndependently()
		{
			var cipher = new MonitoringBlockCipher(ExpectedBlockSize, xorMask: 0xAA); // XOR cipher
			var transform = CreateTransform(cipher, iv: null); // ECB ignores IV

			var original = Enumerable.Range(0, ExpectedBlockSize * 2).Select(i => (byte)i).ToArray();
			var encrypted = original.Select(b => (byte)(b ^ 0xAA)).ToArray();
			var decrypted = new byte[encrypted.Length];

			transform.Transform(encrypted, decrypted, encrypt: false);

			CollectionAssert.AreEqual(original, decrypted, "Decrypted output should match original plaintext in ECB mode.");
		}

		[TestMethod]
		public void Transform_WhenEncrypting_ShouldEncryptEachBlockIndependently()
		{
			var cipher = new MonitoringBlockCipher(ExpectedBlockSize, xorMask: 0xAA); // Applies XOR with 0xAA per byte
			var transform = CreateTransform(cipher, iv: null); // ECB ignores IV

			var plaintext = Enumerable.Range(0, ExpectedBlockSize * 2).Select(i => (byte)i).ToArray();
			var output = new byte[plaintext.Length];

			transform.Transform(plaintext, output, encrypt: true);

			// ECB should apply the block transform directly to each block
			var expectedBlock1 = plaintext[..ExpectedBlockSize].Select(b => (byte)(b ^ 0xAA)).ToArray();
			var expectedBlock2 = plaintext[ExpectedBlockSize..].Select(b => (byte)(b ^ 0xAA)).ToArray();

			CollectionAssert.AreEqual(expectedBlock1, output[..ExpectedBlockSize].ToArray(), "First block did not match expected ECB output.");
			CollectionAssert.AreEqual(expectedBlock2, output[ExpectedBlockSize..].ToArray(), "Second block did not match expected ECB output.");
		}

		[TestMethod]
		public void Transform_WhenPlaintextBlocksAreIdentical_ShouldProduceIdenticalCipherTextBlocks()
		{
			var cipher = new MonitoringBlockCipher(ExpectedBlockSize, xorMask: 0xAA);
			var transform = CreateTransform(cipher, iv: null);

			// Two identical plaintext blocks
			var block = Enumerable.Range(0, ExpectedBlockSize).Select(i => (byte)i).ToArray();
			var plaintext = block.Concat(block).ToArray();
			var output = new byte[plaintext.Length];

			transform.Transform(plaintext, output, encrypt: true);

			CollectionAssert.AreEqual(output[..ExpectedBlockSize], output[ExpectedBlockSize..],
				"ECB mode should produce identical ciphertext for identical plaintext blocks.");
		}
	}
}