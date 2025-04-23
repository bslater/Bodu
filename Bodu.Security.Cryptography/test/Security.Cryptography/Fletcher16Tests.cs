// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Adler32Tests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="Fletcher16" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class Fletcher16Tests
		: Security.Cryptography.FletcherTests<Fletcher16>
	{
		/// <inheritdoc />
		protected override int ExpectedInputBlockSize => 2;

		/// <inheritdoc />
		protected override int ExpectedOutputBlockSize => 2;

		/// <inheritdoc />
		protected override Fletcher16 CreateAlgorithm() => new Fletcher16();

		/// <inheritdoc />
		protected override string ExpectedHash_ForEmptyByteArray => "0000";
	}
}