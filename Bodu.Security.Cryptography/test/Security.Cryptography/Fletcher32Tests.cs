// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Adler32Tests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="Fletcher32" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class Fletcher32Tests
		: Security.Cryptography.FletcherTests<Fletcher32>
	{
		/// <inheritdoc />
		protected override int ExpectedInputBlockSize => 4;

		/// <inheritdoc />
		protected override int ExpectedOutputBlockSize => 4;

		/// <inheritdoc />
		protected override Fletcher32 CreateAlgorithm() => new Fletcher32();

		/// <inheritdoc />
		protected override string ExpectedHash_ForEmptyByteArray => "00000000";
	}
}