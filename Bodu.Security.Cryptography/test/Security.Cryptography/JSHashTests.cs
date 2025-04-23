// ---------------------------------------------------------------------------------------------------------------
// <copyright file="JSHashTests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="JSHash" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class JSHashTests
		: Security.Cryptography.HashAlgorithmTests<JSHash>
	{
		/// <inheritdoc />
		protected override JSHash CreateAlgorithm() => new JSHash();

		/// <inheritdoc />
		protected override string ExpectedHash_ForEmptyByteArray => "4E67C6A7";
	}
}