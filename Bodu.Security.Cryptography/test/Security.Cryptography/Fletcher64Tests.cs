// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Adler32Tests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="Fletcher64" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class Fletcher64Tests
		: Security.Cryptography.FletcherTests<Fletcher64>
	{
		/// <inheritdoc />
		protected override int ExpectedInputBlockSize => 8;

		/// <inheritdoc />
		protected override int ExpectedOutputBlockSize => 8;

		/// <inheritdoc />
		protected override Fletcher64 CreateAlgorithm() => new Fletcher64();

		/// <inheritdoc />
		protected override string ExpectedHash_ForEmptyByteArray => "0000000000000000";
	}
}