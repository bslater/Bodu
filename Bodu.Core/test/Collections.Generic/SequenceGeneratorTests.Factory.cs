﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Collections.Generic
{
	public partial class SequenceGeneratorTests
	{
		/// <summary>
		/// Verifies that Factory returns the sequence produced by the supplied enumerator.
		/// </summary>
		[TestMethod]
		public void Factory_WhenEnumeratorIsValid_ShouldReturnExpectedSequence()
		{
			var result = SequenceGenerator.Factory(() => new List<int> { 1, 2, 3 }.GetEnumerator()).ToArray();
			CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
		}

		/// <summary>
		/// Verifies that Factory throws an ArgumentNullException when the enumerator factory is null.
		/// </summary>
		[TestMethod]
		public void Factory_WhenFactoryIsNull_ShouldThrow()
		{
			Assert.ThrowsException<ArgumentNullException>(() =>
			{
				_ = SequenceGenerator.Factory<int>(null!).ToArray();
			});
		}

		/// <summary>
		/// Verifies that Factory defers enumeration until the resulting sequence is explicitly consumed.
		/// </summary>
		[TestMethod]
		public void Factory_WhenCalled_ShouldDeferExecution()
		{
			var source = new List<int> { 1, 2, 3 };
			AssertExecutionIsDeferred("Factory", _ => SequenceGenerator.Factory(() => source.GetEnumerator()), source);
		}
	}
}