using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Infrastructure
{
	public sealed class HashTestVector
	{
		public string Name { get; init; } = "";

		public byte[] Input { get; init; } = Array.Empty<byte>();

		public byte[] ExpectedHash { get; init; } = Array.Empty<byte>();
	}
}