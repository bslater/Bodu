﻿using Bodu.Collections.Generic.Extensions;
using Bodu.Infrastructure;
using System;

namespace Bodu.Collections.Generic
{
	public partial class ShuffleHelpersTests
	{
		/// <summary>
		/// Verifies that ShuffleAndYield for an array returns a subset of the specified count, and all returned elements are from the
		/// original array.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_Array_ShouldReturnExpectedSubset()
		{
			var buffer = Enumerable.Range(1, 10).ToArray();
			var result = ShuffleHelpers.ShuffleAndYield(buffer, new XorShiftRandom(), 5).ToArray();

			Assert.AreEqual(5, result.Length);
			CollectionAssert.IsSubsetOf(result, buffer);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield for a span returns a subset of the specified count, and all returned elements are from the
		/// original span.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_Span_ShouldReturnExpectedSubset()
		{
			var span = Enumerable.Range(1, 10).ToArray().AsSpan();
			var result = ShuffleHelpers.ShuffleAndYield<int>(span, new XorShiftRandom(), 4).ToArray();

			Assert.AreEqual(4, result.Length);
			CollectionAssert.IsSubsetOf(result, span.ToArray());
		}

		/// <summary>
		/// Verifies that ShuffleAndYield for a memory block returns a subset of the specified count, and all returned elements are from the
		/// original memory block.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_Memory_ShouldReturnExpectedSubset()
		{
			var memory = Enumerable.Range(1, 8).ToArray().AsMemory();
			var result = ShuffleHelpers.ShuffleAndYield(memory, new XorShiftRandom(), 3).ToArray();

			Assert.AreEqual(3, result.Length);
			CollectionAssert.IsSubsetOf(result, memory.ToArray());
		}

		/// <summary>
		/// Verifies that ShuffleAndYield for IEnumerable returns a subset of the specified count, and all returned elements are from the
		/// original source.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_IEnumerable_ShouldReturnExpectedSubset()
		{
			var source = Enumerable.Range(1, 10);
			var result = ShuffleHelpers.ShuffleAndYield(source, new XorShiftRandom(), 3).ToArray();

			Assert.AreEqual(3, result.Length);
			CollectionAssert.IsSubsetOf(result, source.ToArray());
		}

		/// <summary>
		/// Verifies that ShuffleAndYield returns an empty array when the input is empty or the count is zero.
		/// </summary>
		[DataTestMethod]
		[DataRow(0, 0)]
		[DataRow(3, 0)]
		public void ShuffleAndYield_WhenEmptyOrZeroCount_ShouldReturnEmpty(int bufferSize, int count)
		{
			var buffer = Enumerable.Range(1, bufferSize).ToArray();
			var result = ShuffleHelpers.ShuffleAndYield(buffer, new XorShiftRandom(), count).ToArray();

			if (count == 0 || bufferSize == 0)
				Assert.AreEqual(0, result.Length);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield throws an ArgumentOutOfRangeException when count is negative.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenCountNegative_ShouldThrow()
		{
			var buffer = new[] { 1, 2, 3 };
			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
				ShuffleHelpers.ShuffleAndYield(buffer, new XorShiftRandom(), -1).ToArray());
		}

		/// <summary>
		/// Verifies that ShuffleAndYield throws an ArgumentOutOfRangeException when count exceeds the number of elements in the input.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenCountExceedsLength_ShouldThrow()
		{
			var buffer = new[] { 1, 2, 3 };
			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
				ShuffleHelpers.ShuffleAndYield(buffer, new XorShiftRandom(), 4).ToArray());
		}

		/// <summary>
		/// Verifies that ShuffleAndYield for IEnumerable defers execution and does not enumerate the source until iterated.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_IEnumerable_ShouldDeferExecution()
		{
			AssertExecutionIsDeferred(
				methodName: "ShuffleAndYield",
				invokeExtensionMethod: src => ShuffleHelpers.ShuffleAndYield(src, new XorShiftRandom(), 2),
				values: new[] { 1, 2, 3 });
		}

		/// <summary>
		/// Verifies that ShuffleAndYield correctly handles input with duplicate values.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_ShouldWorkWithDuplicates()
		{
			var buffer = new[] { 5, 5, 5, 5, 5 };
			var result = ShuffleHelpers.ShuffleAndYield(buffer, new XorShiftRandom(), 3).ToArray();

			CollectionAssert.AreEqual(new[] { 5, 5, 5 }, result);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield does not modify the contents of the original array.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_ShouldNotMutateOriginalArray()
		{
			var original = Enumerable.Range(1, 10).ToArray();
			var copy = original.ToArray();
			Assert.AreNotSame(original, copy);

			_ = ShuffleHelpers.ShuffleAndYield(original, new XorShiftRandom(), 5).ToArray();
			CollectionAssert.AreEqual(copy, original);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield using a span does not mutate the underlying original array.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_Span_ShouldNotModifyOriginalSpan()
		{
			var original = Enumerable.Range(1, 10).ToArray();
			var copy = original.ToArray();

			_ = ShuffleHelpers.ShuffleAndYield<int>(original.AsSpan(), new XorShiftRandom(), 5).ToArray();

			CollectionAssert.AreEqual(copy, original, "Span-based shuffle should not mutate the original array.");
		}

		/// <summary>
		/// Verifies that when count equals array length, all unique items are returned in a different order.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_Array_WhenCountEqualsLength_ShouldReturnAllUniqueItems()
		{
			var buffer = Enumerable.Range(1, 10).ToArray();
			var result = ShuffleHelpers.ShuffleAndYield(buffer, new XorShiftRandom(), buffer.Length).ToArray();

			CollectionAssert.AreEquivalent(buffer, result);
			CollectionAssert.AllItemsAreUnique(result);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield for IEnumerable returns the expected count and all elements belong to the source.
		/// </summary>
		[DataTestMethod]
		[DataRow(0)]
		[DataRow(1)]
		[DataRow(5)]
		[DataRow(10)]
		public void ShuffleAndYield_IEnumerable_ShouldReturnExpectedCount(int count)
		{
			var source = Enumerable.Range(1, 10);
			var result = ShuffleHelpers.ShuffleAndYield(source, new XorShiftRandom(), count).ToArray();

			Assert.AreEqual(count, result.Length);
			CollectionAssert.IsSubsetOf(result, source.ToArray());
		}

		/// <summary>
		/// Verifies that ShuffleAndYield only begins enumeration when the result is iterated.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_IEnumerable_ShouldEnumerateOnIteration()
		{
			bool enumerated = false;

			var tracking = new TrackingEnumerable<int>(
				source: new[] { 1, 2, 3 },
				onEnumerate: () => enumerated = true
			);

			_ = ShuffleHelpers.ShuffleAndYield(tracking, new XorShiftRandom(), 2).ToArray();
			Assert.IsTrue(enumerated, "Source should be enumerated upon iteration.");
		}

		/// <summary>
		/// Verifies that ShuffleAndYield works correctly with complex reference types and preserves object references.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WithReferenceTypes_ShouldReturnExpectedSubset_UsingArray()
		{
			var source = Enumerable.Range(1, 10).Select(i => new Person { Id = i, Name = $"Person {i}" }).ToArray();
			var result = ShuffleHelpers.ShuffleAndYield(source, new XorShiftRandom(), 5).ToArray();

			Assert.AreEqual(5, result.Length);
			CollectionAssert.IsSubsetOf(result, source);

			// Ensure the references point to the original objects
			foreach (var person in result)
			{
				Assert.IsTrue(source.Contains(person));
			}
		}

		/// <summary>
		/// Verifies that ShuffleAndYield works correctly with complex reference types and preserves object references.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WithReferenceTypes_ShouldReturnExpectedSubset_UsingEnumerable()
		{
			var source = Enumerable.Range(1, 10).Select(i => new Person { Id = i, Name = $"Person {i}" }).AsEnumerable();
			var result = ShuffleHelpers.ShuffleAndYield(source, new XorShiftRandom(), 5).ToArray();

			Assert.AreEqual(5, result.Length);
			CollectionAssert.IsSubsetOf(result, source.ToArray());

			// Ensure the references point to the original objects
			foreach (var person in result)
			{
				Assert.IsTrue(source.Contains(person));
			}
		}

		/// <summary>
		/// Verifies that ShuffleAndYield works correctly with complex reference types and preserves object references.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WithReferenceTypes_ShouldReturnExpectedSubset_UsingSpan()
		{
			var source = Enumerable.Range(1, 10).Select(i => new Person { Id = i, Name = $"Person {i}" }).ToArray();
			var result = ShuffleHelpers.ShuffleAndYield<Person>(source.AsSpan(), new XorShiftRandom(), 5).ToArray();

			Assert.AreEqual(5, result.Length);
			CollectionAssert.IsSubsetOf(result, source.ToArray());

			// Ensure the references point to the original objects
			foreach (var person in result)
			{
				Assert.IsTrue(source.Contains(person));
			}
		}

		/// <summary>
		/// Verifies that ShuffleAndYield works correctly with complex reference types and preserves object references.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WithReferenceTypes_ShouldReturnExpectedSubset_UsingMemory()
		{
			var source = Enumerable.Range(1, 10).Select(i => new Person { Id = i, Name = $"Person {i}" }).ToArray();
			var result = ShuffleHelpers.ShuffleAndYield<Person>(source.AsMemory(), new XorShiftRandom(), 5).ToArray();

			Assert.AreEqual(5, result.Length);
			CollectionAssert.IsSubsetOf(result, source.ToArray());

			// Ensure the references point to the original objects
			foreach (var person in result)
			{
				Assert.IsTrue(source.Contains(person));
			}
		}

		/// <summary>
		/// Runs 20,000 shuffles using ShuffleAndYield of a 10-element array to validate statistical uniformity of output positions. Each
		/// value should appear roughly equally in each position, with no more than 2 statistically significant outliers.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenRepeated_ShouldDistributeItemsStatistically()
		{
			const int runs = 20000;
			const int size = 10;
			var tracker = new int[size, size];
			var original = Enumerable.Range(0, size).ToArray();

			for (int r = 0; r < runs; r++)
			{
				var shuffled = ShuffleHelpers.ShuffleAndYield(original, new SystemRandomAdapter(), size).ToArray();
				for (int i = 0; i < size; i++)
					tracker[i, shuffled[i]]++;
			}

			AssertStatisticalUniformity(tracker, size, label: nameof(ShuffleHelpers.ShuffleAndYield));
		}

		public class Person
		{
			public int Id { get; set; }

			public string Name { get; set; }

			public override bool Equals(object obj) => obj is Person other && Id == other.Id;

			public override int GetHashCode() => Id.GetHashCode();
		}
	}
}