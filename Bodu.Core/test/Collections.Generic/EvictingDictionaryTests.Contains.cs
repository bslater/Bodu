namespace Bodu.Collections.Generic
{
	public partial class EvictingDictionaryTests
	{
		/// <summary>
		/// Verifies that Contains returns true when both the key and value exist in the dictionary.
		/// </summary>
		[TestMethod]
		public void Contains_WhenKeyAndValueExist_ShouldReturnTrue()
		{
			var dictionary = new EvictingDictionary<string, int>(3);
			dictionary.Add("X", 100);

			Assert.IsTrue(dictionary.Contains(new KeyValuePair<string, int>("X", 100)));
		}

		/// <summary>
		/// Verifies that Contains returns false when the key exists but the value does not match.
		/// </summary>
		[TestMethod]
		public void Contains_WhenKeyExistsButValueDiffers_ShouldReturnFalse()
		{
			var dictionary = new EvictingDictionary<string, int>(3);
			dictionary.Add("X", 100);

			Assert.IsFalse(dictionary.Contains(new KeyValuePair<string, int>("X", 999)));
		}

		/// <summary>
		/// Verifies that Contains returns false when the key is not in the dictionary.
		/// </summary>
		[TestMethod]
		public void Contains_WhenKeyDoesNotExist_ShouldReturnFalse()
		{
			var dictionary = new EvictingDictionary<string, int>(3);

			Assert.IsFalse(dictionary.Contains(new KeyValuePair<string, int>("Missing", 10)));
		}

		/// <summary>
		/// Verifies that Contains returns false when using a default struct key that is not present.
		/// </summary>
		[TestMethod]
		public void Contains_WhenKeyIsDefaultStructAndNotPresent_ShouldReturnFalse()
		{
			var dictionary = new EvictingDictionary<int, string>(3);

			Assert.IsFalse(dictionary.Contains(new KeyValuePair<int, string>(0, "something")));
		}

		/// <summary>
		/// Verifies that Contains returns true when key and value match using a case-insensitive comparer.
		/// </summary>
		[TestMethod]
		public void Contains_WhenCaseInsensitiveComparerAndKeyValueMatch_ShouldReturnTrue()
		{
			var dictionary = new EvictingDictionary<string, int>(
				5,
				new Dictionary<string, int> { { "Alpha", 42 } },
				EvictingDictionaryPolicy.LeastRecentlyUsed,
				StringComparer.InvariantCultureIgnoreCase);

			Assert.IsTrue(dictionary.Contains(new KeyValuePair<string, int>("alpha", 42)));
		}

		/// <summary>
		/// Verifies that Contains returns false when value differs even if the key matches case-insensitively.
		/// </summary>
		[TestMethod]
		public void Contains_WhenCaseInsensitiveComparerAndValueDiffers_ShouldReturnFalse()
		{
			var dictionary = new EvictingDictionary<string, int>(
				5,
				new Dictionary<string, int> { { "Alpha", 42 } },
				EvictingDictionaryPolicy.LeastRecentlyUsed,
				StringComparer.InvariantCultureIgnoreCase);

			Assert.IsFalse(dictionary.Contains(new KeyValuePair<string, int>("alpha", 100)));
		}

		/// <summary>
		/// Verifies that Contains returns false when the key does not exist even with a case-insensitive comparer.
		/// </summary>
		[TestMethod]
		public void Contains_WhenCaseInsensitiveComparerAndKeyNotFound_ShouldReturnFalse()
		{
			var dictionary = new EvictingDictionary<string, int>(
				5,
				new Dictionary<string, int> { { "Beta", 99 } },
				EvictingDictionaryPolicy.LeastRecentlyUsed,
				StringComparer.OrdinalIgnoreCase);

			Assert.IsFalse(dictionary.Contains(new KeyValuePair<string, int>("Gamma", 99)));
		}

		/// <summary>
		/// Verifies that Contains throws ArgumentNullException when the key is null in the KeyValuePair.
		/// </summary>
		[TestMethod]
		public void Contains_WhenKeyIsNull_ShouldThrowExactly()
		{
			var dictionary = new EvictingDictionary<string, int>(3);
			var kvp = new KeyValuePair<string, int>(null!, 42);

			Assert.ThrowsExactly<ArgumentNullException>(() =>
			{
				_ = dictionary.Contains(kvp);
			});
		}

		/// <summary>
		/// Verifies that Contains returns true when key and value match exactly.
		/// </summary>
		[TestMethod]
		public void Contains_WhenExactKeyAndValueMatch_ShouldReturnTrue()
		{
			var dictionary = new EvictingDictionary<string, int>(3);
			dictionary.Add("alpha", 42);

			Assert.IsTrue(dictionary.Contains(new KeyValuePair<string, int>("alpha", 42)));
		}

		/// <summary>
		/// Verifies that Contains returns false when the value differs even though the key matches.
		/// </summary>
		[TestMethod]
		public void Contains_WhenValueDiffersButKeyMatches_ShouldReturnFalse()
		{
			var dictionary = new EvictingDictionary<string, int>(3);
			dictionary.Add("alpha", 42);

			Assert.IsFalse(dictionary.Contains(new KeyValuePair<string, int>("alpha", 99)));
		}

		/// <summary>
		/// Verifies that Contains returns false when the key is missing from the dictionary.
		/// </summary>
		[TestMethod]
		public void Contains_WhenKeyIsMissing_ShouldReturnFalse()
		{
			var dictionary = new EvictingDictionary<string, int>(3);

			Assert.IsFalse(dictionary.Contains(new KeyValuePair<string, int>("alpha", 42)));
		}

		/// <summary>
		/// Verifies that Contains respects a custom key equality comparer.
		/// </summary>
		[TestMethod]
		public void Contains_WhenUsingCustomEqualityComparer_ShouldRespectComparer()
		{
			var dictionary = new EvictingDictionary<string, string>(3, StringComparer.OrdinalIgnoreCase);
			dictionary.Add("KEY", "Value");

			Assert.IsTrue(dictionary.ContainsKey("key"));
			Assert.IsTrue(dictionary.Contains(new KeyValuePair<string, string>("key", "Value")));
		}

		/// <summary>
		/// Verifies that Contains returns false when the value differs despite custom comparer matching the key.
		/// </summary>
		[TestMethod]
		public void Contains_WhenUsingCustomComparerAndValueDiffers_ShouldReturnFalse()
		{
			var dictionary = new EvictingDictionary<string, string>(3, StringComparer.OrdinalIgnoreCase);
			dictionary.Add("KEY", "Value");

			Assert.IsFalse(dictionary.Contains(new KeyValuePair<string, string>("key", "OtherValue")));
		}
	}
}