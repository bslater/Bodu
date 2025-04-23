using Bodu.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Security.Cryptography
{
	[TestClass]
	public partial class SimpleSummingHashAlgorithmTests
		: Security.Cryptography.HashAlgorithmTests<SimpleSummingHashAlgorithm>
	{
		protected override string ExpectedHash_ForEmptyByteArray => "00000000";

		/// <inheritdoc />
		protected override SimpleSummingHashAlgorithm CreateAlgorithm() => new SimpleSummingHashAlgorithm();
	}
}