// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Adler32Tests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="Adler32" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class Adler32Tests
		: Security.Cryptography.HashAlgorithmTests<Adler32>
	{
		/// <inheritdoc />
		protected override Adler32 CreateAlgorithm() => new Adler32();

		/// <inheritdoc />
		protected override string ExpectedHash_ForEmptyByteArray => "00000001";
	}
}