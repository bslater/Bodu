// ---------------------------------------------------------------------------------------------------------------
// <copyright file="HashAlgorithmTests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Bodu.Infrastructure;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Base class for reusable unit tests of hash algorithms.
	/// </summary>
	/// <typeparam name="T">The hash algorithm type under test.</typeparam>
	public abstract partial class HashAlgorithmTests<T>
		: Security.Cryptography.CryptoTransformTests<T>
		where T : HashAlgorithm
	{
		/// <summary>
		/// Returns all algorithm variants to test. Must be overridden in the concrete test class.
		/// </summary>
		public abstract IEnumerable<HashAlgorithmVariant> GetVariants();

		// Property to control if partial blocks should be handled
		public virtual bool HandlePartialBlocks => true;

		public virtual bool IsStateless => false;

		/// <summary>
		/// Gets the expected hexadecimal hash hashValue for an empty byte array input.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This property should be overridden in derived test classes to provide the expected hash output for the specific algorithm being
		/// tested when given an empty buffer ( <c>new byte[0]</c>).
		/// </para>
		/// <para>
		/// It is used in conjunction with the <see cref="CryptoTransformTests{T}.CreateAlgorithm" /> factory method to validate that
		/// <see cref="HashAlgorithm.ComputeHash(byte[])" /> returns the correct result.
		/// </para>
		/// </remarks>
		/// <hashValue>A string representing the expected hash hashValue as an uppercase hexadecimal string without delimiters (e.g., <c>"DEADBEEF"</c>).</hashValue>
		protected abstract string ExpectedHash_ForEmptyByteArray { get; }

		/// <summary>
		/// Gets or sets the expected hashValue of the <see cref="HashAlgorithm.InputBlockSize" /> property. Default is 1 for stream-based algorithms.
		/// </summary>
		protected virtual int ExpectedInputBlockSize { get; init; } = 1;

		/// <summary>
		/// Gets or sets the expected hashValue of the <see cref="HashAlgorithm.OutputBlockSize" /> property. Default is 1 for most hash algorithms.
		/// </summary>
		protected virtual int ExpectedOutputBlockSize { get; init; } = 1;

		/// <summary>
		/// Provides writable public properties from the hash algorithm under test for validation. This supports dynamic tests that check
		/// whether setting properties after hashing begins throws expected exceptions.
		/// </summary>
		/// <returns>
		/// An enumerable of object arrays, where each array contains a single <see cref="PropertyInfo" /> representing a writable property.
		/// </returns>
		protected static IEnumerable<object[]> GetWritableProperties()
		{
			var algorithmType = typeof(T); // T is the hash algorithm under test
			var properties = algorithmType
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
				.Where(p => p.SetMethod?.IsPublic == true)
				.ToList();

			if (properties.Count == 0)
			{
				// Yield a marker to trigger the test with an inconclusive outcome
				yield return new object[] { null! };
				yield break;
			}

			foreach (var prop in properties)
				yield return new object[] { prop };
		}

		/// <summary>
		/// Helper to assert a hash result matches expected hex.
		/// </summary>
		/// <param name="input">The input bytes.</param>
		/// <param name="expectedHex">Expected hex hash result.</param>
		protected static void AssertHashMatches(HashAlgorithmVariant variant, byte[] input, string expectedHex)
		{
			using HashAlgorithm algorithm = variant.Factory.Invoke();
			byte[] actual = algorithm.ComputeHash(input);
			byte[] expected = Convert.FromHexString(expectedHex);
			CollectionAssert.AreEqual(expected, actual, $"Hash mismatch for variant '{variant.Name}'.");
		}

		/// <summary>
		/// Executes the common test that verifies progressively longer byte prefixes hash to expected results.
		/// </summary>
		/// <param name="variant">The algorithm variant containing the expected test data and factory.</param>
		protected static void AssertHashMatchesInputPrefixes(HashAlgorithmVariant variant)
		{
			using HashAlgorithm algorithm = variant.Factory.Invoke();

			int vectorCount = algorithm.HashSize / 2;
			byte[] input = new byte[vectorCount];
			string[] expectedHex = variant.ExpectedHash_ForInputPrefixes as string[]
				?? variant.ExpectedHash_ForInputPrefixes.ToArray();

			Assert.AreEqual(vectorCount, expectedHex.Length, "Unexpected number of test vectors.");

			for (int i = 0; i < vectorCount; i++)
			{
				input[i] = (byte)i;
				byte[] expected = Convert.FromHexString(expectedHex[i]);
				byte[] actual = algorithm.ComputeHash(input, 0, i);

				CollectionAssert.AreEqual(expected, actual, $"Hash mismatch at prefix length {i}");
			}
		}

		protected static void AssertHashMatchesTestVector(HashAlgorithmVariant variant)
		{
			foreach (var testVector in variant.ExpectedHash_ForHashTestVectors)
			{
				using HashAlgorithm algorithm = variant.Factory.Invoke();
				byte[] input = Encoding.ASCII.GetBytes(testVector.InputHex);
				byte[] actual = algorithm.ComputeHash(input);
				string result = Convert.ToHexString(actual);
				Assert.AreEqual(testVector.ExpectedHex, result, $"Hash mismatch for variant '{variant.Name}' test vector.");
			}
		}
	}
}