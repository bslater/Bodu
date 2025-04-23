// ---------------------------------------------------------------------------------------------------------------
// <copyright file="CrcTests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="Crc" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class CrcTests
		: Security.Cryptography.HashAlgorithmTests<Crc>
	{
		/// <inheritdoc />
		protected override Crc CreateAlgorithm() => new Crc();

		/// <inheritdoc />
		protected override string ExpectedHash_ForEmptyByteArray => "00000000";
	}
}