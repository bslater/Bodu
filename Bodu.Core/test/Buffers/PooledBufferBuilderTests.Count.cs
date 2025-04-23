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
		public void Count_WhenItemsAdded_ShouldReturnAccurateValue_UsingSpan()
		{
			using var builder = new PooledBufferBuilder<string>();
			builder.AppendRange(new[] { "a", "b", "c" });

			Assert.AreEqual(3, builder.Count);
		}
	}
}