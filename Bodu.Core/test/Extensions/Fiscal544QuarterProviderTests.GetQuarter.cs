using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Extensions
{
	public partial class Fiscal544QuarterProviderTests
	{
		/// <summary>
		/// Verifies that GetQuarter returns the expected quarter number.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetFiscal544QuarterTestData), DynamicDataSourceType.Method)]
		public void GetQuarter_WhenCalled_ShouldReturnExpected(
			Fiscal544QuarterProvider provider,
			DateTime date,
			int expectedQuarter,
			DateTime _,
			DateTime expectedEnd)
		{
			var result = provider.GetQuarter(date);
			Assert.AreEqual(expectedQuarter, result);
		}
	}
}