using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Collections.Generic
{
	public partial class SequenceGeneratorTests
	{
		/// <summary>
		/// Verifies that NextWhile applies indexed transformations until the condition fails.
		/// </summary>
		[TestMethod]
		public void NextWhile_WhenUsingIndexedTransform_ShouldReturnExpectedSequence()
		{
			var result = SequenceGenerator.NextWhile(
				0,
				x => x < 5,
				(x, _) => x + 1).ToArray();

			CollectionAssert.AreEqual(new[] { 0, 1, 2, 3, 4 }, result);
		}

		/// <summary>
		/// Verifies that NextWhile performs repeated transformation using a simple update function.
		/// </summary>
		[TestMethod]
		public void NextWhile_WhenUsingSimpleTransform_ShouldReturnExpectedSequence()
		{
			var result = SequenceGenerator.NextWhile(
				1,
				x => x <= 8,
				x => x * 2).ToArray();

			CollectionAssert.AreEqual(new[] { 1, 2, 4, 8 }, result);
		}

		/// <summary>
		/// Verifies that NextWhile using a state object returns the correct projection values.
		/// </summary>
		[TestMethod]
		public void NextWhile_WhenUsingStateObject_ShouldReturnProjectedSequence()
		{
			var result = SequenceGenerator.NextWhile(
				new { A = 1, B = 1 },
				state => state.B < 8,
				state => new { A = state.B, B = state.A + state.B },
				state => state.A).ToArray();

			CollectionAssert.AreEqual(new[] { 1, 1, 2, 3 }, result);
		}

		/// <summary>
		/// Verifies that NextWhile throws when the result selector is null.
		/// </summary>
		[TestMethod]
		public void NextWhile_WhenResultSelectorIsNull_ShouldThrow()
		{
			Assert.ThrowsException<ArgumentNullException>(() =>
			{
				SequenceGenerator.NextWhile(0, x => true, (Func<int, int>)null!).ToArray();
			});
		}

		/// <summary>
		/// Verifies that NextWhile returns an empty sequence if the condition fails immediately.
		/// </summary>
		[TestMethod]
		public void NextWhile_WhenInitialConditionIsFalse_ShouldReturnEmptySequence()
		{
			var result = SequenceGenerator.NextWhile(
				5,
				x => false,
				x => x + 1).ToArray();

			Assert.AreEqual(0, result.Length);
		}

		/// <summary>
		/// Verifies that NextWhile throws when the iterate function is null for a stateful generator.
		/// </summary>
		[TestMethod]
		public void NextWhile_WhenIterateFunctionIsNull_ShouldThrow()
		{
			Assert.ThrowsException<ArgumentNullException>(() =>
			{
				SequenceGenerator.NextWhile(new { X = 0 }, x => true, null!, x => x.X).ToArray();
			});
		}
	}
}