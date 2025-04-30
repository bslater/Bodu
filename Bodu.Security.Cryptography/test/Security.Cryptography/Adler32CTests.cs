// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Adler32CTests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="Adler" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class Adler32CTests
		: Security.Cryptography.HashAlgorithmTests<Adler>
	{
		/// <inheritdoc />
		protected override Adler CreateAlgorithm() => new Adler32C();

		/// <inheritdoc />
		protected override string ExpectedHash_ForEmptyByteArray => "00000001";
	}
}