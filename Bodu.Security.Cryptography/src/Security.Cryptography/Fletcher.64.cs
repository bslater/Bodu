// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Fletcher64.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes the hash for the input data using the <c>Fletcher-64</c> hash algorithm. This variant performs a non-cryptographic checksum
	/// calculation using two 32-bit accumulators to efficiently detect errors in byte sequences. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="Fletcher64" /> is a non-cryptographic hash algorithm that computes a 64-bit checksum by iterating over the input data. It
	/// was invented by Brian Kernighan and Dennis Ritchie, and is typically used for error-checking in applications such as network protocols.
	/// </para>
	/// </remarks>
	public sealed class Fletcher64
		: Security.Cryptography.Fletcher<Fletcher64>
	{
		// Define a constant for the hash size
		private const int FletcherHashSize = 64;

		/// <summary>
		/// Initializes a new instance of the <see cref="Fletcher64" /> class with a 64-bit hash size.
		/// </summary>
		public Fletcher64()
			: base(FletcherHashSize)
		{ }
	}
}