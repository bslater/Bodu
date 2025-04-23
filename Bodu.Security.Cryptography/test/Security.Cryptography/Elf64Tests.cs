// -----------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="Elf64" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class Elf64Tests
		: Security.Cryptography.HashAlgorithmTests<Elf64>
	{
		/// <inheritdoc />
		protected override Elf64 CreateAlgorithm() => new Elf64();

		/// <inheritdoc />
		protected override string ExpectedHash_ForEmptyByteArray => "0000000000000000";
	}
}