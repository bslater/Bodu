// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Adler32Tests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="Fletcher" /> hash algorithm.
	/// </summary>
	[TestClass]
	public abstract partial class FletcherTests<TTest, TAlgorithm>
		: HashAlgorithmTests<TTest, TAlgorithm, SingleHashVariant>
		where TTest : HashAlgorithmTests<TTest, TAlgorithm, SingleHashVariant>, new()
		where TAlgorithm : Fletcher, new()
	{
		public override IEnumerable<SingleHashVariant> GetHashAlgorithmVariants() => new[]
		{
			SingleHashVariant.Default
		};
	}
}