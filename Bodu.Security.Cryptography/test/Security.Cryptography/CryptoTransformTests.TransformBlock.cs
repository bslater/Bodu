// ---------------------------------------------------------------------------------------------------------------
// <copyright file="CryptoTransformTests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Bodu.Security.Cryptography;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	public abstract partial class CryptoTransformTests<T>
	{
		/// <summary>
		/// Verifies that <see cref="ICryptoTransform.TransformBlock" /> throws an
		/// <see cref="ObjectDisposedException" /> after the algorithm is disposed.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void WhenDisposed_ShouldThrowObjectDisposedException_OnTransformBlock()
		{
			using var algorithm = this.CreateAlgorithm();
			algorithm.Dispose();
			algorithm.TransformBlock(CryptoTestUtilities.EmptyByteArray, 0, 0, null, 0);
		}
	}
}