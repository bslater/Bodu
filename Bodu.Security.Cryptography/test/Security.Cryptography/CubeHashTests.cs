// -----------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="CubeHash" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class CubeHashTests
		: Security.Cryptography.HashAlgorithmTests<CubeHash>
	{
		/// <inheritdoc />
		protected override CubeHash CreateAlgorithm() => new CubeHash();

		/// <inheritdoc />
		protected override int ExpectedInputBlockSize => 32;

		/// <inheritdoc />
		protected override int ExpectedOutputBlockSize => 64;

		/// <inheritdoc />
		protected override string ExpectedHash_ForEmptyByteArray => "0000000000000000";
	}
}