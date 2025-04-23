// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Adler32Tests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="Fletcher" /> hash algorithm.
	/// </summary>
	[TestClass]
	public abstract partial class FletcherTests<T>
		: Security.Cryptography.HashAlgorithmTests<T>
		where T : Fletcher
	{
	}
}