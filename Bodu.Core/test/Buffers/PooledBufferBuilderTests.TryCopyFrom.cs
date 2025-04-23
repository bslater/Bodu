using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Buffers
{
	public partial class PooledBufferBuilderTests
	{
		[TestMethod]
		public void TryCopyFrom_WhenListPassed_ShouldCopyCorrectly_UsingICollection()
		{
			var source = new List<int> { 1, 2, 3 };
			using var builder = new PooledBufferBuilder<int>();

			bool success = builder.TryCopyFrom(source);

			Assert.IsTrue(success);
			CollectionAssert.AreEqual(source, builder.AsSpan().ToArray());
		}
	}
}