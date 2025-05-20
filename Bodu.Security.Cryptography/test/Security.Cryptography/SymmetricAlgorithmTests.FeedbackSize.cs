﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Security.Cryptography
{
	public abstract partial class SymmetricAlgorithmTests<T>
	{
		/// <summary>
		/// Validates that setting an invalid feedback size throws a CryptographicException.
		/// </summary>
		/// <param name="feedbackSize">The feedback size to test.</param>
		[DataTestMethod]
		[DataRow(-1)]
		[DataRow(0)]
		[DataRow(null)]
		public void FeedbackSize_WhenInvalid_ShouldThrowExactly(int? feedbackSize)
		{
			using T algorithm = this.CreateAlgorithm();
			int sizeToTest = feedbackSize ?? algorithm.BlockSize + 1;
			Assert.ThrowsExactly<CryptographicException>(() => algorithm.FeedbackSize = sizeToTest);
		}
	}
}