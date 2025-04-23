// -----------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Bodu.Infrastructure;

namespace Bodu.Security.Cryptography
{
	public partial class CrcTests
	{
		/// <summary>
		/// Verifies that two <see cref="Crc" /> instances with identical configurations are considered equal.
		/// </summary>
		[TestMethod]
		public void Equals_WhenSameConfiguration_ShouldReturnTrue()
		{
			var a = new Crc(CrcStandard.CRC32_ISOHDLC);
			var b = new Crc(CrcStandard.CRC32_ISOHDLC);

			Assert.IsTrue(a.Equals(b));
			Assert.IsTrue(b.Equals(a));
			Assert.IsTrue(a.Equals((object)b));
		}

		/// <summary>
		/// Verifies that <see cref="Crc.Equals(object)" /> returns <c>false</c> when configurations differ.
		/// </summary>
		[TestMethod]
		public void Equals_WhenDifferentConfiguration_ShouldReturnFalse()
		{
			var crcA = new Crc(CrcStandard.CRC32_ISOHDLC);
			var crcB = new Crc(new CrcStandard(
				name: "Custom",
				size: 32,
				polynomial: CrcStandard.CRC32_ISOHDLC.Polynomial,
				initialValue: 0x12345678,
				reflectIn: true,
				reflectOut: true,
				xOrOut: 0xFFFFFFFF));

			Assert.IsFalse(crcA.Equals(crcB));
			Assert.IsFalse(crcB.Equals(crcA));
			Assert.IsFalse(crcA.Equals((object)crcB));
		}

		/// <summary>
		/// Verifies that <see cref="Crc.Equals(object)" /> returns <c>false</c> when comparing with <c>null</c>.
		/// </summary>
		[TestMethod]
		public void Equals_WhenComparedWithNull_ShouldReturnFalse()
		{
			var crc = new Crc(CrcStandard.CRC32_ISOHDLC);

			Assert.IsFalse(crc.Equals(null));
			Assert.IsFalse(crc.Equals((object)null));
		}

		/// <summary>
		/// Verifies that <see cref="Crc.Equals(object)" /> returns <c>false</c> when compared to an object of a different type.
		/// </summary>
		[TestMethod]
		public void Equals_WhenComparedToNonCrcObject_ShouldReturnFalse()
		{
			var crc = new Crc(CrcStandard.CRC32_ISOHDLC);

			Assert.IsFalse(crc.Equals("not a Crc instance"));
		}
	}
}