// -----------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="Adler" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class BernsteinTests
		: Security.Cryptography.HashAlgorithmTests<Bernstein>
	{
		/// <inheritdoc />
		protected override Bernstein CreateAlgorithm() => new Bernstein();

		/// <inheritdoc />
		protected override string ExpectedHash_ForEmptyByteArray => "00001505";
	}
}