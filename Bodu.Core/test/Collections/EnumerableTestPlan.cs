using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Collections
{
	/// <summary>
	/// Represents a typed test plan for recursive or iterator-based methods.
	/// </summary>
	/// <typeparam name="TSource">The input sequence element type.</typeparam>
	/// <typeparam name="TResult">The result type after transformation.</typeparam>
	public class EnumerableTestPlan<TSource, TResult>
	{
		public string Name { get; init; } = string.Empty;

		/// <summary>
		/// The strongly-typed source input sequence.
		/// </summary>
		public IEnumerable<TSource> Source { get; init; } = null!;

		/// <summary>
		/// The strongly-typed selector used to transform results into comparable values.
		/// </summary>
		public Func<TSource, TResult> Selector { get; init; } = null!;

		/// <summary>
		/// The strongly-typed transformation or query logic to test.
		/// </summary>
		public Func<IEnumerable<TSource>, IEnumerable> Invoke { get; init; } = null!;

		/// <summary>
		/// The expected transformed result.
		/// </summary>
		public IEnumerable<TResult> ExpectedResult { get; init; } = null!;

		public override string ToString() => Name;
	}
}