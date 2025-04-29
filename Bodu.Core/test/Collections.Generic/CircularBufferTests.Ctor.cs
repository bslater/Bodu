namespace Bodu.Collections.Generic
{
	public partial class CircularBufferTests
	{
		/// <summary>
		/// Verifies that the default constructor initializes an empty buffer with the default capacity and overwrite enabled.
		/// </summary>
		[TestMethod]
		public void Ctor_WhenDefaultUsed_ShouldInitializeWithDefaultCapacityAndAllowOverwrite()
		{
			var buffer = new CircularBuffer<int>();
			Assert.AreEqual(DefaultCapacity, buffer.Capacity);
			Assert.AreEqual(0, buffer.Count);
			Assert.IsTrue(buffer.AllowOverwrite);
		}

		/// <summary>
		/// Verifies that specifying capacity uses it and defaults AllowOverwrite to true.
		/// </summary>
		[TestMethod]
		public void Ctor_WhenCapacityProvided_ShouldUseCapacityAndDefaultAllowOverwrite()
		{
			var buffer = new CircularBuffer<int>(5);
			Assert.AreEqual(5, buffer.Capacity);
			Assert.AreEqual(0, buffer.Count);
			Assert.IsTrue(buffer.AllowOverwrite);
		}

		/// <summary>
		/// Verifies that AllowOverwrite is set correctly when passed to the constructor.
		/// </summary>
		[TestMethod]
		public void Ctor_WhenAllowOverwriteFalseProvided_ShouldRespectFlag()
		{
			var buffer = new CircularBuffer<string>(5, false);
			Assert.AreEqual(5, buffer.Capacity);
			Assert.IsFalse(buffer.AllowOverwrite);
		}

		/// <summary>
		/// Verifies that initializing from a collection loads its contents and uses default capacity.
		/// </summary>
		[TestMethod]
		public void Ctor_WhenCollectionProvided_ShouldAdoptContentsAndUseDefaultCapacity()
		{
			var source = new[] { "a", "b", "c" };
			var buffer = new CircularBuffer<string>(source);

			CollectionAssert.AreEqual(source, buffer.ToArray());
			Assert.AreEqual(DefaultCapacity, buffer.Capacity);
			Assert.AreEqual(source.Length, buffer.Count);
			Assert.IsTrue(buffer.AllowOverwrite);
		}

		/// <summary>
		/// Verifies that capacity limits trim the contents of a supplied collection.
		/// </summary>
		[TestMethod]
		public void Ctor_WhenCollectionAndLimitedCapacityProvided_ShouldTrimOlderItems()
		{
			var source = new[] { 1, 2, 3, 4 };
			var buffer = new CircularBuffer<int>(source, 2);

			CollectionAssert.AreEqual(new[] { 3, 4 }, buffer.ToArray());
			Assert.AreEqual(2, buffer.Capacity);
		}

		/// <summary>
		/// Verifies that all constructor parameters are respected when used together.
		/// </summary>
		[TestMethod]
		public void Ctor_WhenCollectionCapacityAndOverwriteFlagProvided_ShouldRespectAllParameters()
		{
			var source = new[] { 1, 2, 3 };
			var buffer = new CircularBuffer<int>(source, 3, false);

			CollectionAssert.AreEqual(source, buffer.ToArray());
			Assert.AreEqual(3, buffer.Capacity);
			Assert.IsFalse(buffer.AllowOverwrite);
		}

		/// <summary>
		/// Verifies that passing null as the source collection throws an ArgumentNullException.
		/// </summary>
		[TestMethod]
		public void Ctor_WhenCollectionIsNull_ShouldThrowExactly()
		{
			Assert.ThrowsExactly<ArgumentNullException>(() =>
			{
				_ = new CircularBuffer<object>((IEnumerable<object>)null);
			});
		}

		/// <summary>
		/// Verifies that specifying a negative capacity throws an ArgumentOutOfRangeException.
		/// </summary>
		[DataTestMethod]
		[DataRow(-1)]
		[DataRow(0)]
		public void Ctor_WhenCapacityIsInvalid_ShouldThrowExactly(int size)
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = new CircularBuffer<int>(size);
			});
		}

		/// <summary>
		/// Verifies that initializing from a collection that exceeds capacity with overwrite disabled throws.
		/// </summary>
		[TestMethod]
		public void Ctor_WhenCollectionExceedsCapacityAndOverwriteIsFalse_ShouldThrowExactly()
		{
			var source = new[] { 1, 2, 3 };

			Assert.ThrowsExactly<InvalidOperationException>(() =>
			{
				_ = new CircularBuffer<int>(source, 2, false);
			});
		}

		/// <summary>
		/// Verifies that a negative capacity with an empty collection throws ArgumentOutOfRangeException.
		/// </summary>
		[TestMethod]
		public void Ctor_WhenEmptyCollectionAndNegativeCapacityProvided_ShouldThrowExactly()
		{
			var empty = Array.Empty<string>();

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = new CircularBuffer<string>(empty, -1);
			});
		}

		/// <summary>
		/// Verifies that negative capacity and overwrite flag throw even with empty collection.
		/// </summary>
		[TestMethod]
		public void Ctor_WhenEmptyCollectionNegativeCapacityAndOverwriteProvided_ShouldThrowExactly()
		{
			var empty = Array.Empty<string>();

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = new CircularBuffer<string>(empty, -10, allowOverwrite: true);
			});
		}

		/// <summary>
		/// Verifies that a non-array enumerable source is consumed using ToArray.
		/// </summary>
		[TestMethod]
		public void Ctor_WhenNonArrayEnumerableProvided_ShouldConsumeUsingToArray()
		{
			IEnumerable<int> source = Enumerable.Range(1, 3).Select(x => x * 10);
			var buffer = new CircularBuffer<int>(source, 5);

			CollectionAssert.AreEqual(new[] { 10, 20, 30 }, buffer.ToArray());
			Assert.AreEqual(3, buffer.Count);
		}

		/// <summary>
		/// Verifies that array sources are consumed directly without unnecessary copying.
		/// </summary>
		[TestMethod]
		public void Ctor_WhenArrayProvided_ShouldUseDirectReference()
		{
			int[] source = { 1, 2, 3 };
			var buffer = new CircularBuffer<int>(source, 5);

			CollectionAssert.AreEqual(source, buffer.ToArray());
			Assert.AreEqual(3, buffer.Count);
		}
	}
}