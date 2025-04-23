using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Collections.Generic
{
	[TestClass]
	public partial class ShuffleHelpersTests
		: EnumerableTests
	{
		public static IEnumerable<object[]> ValidCounts()
		{
			yield return new object[] { Enumerable.Range(1, 10).ToArray(), 0 };
			yield return new object[] { Enumerable.Range(1, 10).ToArray(), 1 };
			yield return new object[] { Enumerable.Range(1, 10).ToArray(), 5 };
			yield return new object[] { Enumerable.Range(1, 10).ToArray(), 10 };
		}
	}
}