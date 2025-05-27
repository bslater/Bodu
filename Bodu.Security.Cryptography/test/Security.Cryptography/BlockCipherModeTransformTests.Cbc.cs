using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Security.Cryptography;
using Bodu.Testing.Security;

namespace Bodu.Tests.Security.Cryptography
{
	[TestClass]
	public sealed class CbcModeTransformTests : BlockCipherModeTransformTests<CbcModeTransform>
	{
		[TestMethod]
		public void Transform_WhenEncrypting_ShouldApplyCBCChaining()
		{
			var cipher = new MonitoringBlockCipher(BlockSize, xorMask: 0x00); // Identity cipher
			var iv = new byte[] { 0x10, 0x20, 0x30, 0x40 };
			var transform = CreateTransform(cipher, (byte[])iv.Clone());

			var plaintext = new byte[]
			{
				0x01, 0x02, 0x03, 0x04,
				0x05, 0x06, 0x07, 0x08
			};
			var output = new byte[plaintext.Length];

			transform.Transform(plaintext, output, encrypt: true);

			var expectedBlock1 = plaintext[..BlockSize].Zip(iv, (a, b) => (byte)(a ^ b)).ToArray();
			var expectedBlock2 = plaintext[BlockSize..].Zip(expectedBlock1, (a, b) => (byte)(a ^ b)).ToArray();

			CollectionAssert.AreEqual(expectedBlock1, output[..BlockSize].ToArray());
			CollectionAssert.AreEqual(expectedBlock2, output[BlockSize..].ToArray());
		}

		protected override CbcModeTransform CreateTransform(IBlockCipher cipher, byte[] iv)
					=> new CbcModeTransform(cipher, iv);
	}
}