using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu.Security.Cryptography
{
	public abstract partial class SipHashTests<T>
	{
		/// <summary>
		/// Verifies that the constructor throws an <see cref="ArgumentException" /> when provided an invalid <c>hashSize</c>.
		/// </summary>
		/// <param name="hashSize">The invalid hash size to test.</param>
		[TestMethod]
		[DataRow(0)]
		[DataRow(8)]
		[DataRow(63)]
		[DataRow(129)]
		[DataRow(-1)]
		public void Ctor_WhenHashSizeIsInvalid_ShouldThrowArgumentException(int hashSize)
		{
			Assert.ThrowsException<ArgumentException>(() =>
			{
				new TestSipHash(hashSize);
			});
		}

		/// <summary>
		/// Verifies that the Fletcher constructor accepts only valid <c>hashSize</c> of 16, 32, or 64.
		/// </summary>
		[TestMethod]
		[DataRow(64)]
		[DataRow(128)]
		public void Ctor_WhenHashSizeIsValid_ShouldSucceed(int hashSize)
		{
			var algorithm = new TestSipHash(hashSize);
			Assert.IsNotNull(algorithm);
		}

		private class TestSipHash
		: SipHash
		{
			public TestSipHash(int hashSize)
				: base(hashSize)
			{ }
		}
	}
}