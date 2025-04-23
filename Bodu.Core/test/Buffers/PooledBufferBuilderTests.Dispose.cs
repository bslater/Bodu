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
		public void Dispose_WhenCalled_ShouldPreventBufferAccess_UsingPooledBufferBuilder()
		{
			var builder = new PooledBufferBuilder<int>();
			builder.AppendRange(Enumerable.Range(1, 5));
			builder.Dispose();

			Assert.ThrowsException<ObjectDisposedException>(() => builder.AsArray());
			Assert.ThrowsException<ObjectDisposedException>(() => builder.AsSpan());
			Assert.ThrowsException<ObjectDisposedException>(() => _ = builder.Count);
		}

		[TestMethod]
		public void Dispose_WhenInvokedMultipleTimes_ShouldNotThrow_UsingPooledBufferBuilder()
		{
			var builder = new PooledBufferBuilder<int>();
			builder.AppendRange(new[] { 1, 2, 3 });

			builder.Dispose();
			builder.Dispose(); // no exception expected
		}
	}
}