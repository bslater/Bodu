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
		/// Verifies that two <see cref="Crc" /> instances with identical configurations produce the same hash code.
		/// </summary>
		[TestMethod]
		public void GetHashCode_WhenSameConfiguration_ShouldBeEqual()
		{
			var crc1 = new Crc(CrcStandard.CRC32_ISOHDLC);
			var crc2 = new Crc(CrcStandard.CRC32_ISOHDLC);

			Assert.AreEqual(crc1.GetHashCode(), crc2.GetHashCode());
		}

		/// <summary>
		/// Verifies that <see cref="Crc.GetHashCode" /> returns different hash codes for different configurations.
		/// </summary>
		[TestMethod]
		public void GetHashCode_WhenDifferentConfiguration_ShouldDiffer()
		{
			var crc1 = new Crc(CrcStandard.CRC32_ISOHDLC);
			var crc2 = new Crc(new CrcStandard(
				name: "Different",
				size: 32,
				polynomial: CrcStandard.CRC32_ISOHDLC.Polynomial,
				initialValue: 0x00000000,
				reflectIn: false,
				reflectOut: false,
				xOrOut: 0x00000000));

			Assert.AreNotEqual(crc1.GetHashCode(), crc2.GetHashCode());
		}
	}
}