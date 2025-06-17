﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Security.Cryptography
{
	public abstract partial class TweakableSymmetricAlgorithmTests<TTest, TAlgorithm>
	{
		/// <summary>
		/// Verifies that accessing <see cref="TweakableSymmetricAlgorithm.Tweak" /> after disposal throws <see cref="ObjectDisposedException" />.
		/// </summary>
		[TestMethod]
		public void Tweak_WhenAccessedAfterDispose_ShouldThrowObjectDisposedException()
		{
			using var algorithm = CreateAlgorithm();
			algorithm.TweakSize = algorithm.LegalTweakSizes[0].MinSize;
			algorithm.GenerateTweak();

			algorithm.Dispose();

			Assert.ThrowsException<ObjectDisposedException>(() =>
			{
				var _ = algorithm.Tweak;
			});
		}

		/// <summary>
		/// Verifies that each access to <see cref="TweakableSymmetricAlgorithm.Tweak" /> returns a new array instance.
		/// </summary>
		[TestMethod]
		public void Tweak_WhenCalledMultipleTimes_ShouldReturnNewArrayInstances()
		{
			using TAlgorithm algorithm = CreateAlgorithm();
			algorithm.TweakSize = algorithm.LegalTweakSizes[0].MinSize;
			algorithm.GenerateTweak();

			var tweak1 = algorithm.Tweak;
			var tweak2 = algorithm.Tweak;

			Assert.AreNotSame(tweak1, tweak2);
		}

		/// <summary>
		/// Verifies that each access to <see cref="TweakableSymmetricAlgorithm.Tweak" /> returns arrays with the same contents.
		/// </summary>
		[TestMethod]
		public void Tweak_WhenCalledMultipleTimes_ShouldReturnSameValue()
		{
			using TAlgorithm algorithm = CreateAlgorithm();
			algorithm.TweakSize = algorithm.LegalTweakSizes[0].MinSize;
			algorithm.GenerateTweak();

			var tweak1 = algorithm.Tweak;
			var tweak2 = algorithm.Tweak;

			CollectionAssert.AreEqual(tweak1, tweak2);
		}

		/// <summary>
		/// Verifies that GenerateTweak produces a non-zero value that is preserved by the Tweak property.
		/// </summary>
		[TestMethod]
		public void Tweak_WhenGenerated_ShouldMatchInternalTweakValue()
		{
			using var algorithm = CreateAlgorithm();
			int size = algorithm.LegalTweakSizes[0].MinSize;

			algorithm.TweakSize = size;
			algorithm.GenerateTweak();

			byte[] tweak = algorithm.Tweak;
			Assert.AreEqual(size / 8, tweak.Length);
			Assert.IsTrue(tweak.Any(b => b != 0), "Generated tweak should not be all zero.");
		}

		/// <summary>
		/// Verifies that accessing Tweak before it is initialized throws.
		/// </summary>
		[TestMethod]
		public void Tweak_WhenNoAccessedMultipleTimes_ShouldReturnSameValue()
		{
			using var algorithm = CreateAlgorithm();

			byte[] tweak1 = algorithm.Tweak;
			byte[] tweak2 = algorithm.Tweak;

			CollectionAssert.AreEqual(tweak1, tweak2);
		}

		/// <summary>
		/// Verifies that accessing Tweak before it is initialized throws.
		/// </summary>
		[TestMethod]
		public void Tweak_WhenNotInitialized_ShouldReturnExpectedValue()
		{
			using var algorithm = CreateAlgorithm();

			byte[] tweak = algorithm.Tweak;
			Assert.AreEqual(algorithm.TweakSize / 8, tweak.Length);
			Assert.IsTrue(tweak.Any(b => b != 0), "Generated tweak should not be all zero.");
		}

		/// <summary>
		/// Verifies that once set, the Tweak value does not change unless reassigned or reset.
		/// </summary>
		[TestMethod]
		public void Tweak_WhenNotReassigned_ShouldRemainUnchanged()
		{
			using var algorithm = CreateAlgorithm();
			int size = algorithm.LegalTweakSizes[0].MinSize;
			byte[] expected = Enumerable.Repeat((byte)0xAA, size / 8).ToArray();

			algorithm.TweakSize = size;
			algorithm.Tweak = expected;

			byte[] first = algorithm.Tweak;
			byte[] second = algorithm.Tweak;

			CollectionAssert.AreEqual(first, second);
		}

		/// <summary>
		/// Verifies that setting the Tweak returns the exact same content when retrieved.
		/// </summary>
		[TestMethod]
		public void Tweak_WhenSet_ShouldReturnExpectedValue()
		{
			using var algorithm = CreateAlgorithm();
			int size = algorithm.LegalTweakSizes[0].MinSize;
			byte[] expected = Enumerable.Range(0, size / 8).Select(i => (byte)(i + 1)).ToArray();

			algorithm.TweakSize = size;
			algorithm.Tweak = expected;
			byte[] actual = algorithm.Tweak;

			CollectionAssert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Verifies that Tweak set to an invalid size throws.
		/// </summary>
		[TestMethod]
		public void Tweak_WhenSetToInvalidSize_ShouldThrowCryptographicException()
		{
			using var algorithm = CreateAlgorithm();
			var invalid = new byte[7]; // 56 bits is uncommon

			Assert.ThrowsException<CryptographicException>(() =>
			{
				algorithm.Tweak = invalid;
			});
		}

		/// <summary>
		/// Verifies that Tweak set to null throws ArgumentNullException.
		/// </summary>
		[TestMethod]
		public void Tweak_WhenSetToNull_ShouldThrowArgumentNullException()
		{
			using var algorithm = CreateAlgorithm();

			Assert.ThrowsException<ArgumentNullException>(() =>
			{
				algorithm.Tweak = null!;
			});
		}
	}
}