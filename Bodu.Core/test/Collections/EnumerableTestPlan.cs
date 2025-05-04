using System.Collections;

namespace Bodu.Collections
{
	/// <summary>
	/// Represents a typed test plan for recursive or iterator-based methods.
	/// </summary>
	/// <typeparam name="TSource">The input sequence element type.</typeparam>
	/// <typeparam name="TResult">The result type after transformation.</typeparam>
	public class EnumerableTestPlan<TSource>
	{
		public string Name { get; init; } = string.Empty;

		/// <summary>
		/// The strongly-typed source input sequence.
		/// </summary>
		public IEnumerable<TSource> Source { get; init; } = null!;

		/// <summary>
		/// The strongly-typed selector used to transform results into comparable values.
		/// </summary>
		public Func<TSource, object> ResultSelector { get; init; } = null!;

		/// <summary>
		/// The strongly-typed transformation or query logic to test.
		/// </summary>
		public Func<IEnumerable<TSource>, IEnumerable> Invoke { get; init; } = null!;

		/// <summary>
		/// The expected transformed result.
		/// </summary>
		public IEnumerable<object> ExpectedResult { get; init; } = null!;

		public override string ToString() => Name;
	}
}