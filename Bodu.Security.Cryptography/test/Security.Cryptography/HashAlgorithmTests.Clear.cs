// ---------------------------------------------------------------------------------------------------------------
// <copyright file="HashAlgorithmTests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	public abstract partial class HashAlgorithmTests<T>
	{
		/// <summary>
		/// Verifies that calling <see cref="HashAlgorithm.Clear" /> disposes the instance, and any further attempts to use it result in <see cref="ObjectDisposedException" />.
		/// </summary>
		[TestMethod]
		public void Clear_ShouldDisposeAlgorithmAndPreventFurtherUse()
		{
			byte[] buffer = null!;

			using var algorithm = this.CreateAlgorithm();

			// Expect ArgumentNullException on null buffer
			Assert.ThrowsException<ArgumentNullException>(() => algorithm.ComputeHash(buffer));

			// Clear should dispose
			algorithm.Clear();

			// Any further call should throw ObjectDisposedException
			Assert.ThrowsException<ObjectDisposedException>(() => algorithm.ComputeHash(buffer));
		}
	}
}