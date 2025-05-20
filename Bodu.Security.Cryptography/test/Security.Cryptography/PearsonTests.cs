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
		: Security.Cryptography.HashAlgorithmTests<PearsonTests, Pearson, SingleHashVariant>
	{
		public override IEnumerable<SingleHashVariant> GetHashAlgorithmVariants() => new[]
		{
			SingleHashVariant.Default
		};

		protected override Pearson CreateAlgorithm() => new Pearson();

		protected override Pearson CreateAlgorithm(SingleHashVariant variant) => new Pearson();

		protected override IReadOnlyList<string> GetExpectedHashesForIncrementalInput(SingleHashVariant variant) => new[]
		{
			"00",
			"01",
			"01",
			"17" ,
		};

		protected override IReadOnlyDictionary<string, string> GetExpectedHashesForNamedInputs(SingleHashVariant variant) => new Dictionary<string, string>
		{
			["Empty"] = "00",
			["ABC"] = "1A",
			["Zeros_16"] = "C1",
			["QuickBrownFox"] = "E9",
		};
	}
}