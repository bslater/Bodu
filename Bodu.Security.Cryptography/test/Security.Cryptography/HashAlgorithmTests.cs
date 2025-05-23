// ---------------------------------------------------------------------------------------------------------------
// <copyright file="HashAlgorithmTests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Bodu.Infrastructure;
using Newtonsoft.Json.Linq;

namespace Bodu.Security.Cryptography
{
	public enum SingleHashVariant
	{
		Default
	}

	/// <summary>
	/// Base class for reusable unit tests of hash algorithms.
	/// </summary>
	/// <typeparam name="T">The hash algorithm type under test.</typeparam>
	public abstract partial class HashAlgorithmTests<TTest, TAlgorithm, TVariant>
		: Security.Cryptography.CryptoTransformTests<TAlgorithm>
		where TTest : HashAlgorithmTests<TTest, TAlgorithm, TVariant>, new()
		where TAlgorithm : HashAlgorithm, new()
		where TVariant : Enum
	{
		/// <summary>
		/// Returns all variants to be tested.
		/// </summary>
		public abstract IEnumerable<TVariant> GetHashAlgorithmVariants();

		// Property to control if partial blocks should be handled
		public virtual bool HandlePartialBlocks => true;

		public virtual bool IsStateless => false;

		protected override TAlgorithm CreateAlgorithm() =>
			this.CreateAlgorithm(DefaultVariant);

		/// <summary>
		/// Returns the default variant to use in parameterless scenarios.
		/// </summary>
		protected virtual TVariant DefaultVariant => GetHashAlgorithmVariants().First();

		/// <summary>
		/// Returns the expected hash result when hashing an empty byte array using the default variant.
		/// </summary>
		/// <exception cref="KeyNotFoundException">Thrown if the expected hash for the empty input is not defined for the default variant.</exception>
		protected override byte[] ExpectedEmptyInputHash =>
			Convert.FromHexString(
				GetExpectedHashesForNamedInputs(DefaultVariant).TryGetValue("Empty", out var hex)
					? hex
					: throw new KeyNotFoundException(
						$"Expected hash for \"Empty\" input is not defined for variant '{DefaultVariant}'."));

		protected abstract TAlgorithm CreateAlgorithm(TVariant variant);

		/// <summary>
		/// Returns expected hash outputs for well-known named test inputs (e.g., "Empty", "ABC").
		/// </summary>
		protected abstract IReadOnlyDictionary<string, string> GetExpectedHashesForNamedInputs(TVariant variant);

		/// <summary>
		/// Returns expected hash outputs for incremental byte sequences input[0..i].
		/// </summary>
		protected abstract IReadOnlyList<string> GetExpectedHashesForIncrementalInput(TVariant variant);

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
			var algorithmType = typeof(TAlgorithm); // T is the hash algorithm under test
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
		/// Shared input vectors used across all hash algorithm tests.
		/// </summary>
		protected static readonly IReadOnlyDictionary<string, byte[]> SharedInputs = new Dictionary<string, byte[]>
		{
			["Empty"] = Array.Empty<byte>(),
			["ABC"] = Encoding.ASCII.GetBytes("ABC"),
			["QuickBrownFox"] = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"),
			["Zeros_16"] = new byte[16],
			["Sequential_0_255"] = Enumerable.Range(0, 255).Select(i => (byte)i).ToArray()
		};

		public static IEnumerable<object[]> ComputeHashNamedInputTestData()
		{
			var instance = new TTest();
			foreach (var variant in instance.GetHashAlgorithmVariants())
			{
				var testVectors = instance.GetTestVectors(variant);
				foreach (var vector in testVectors)
				{
					yield return new object[] { variant, vector.Name, vector.Input, vector.ExpectedHash };
				}
			}
		}

		public static IEnumerable<object[]> HashAlgorithmVariants() =>
			new TTest().GetHashAlgorithmVariants().Select(variant => new object[] { variant });

		/// <summary>
		/// Combines shared inputs with expected outputs into test vectors.
		/// </summary>
		protected virtual IEnumerable<HashTestVector> GetTestVectors(TVariant variant)
		{
			var expected = GetExpectedHashesForNamedInputs(variant);
			foreach (var (name, input) in SharedInputs)
			{
				if (expected.TryGetValue(name, out var hex))
				{
					yield return new HashTestVector
					{
						Name = name,
						Input = input,
						ExpectedHash = Convert.FromHexString(hex)
					};
				}
			}
		}

		[DataTestMethod]
		[DynamicData(nameof(HashAlgorithmVariants), DynamicDataSourceType.Method)]
		public void HashAlgorithm_TestData_Check(TVariant variant)
		{
			var emptyA = this.GetExpectedHashesForNamedInputs(variant)["Empty"];
			var emptyB = this.GetExpectedHashesForIncrementalInput(variant)[0];
			Assert.AreEqual(emptyA, emptyB, "Expected Hash value for 'Empty' Named Input should equal first item of Incremental Input");
		}
	}
}