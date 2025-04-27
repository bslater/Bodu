using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Extensions
{
	public static partial class ComparableHelper
	{
		/// <summary>
		/// Returns the first non-null value from two specified values.
		/// </summary>
		/// <typeparam name="T">The type of the values.</typeparam>
		/// <param name="first">The first value to return if not <c>null</c>.</param>
		/// <param name="second">The second value to return if the first is <c>null</c>.</param>
		/// <returns>The first non-null value, or <c>null</c> if both values are <c>null</c>.</returns>
		public static T? Coalesce<T>(T? first, T? second) =>
			first ?? second;
	}
}
