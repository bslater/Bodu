namespace Bodu.Collections.Extensions
{
	[TestClass]
	public partial class IEnumerableExtensionsTests_RecursiveSelect
		: Bodu.Collections.EnumerableTests
	{
		public static IEnumerable<object[]> GetDeferredExecutionCases()
			=> GetCases().Select(tc => new object[] { tc });

		/// <summary>
		/// Provides extension method test cases that should exhibit deferred execution.
		/// </summary>
		public static IEnumerable<EnumerableTestPlan<Node, string>> GetCases()
		{
			yield return new EnumerableTestPlan<Node, string>
			{
				Name = "RecursiveSelect",
				Source = NodeSampleTree.BuildSampleTree(),
				Invoke = source => IEnumerableExtensions.RecursiveSelect(
					source: source,
					childSelector: e => (e as Node).Children
				),
				Selector = node => node.Name,
				ExpectedResult = new[] { "Root", "A", "B", "B1", "B2", "C", "C1", "C1A", "C2", "C2A", "C2B", "C2C", "D", "E" },
			};

			yield return new EnumerableTestPlan<Node, string>
			{
				Name = "RecursiveSelect with Index Selector",
				Source = NodeSampleTree.BuildSampleTree(),
				Invoke = source => IEnumerableExtensions.RecursiveSelect(
					source: source,
					childSelector: e => (e as Node).Children,
					selector: (e, i) => $"{i}:{(e as Node).Name}"
				),
				ExpectedResult = new[] { "0:Root", "0:A", "1:B", "0:B1", "1:B2", "2:C", "0:C1", "0:C1A", "1:C2", "0:C2A", "1:C2B", "2:C2C", "3:D", "4:E" },
			};

			yield return new EnumerableTestPlan<Node, string>
			{
				Name = "RecursiveSelect with Index and Depth Selector",
				Source = NodeSampleTree.BuildSampleTree(),
				Invoke = source => IEnumerableExtensions.RecursiveSelect(
					source: source,
					childSelector: e => (e as Node).Children,
					selector: (e, i, d) => $"{new string('-', d)}{i}:{(e as Node).Name}"
				),
				ExpectedResult = new[] { "0:Root", "-0:A", "-1:B", "--0:B1", "--1:B2", "-2:C", "--0:C1", "---0:C1A", "--1:C2", "---0:C2A", "---1:C2B", "---2:C2C", "-3:D", "-4:E" },
			};

			yield return new EnumerableTestPlan<Node, string>
			{
				Name = "RecursiveSelect with Index and Depth Selector and Control = YieldAndBreak",
				Source = NodeSampleTree.BuildSampleTree(),
				Invoke = source => IEnumerableExtensions.RecursiveSelect(
					source: source,
					childSelector: e => (e as Node).Children,
					selector: (e, i, d) => $"{new string('-', d)}{i}:{(e as Node).Name}",
					e => (e as Node).Stop ? RecursiveSelectControl.YieldAndExit : RecursiveSelectControl.YieldAndRecurse
				),
				ExpectedResult = new[] { "0:Root", "-0:A", "-1:B", "--0:B1", },
			};

			yield return new EnumerableTestPlan<Node, string>
			{
				Name = "RecursiveSelect with Index and Depth Selector and Control = SkipOnly",
				Source = NodeSampleTree.BuildSampleTree(),
				Invoke = source => IEnumerableExtensions.RecursiveSelect(
					source: source,
					childSelector: e => (e as Node).Children,
					selector: (e, i, d) => $"{new string('-', d)}{i}:{(e as Node).Name}",
					e => (e as Node).Stop ? RecursiveSelectControl.SkipOnly : RecursiveSelectControl.YieldAndRecurse
				),
				ExpectedResult = new[] { "0:Root", "-0:A", "-1:B", "--1:B2", "-2:C", "--1:C2", "---0:C2A", "---2:C2C", "-3:D" },
			};

			yield return new EnumerableTestPlan<Node, string>
			{
				Name = "RecursiveSelect with Index and Depth Selector and Control = SkipAndRecurse",
				Source = NodeSampleTree.BuildSampleTree(),
				Invoke = source => IEnumerableExtensions.RecursiveSelect(
					source: source,
					childSelector: e => (e as Node).Children,
					selector: (e, i, d) => $"{new string('-', d)}{i}:{(e as Node).Name}",
					e => RecursiveSelectControl.SkipAndRecurse
				),
				ExpectedResult = Array.Empty<string>(),
			};

			yield return new EnumerableTestPlan<Node, string>
			{
				Name = "RecursiveSelect with Index and Depth Selector and Control = YieldAndExit",
				Source = NodeSampleTree.BuildSampleTree(),
				Invoke = source => IEnumerableExtensions.RecursiveSelect(
					source: source,
					childSelector: e => (e as Node).Children,
					selector: (e, i, d) => $"{new string('-', d)}{i}:{(e as Node).Name}",
					e => RecursiveSelectControl.YieldAndExit
				),
				ExpectedResult = new[] { "0:Root" },
			};

			yield return new EnumerableTestPlan<Node, string>
			{
				Name = "RecursiveSelect with Index and Depth Selector and Control = SkipAndBreak",
				Source = NodeSampleTree.BuildSampleTree(),
				Invoke = source => IEnumerableExtensions.RecursiveSelect(
					source: source,
					childSelector: e => (e as Node).Children,
					selector: (e, i, d) => $"{new string('-', d)}{i}:{(e as Node).Name}",
					e => RecursiveSelectControl.SkipAndBreak
				),
				ExpectedResult = Array.Empty<string>(),
			};

			yield return new EnumerableTestPlan<Node, string>
			{
				Name = "RecursiveSelect with Index and Depth Selector and Control = YieldAndRecurse on 'C'",
				Source = NodeSampleTree.BuildSampleTree(),
				Invoke = source => IEnumerableExtensions.RecursiveSelect(
					source: source,
					childSelector: e => (e as Node).Children,
					selector: (e, i, d) => $"{new string('-', d)}{i}:{(e as Node).Name}",
					e => (e as Node).Name[0] == 'C' ? RecursiveSelectControl.YieldAndRecurse : RecursiveSelectControl.SkipAndRecurse
				),
				ExpectedResult = new[] { "-2:C", "--0:C1", "---0:C1A", "--1:C2", "---0:C2A", "---1:C2B", "---2:C2C" },
			};

			yield return new EnumerableTestPlan<Node, string>
			{
				Name = "RecursiveSelect with Control = YieldOnly for Stop nodes",
				Source = NodeSampleTree.BuildSampleTree(),
				Invoke = source => IEnumerableExtensions.RecursiveSelect(
					source: source,
					childSelector: e => (e as Node).Children,
					selector: (e, i, d) => $"{new string('-', d)}{i}:{(e as Node).Name}",
					recursionControl: e => (e as Node).Stop ? RecursiveSelectControl.YieldOnly : RecursiveSelectControl.YieldAndRecurse
				),

				// C1A is excluded since C1 is Stop = true and marked YieldOnly (no recurse)
				ExpectedResult = new[] { "0:Root", "-0:A", "-1:B", "--0:B1", "--1:B2", "-2:C", "--0:C1", "--1:C2", "---0:C2A", "---1:C2B", "---2:C2C", "-3:D", "-4:E" }
			};

			yield return new EnumerableTestPlan<Node, string>
			{
				Name = "RecursiveSelect with Control = RecurseOnly on Root",
				Source = NodeSampleTree.BuildSampleTree(),
				Invoke = source => IEnumerableExtensions.RecursiveSelect(
					source: source,
					childSelector: e => (e as Node).Children,
					selector: (e, i, d) => $"{new string('-', d)}{i}:{(e as Node).Name}",
					recursionControl: e => (e as Node).Name == "Root"
						? RecursiveSelectControl.RecurseOnly
						: RecursiveSelectControl.YieldAndRecurse
				),
				ExpectedResult = new[] { "-0:A", "-1:B", "--0:B1", "--1:B2", "-2:C", "--0:C1", "---0:C1A", "--1:C2", "---0:C2A", "---1:C2B", "---2:C2C", "-3:D", "-4:E" }
			};

			yield return new EnumerableTestPlan<Node, string>
			{
				Name = "RecursiveSelect with Control = SkipAndExit on Stop",
				Source = NodeSampleTree.BuildSampleTree(),
				Invoke = source => IEnumerableExtensions.RecursiveSelect(
					source: source,
					childSelector: e => (e as Node).Children,
					selector: (e, i, d) => $"{new string('-', d)}{i}:{(e as Node).Name}",
					recursionControl: e => (e as Node).Stop
						? RecursiveSelectControl.SkipAndExit
						: RecursiveSelectControl.YieldAndRecurse
				),

				// B1 is Stop = true → SkipAndExit triggers full stop
				ExpectedResult = new[] { "0:Root", "-0:A", "-1:B" }
			};

			yield return new EnumerableTestPlan<Node, string>
			{
				Name = "RecursiveSelect with complex control logic per node name",
				Source = NodeSampleTree.BuildSampleTree(),
				Invoke = source => IEnumerableExtensions.RecursiveSelect(
					source: source,
					childSelector: e => (e as Node).Children,
					selector: (e, i, d) => $"{new string('-', d)}{i}:{(e as Node).Name}",
					recursionControl: e => (e as Node).Name switch
					{
						"Root" => RecursiveSelectControl.YieldAndRecurse,
						"A" => RecursiveSelectControl.YieldOnly,
						"B" => RecursiveSelectControl.RecurseOnly,
						"B1" => RecursiveSelectControl.YieldAndExit,
						_ => RecursiveSelectControl.YieldAndRecurse
					}
				),

				// Skips over B but recurses B, nd exits immediately on B1
				ExpectedResult = new[] { "0:Root", "-0:A", "--0:B1" }
			};

			yield return new EnumerableTestPlan<Node, string>
			{
				Name = "RecursiveSelect skipping only C2B",
				Source = NodeSampleTree.BuildSampleTree(),
				Invoke = source => IEnumerableExtensions.RecursiveSelect(
					source: source,
					childSelector: e => (e as Node).Children,
					selector: (e, i, d) => $"{new string('-', d)}{i}:{(e as Node).Name}",
					recursionControl: e => (e as Node).Name == "C2B"
						? RecursiveSelectControl.SkipOnly
						: RecursiveSelectControl.YieldAndRecurse
				),
				ExpectedResult = new[] { "0:Root", "-0:A", "-1:B", "--0:B1", "--1:B2", "-2:C", "--0:C1", "---0:C1A", "--1:C2", "---0:C2A", "---2:C2C", "-3:D", "-4:E" }
			};
		}

		/// <summary>
		/// Verifies that the method under test does not trigger enumeration until it is explicitly enumerated.
		/// </summary>
		[TestMethod]
		[DynamicData(nameof(GetDeferredExecutionCases), DynamicDataSourceType.Method)]
		public void RecursiveSelect_WhenCalled_ShouldDeferExecution(EnumerableTestPlan<Node, string> testCase)
		{
			AssertExecutionIsDeferred(testCase.Name, testCase.Invoke, testCase.Source);
		}

		/// <summary>
		/// Verifies that the method under test triggers enumeration when explicitly iterated.
		/// </summary>
		[TestMethod]
		[DynamicData(nameof(GetDeferredExecutionCases), DynamicDataSourceType.Method)]
		public void RecursiveSelect_WhenEnumerated_ShouldTriggerExecution(EnumerableTestPlan<Node, string> testCase)
		{
			AssertExecutionOccursOnEnumeration(testCase.Name, testCase.Invoke, testCase.Source);
		}

		/// <summary>
		/// Verifies that the method under test triggers enumeration when explicitly iterated.
		/// </summary>
		[TestMethod]
		[DynamicData(nameof(GetDeferredExecutionCases), DynamicDataSourceType.Method)]
		public void RecursiveSelect_WhenEnumerated_ShouldReturnExpectedResults(EnumerableTestPlan<Node, string> testCase)
		{
			AssertExecutionReturnsExpectedResults(testCase.Name, testCase.Invoke, testCase.Source, testCase.ExpectedResult, testCase.Selector);
		}

		/// <summary>
		/// Verifies that calling RecursiveSelect throws <see cref="ArgumentNullException" /> when the source sequence is <c>null</c>.
		/// </summary>
		[TestMethod]
		public void RecursiveSelect_WhenSourceIsNull_ShouldThrowExactly()
		{
			IEnumerable<object>? source = null!;
			Assert.ThrowsExactly<ArgumentNullException>(
				() => source.RecursiveSelect(_ => Enumerable.Empty<object>()).Cast<object>().ToList()
			);
		}

		/// <summary>
		/// Verifies that calling RecursiveSelect throws <see cref="ArgumentNullException" /> when the child selector delegate is <c>null</c>.
		/// </summary>
		[TestMethod]
		public void RecursiveSelect_WhenChildSelectorIsNull_ShouldThrowExactly()
		{
			var source = new[] { new object() };
			Func<object, IEnumerable<object>>? childSelector = null!;
			Assert.ThrowsExactly<ArgumentNullException>(
				() => source.RecursiveSelect(childSelector).Cast<object>().ToList())
			;
		}

		/// <summary>
		/// Verifies that RecursiveSelect throws <see cref="ArgumentNullException" /> when the element selector delegate is <c>null</c> in
		/// the two-parameter overload.
		/// </summary>
		[TestMethod]
		public void RecursiveSelect_WithSelector_WhenSelectorIsNull_ShouldThrowExactly()
		{
			var source = new[] { new object() };
			Assert.ThrowsExactly<ArgumentNullException>(
				() => source.RecursiveSelect(
					childSelector: _ => Enumerable.Empty<object>(),
					selector: (Func<object, object>)null!
				).Cast<object>().ToList()
			);
		}

		/// <summary>
		/// Verifies that RecursiveSelect throws <see cref="ArgumentNullException" /> when the indexed selector delegate is <c>null</c>.
		/// </summary>
		[TestMethod]
		public void RecursiveSelect_WithIndexSelector_WhenSelectorIsNull_ShouldThrowExactly()
		{
			var source = new[] { new object() };
			Assert.ThrowsExactly<ArgumentNullException>(
				() => source.RecursiveSelect(
					childSelector: _ => Enumerable.Empty<object>(),
					selector: (Func<object, int, object>)null!
				).Cast<object>().ToList()
			);
		}

		/// <summary>
		/// Verifies that RecursiveSelect throws <see cref="ArgumentNullException" /> when the depth-aware selector delegate is <c>null</c>.
		/// </summary>
		[TestMethod]
		public void RecursiveSelect_WithIndexAndDepthSelector_WhenSelectorIsNull_ShouldThrowExactly()
		{
			var source = new[] { new object() };
			Assert.ThrowsExactly<ArgumentNullException>(
				() => source.RecursiveSelect(
					childSelector: _ => Enumerable.Empty<object>(),
					selector: (Func<object, int, int, object>)null!
				).Cast<object>().ToList()
			);
		}

		/// <summary>
		/// Verifies that RecursiveSelect throws <see cref="ArgumentNullException" /> when the recursion control delegate is supplied but
		/// the selector is <c>null</c>.
		/// </summary>
		[TestMethod]
		public void RecursiveSelect_WithRecursionControl_WhenSelectorIsNull_ShouldThrowExactly()
		{
			var source = new[] { new object() };
			Assert.ThrowsExactly<ArgumentNullException>(
				() => source.RecursiveSelect(
					childSelector: _ => Enumerable.Empty<object>(),
					selector: (Func<object, int, int, object>)null!,
					recursionControl: _ => RecursiveSelectControl.YieldAndRecurse
				).Cast<object>().ToList()
			);
		}

		/// <summary>
		/// Verifies that RecursiveSelect throws <see cref="ArgumentNullException" /> when the recursion control delegate is <c>null</c>.
		/// </summary>
		[TestMethod]
		public void RecursiveSelect_WithRecursionControl_WhenRecursionControlIsNull_ShouldThrowExactly()
		{
			var source = new[] { new object() };
			Assert.ThrowsExactly<ArgumentNullException>(
				() => source.RecursiveSelect(
					childSelector: _ => Enumerable.Empty<object>(),
					selector: (e, i, d) => e,
					recursionControl: (Func<object, RecursiveSelectControl>)null!
				).Cast<object>().ToList()
			);
		}

		/// <summary>
		/// Ensures RecursiveSelect works with an empty source collection and produces no output.
		/// </summary>
		[TestMethod]
		public void RecursiveSelect_WithEmptySource_ShouldReturnEmpty()
		{
			object[] source = Array.Empty<object>();
			var result = source.RecursiveSelect(n => Array.Empty<object>()).Cast<object>().ToList();
			Assert.AreEqual(0, result.Count);
		}

		/// <summary>
		/// Ensures RecursiveSelect works when some childSelector calls return null (graceful fallback).
		/// </summary>
		[TestMethod]
		public void ChildSelector_ReturnsNull_ShouldSkipChildren()
		{
			var root = new Node { Name = "Root", Children = null! };
			var result = new object[] { root }.RecursiveSelect(n => (n as Node).Children).Cast<Node>().ToList();

			Assert.AreEqual(1, result.Count);
			Assert.AreEqual("Root", (result[0] as Node).Name);
		}

		/// <summary>
		/// Ensures that exceptions thrown by the selector are propagated correctly.
		/// </summary>
		[TestMethod]
		public void Selector_ThrowsException_ShouldBubbleUp()
		{
			var tree = new object[] { NodeSampleTree.BuildSampleTree() };

			Assert.ThrowsExactly<InvalidOperationException>(() =>
			{
				tree.RecursiveSelect(
					n => (n as Node).Children,
					n => throw new InvalidOperationException("Test failure")
				).Cast<Node>().ToList();
			});
		}
	}
}