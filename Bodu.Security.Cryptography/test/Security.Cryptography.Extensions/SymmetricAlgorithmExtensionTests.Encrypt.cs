using System;
using System.IO;
using System.Security.Cryptography;
using Bodu.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu.Security.Cryptography.Extensions
{
	public partial class SymmetricAlgorithmExtensionTests
	{
		[TestMethod]
		public void Encrypt_WhenUsingVariousOverloads_ShouldDisposeTransform()
		{
			using var algorithm = new SimpleReversingSymmetricAlgorithm();
			var buffer = new byte[0];
			var source = new MemoryStream();
			var target = new MemoryStream();

			Assert.IsTrue(algorithm.IsCryptoTransformDisposed);
			algorithm.Encrypt(buffer);
			Assert.IsTrue(algorithm.IsCryptoTransformDisposed);

			algorithm.Encrypt(buffer, 0);
			Assert.IsTrue(algorithm.IsCryptoTransformDisposed);

			algorithm.Encrypt(buffer, 0, 0);
			Assert.IsTrue(algorithm.IsCryptoTransformDisposed);

			algorithm.Encrypt(source, target);
			Assert.IsTrue(algorithm.IsCryptoTransformDisposed);

			algorithm.Encrypt(source, target, 1024);
			Assert.IsTrue(algorithm.IsCryptoTransformDisposed);
		}

		[DataTestMethod]
		[DataRow(0, -1)]
		[DataRow(0, 1)]
		public void Encrypt_WhenOffsetIsInvalid_ShouldThrow(int size, int offset)
		{
			var algorithm = new SimpleReversingSymmetricAlgorithm();
			var buffer = new byte[size];

			Assert.ThrowsException<ArgumentOutOfRangeException>(() => algorithm.EncryptCbc(buffer, offset));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => algorithm.Encrypt(buffer, offset, 0));
		}

		[DataTestMethod]
		[DataRow(0, 0, -1)]
		[DataRow(1, 1, 1)]
		public void Encrypt_WhenCountIsInvalid_ShouldThrow(int size, int offset, int count)
		{
			var algorithm = new SimpleReversingSymmetricAlgorithm();
			var buffer = new byte[size];

			Assert.ThrowsException<ArgumentOutOfRangeException>(() => algorithm.Encrypt(buffer, offset, count));
		}

		[DataTestMethod]
		[DataRow(-1)]
		[DataRow(0)]
		public void Encrypt_WhenBufferSizeIsInvalid_ShouldThrow(int bufferSize)
		{
			var algorithm = new SimpleReversingSymmetricAlgorithm();
			var source = new MemoryStream();
			var target = new MemoryStream();

			Assert.ThrowsException<ArgumentOutOfRangeException>(() => algorithm.Encrypt(source, target, bufferSize));
		}

		[TestMethod]
		public void Encrypt_WhenAlgorithmIsNull_ShouldThrow()
		{
			SymmetricAlgorithm algorithm = null;
			var buffer = new byte[0];
			var stream = new MemoryStream();

			Assert.ThrowsException<ArgumentNullException>(() => SymmetricAlgorithmExtensions.Encrypt(algorithm, buffer));
			Assert.ThrowsException<ArgumentNullException>(() => SymmetricAlgorithmExtensions.Encrypt(algorithm, buffer, 0));
			Assert.ThrowsException<ArgumentNullException>(() => SymmetricAlgorithmExtensions.Encrypt(algorithm, buffer, 0, 0));
			Assert.ThrowsException<ArgumentNullException>(() => SymmetricAlgorithmExtensions.Encrypt(algorithm, stream, stream));
			Assert.ThrowsException<ArgumentNullException>(() => SymmetricAlgorithmExtensions.Encrypt(algorithm, stream, stream, 1024));
		}

		[TestMethod]
		public void Encrypt_WhenArrayIsNull_ShouldThrow()
		{
			var algorithm = new SimpleReversingSymmetricAlgorithm();
			byte[] buffer = null;

			Assert.ThrowsException<ArgumentNullException>(() => algorithm.Encrypt(buffer));
			Assert.ThrowsException<ArgumentNullException>(() => algorithm.Encrypt(buffer, 0));
			Assert.ThrowsException<ArgumentNullException>(() => algorithm.Encrypt(buffer, 0, 0));
		}

		[TestMethod]
		public void Encrypt_WhenSourceStreamIsNull_ShouldThrow()
		{
			var algorithm = new SimpleReversingSymmetricAlgorithm();
			Stream source = null;
			Stream target = new MemoryStream();

			Assert.ThrowsException<ArgumentNullException>(() => algorithm.Encrypt(source, target));
			Assert.ThrowsException<ArgumentNullException>(() => algorithm.Encrypt(source, target, 1024));
		}

		[TestMethod]
		public void Encrypt_WhenTargetStreamIsNull_ShouldThrow()
		{
			var algorithm = new SimpleReversingSymmetricAlgorithm();
			Stream source = new MemoryStream();
			Stream target = null;

			Assert.ThrowsException<ArgumentNullException>(() => algorithm.Encrypt(source, target));
			Assert.ThrowsException<ArgumentNullException>(() => algorithm.Encrypt(source, target, 1024));
		}

		[TestMethod]
		public void Encrypt_ShouldNotDisposeStreams()
		{
			var algorithm = new SimpleReversingSymmetricAlgorithm();
			var source = new MemoryStream();
			var target = new MemoryStream();

			algorithm.Encrypt(source, target);

			Assert.IsTrue(source.CanRead);
			Assert.IsTrue(target.CanWrite);
		}
	}
}