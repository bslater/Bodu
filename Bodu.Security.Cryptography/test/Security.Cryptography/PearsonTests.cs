using Bodu.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Security.Cryptography
{
	[TestClass]
	public partial class PearsonTests
		: Security.Cryptography.HashAlgorithmTests<Pearson>
	{
		protected override string ExpectedHash_ForEmptyByteArray => "00";

		public override IEnumerable<HashAlgorithmVariant> GetVariants() => Enumerable.Empty<HashAlgorithmVariant>();

		protected override Pearson CreateAlgorithm() => new Pearson();
	}
}