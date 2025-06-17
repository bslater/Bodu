using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Security.Cryptography.Extensions
{
	public partial class SymmetricAlgorithmExtensionTests
	{
		/// <summary>
		/// Verifies TryCreateEncryptor returns true and produces a transform for valid inputs.
		/// </summary>
		[TestMethod]
		public void TryCreateEncryptor_WhenAValid_ShouldReturnTrue()
		{
			using var algorithm = CreateAlgorithm();

			bool result = algorithm.TryCreateEncryptor(out ICryptoTransform transform);
			Assert.IsTrue(result);
			Assert.IsNotNull(transform);
		}

		/// <summary>
		/// Verifies that <see cref="SymmetricAlgorithm.TryCreateEncryptor(byte[], byte[], ut ICryptoTransform)" /> returns <c>false</c> and
		/// outputs <c>null</c> when the IV is <c>null</c>.
		/// </summary>
		[TestMethod]
		public void TryCreateEncryptor_WhenIVIsNull_ShouldReturnFalseAndNullOutput()
		{
			using var algorithm = CreateAlgorithm();
			bool result = algorithm.TryCreateEncryptor(new byte[algorithm.KeySize / 8], null!, out var transform);

			Assert.IsFalse(result);
			Assert.IsNull(transform);
		}

		/// <summary>
		/// Verifies that <see cref="SymmetricAlgorithm.TryCreateEncryptor(byte[], byte[],out ICryptoTransform)" /> returns <c>false</c> and
		/// outputs <c>null</c> when the IV length is invalid.
		/// </summary>
		[TestMethod]
		public void TryCreateEncryptor_WhenIVSizeIsInvalid_ShouldReturnFalseAndNullOutput()
		{
			using var algorithm = CreateAlgorithm();

			var key = new byte[algorithm.KeySize / 8];
			var iv = new byte[(algorithm.BlockSize / 8) + 1]; // one byte too long

			bool result = algorithm.TryCreateEncryptor(key, iv, out var transform);
			Assert.IsFalse(result);
			Assert.IsNull(transform);
		}

		[TestMethod]
		public void TryCreateEncryptor_WhenKeyAndIvAreUnset_ShouldGenerateAndReturnTransform()
		{
			using var algorithm = CreateAlgorithm();

			bool result = algorithm.TryCreateEncryptor(out var transform);

			Assert.IsTrue(result);
			Assert.IsNotNull(transform);
		}

		/// <summary>
		/// Verifies that <see cref="SymmetricAlgorithm.TryCreateEncryptor(byte[], byte[],  out ICryptoTransform)" /> returns <c>false</c>
		/// and outputs <c>null</c> when the key is <c>null</c>.
		/// </summary>
		[TestMethod]
		public void TryCreateEncryptor_WhenKeyIsNull_ShouldReturnFalseAndNullOutput()
		{
			using var algorithm = CreateAlgorithm();
			bool result = algorithm.TryCreateEncryptor(null!, new byte[algorithm.BlockSize / 8], out var transform);

			Assert.IsFalse(result);
			Assert.IsNull(transform);
		}

		/// <summary>
		/// Verifies that <see cref="SymmetricAlgorithm.TryCreateEncryptor(byte[], byte[],  out ICryptoTransform)" /> returns <c>false</c>
		/// and outputs <c>null</c> when the key length is invalid.
		/// </summary>
		[TestMethod]
		public void TryCreateEncryptor_WhenKeySizeIsInvalid_ShouldReturnFalseAndNullOutput()
		{
			using var algorithm = CreateAlgorithm();

			// One byte short of required key size
			var key = new byte[(algorithm.KeySize / 8) + 1];// one byte too long
			var iv = new byte[algorithm.BlockSize / 8];

			bool result = algorithm.TryCreateEncryptor(key, iv, out var transform);
			Assert.IsFalse(result);
			Assert.IsNull(transform);
		}

		/// <summary>
		/// Verifies TryCreateEncryptor returns true and produces a transform for valid inputs.
		/// </summary>
		[TestMethod]
		public void TryCreateEncryptor_WhenValid_ShouldReturnTrue()
		{
			using var algorithm = CreateAlgorithm();

			var key = new byte[algorithm.KeySize / 8];
			var iv = new byte[algorithm.BlockSize / 8];

			bool result = algorithm.TryCreateEncryptor(key, iv, out ICryptoTransform transform);
			Assert.IsTrue(result);
			Assert.IsNotNull(transform);
		}
	}
}