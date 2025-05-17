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
		protected override string ExpectedHash_ForEmptyByteArray => "37045CCA405EE6FBDF815ED8B57C971BB78DAFB58F3EF676C977A716F66DBD8F376FEF59D2E0687CF5608C5DAD53BA42C8456269F3F3BCFB27D9B75CAAA26E11";
	}
}