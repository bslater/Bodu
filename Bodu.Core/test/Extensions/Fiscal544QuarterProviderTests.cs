using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Extensions
{
	[TestClass]
	public partial class Fiscal544QuarterProviderTests
	{
		private static readonly Fiscal544QuarterProvider SundayProvider = new Fiscal544QuarterProvider(
						fiscalYearStart: new DateTime(2023, 1, 29),
						firstDayOfWeek: DayOfWeek.Sunday);

		private static readonly Fiscal544QuarterProvider MondayProvider = new Fiscal544QuarterProvider(
						fiscalYearStart: new DateTime(2023, 1, 30),
						firstDayOfWeek: DayOfWeek.Monday);

		private static readonly Fiscal544QuarterProvider SaturdayProvider = new Fiscal544QuarterProvider(
						fiscalYearStart: new DateTime(2023, 1, 28),
						firstDayOfWeek: DayOfWeek.Saturday);

		private static readonly Fiscal544QuarterProvider AlternateProvider = new Fiscal544QuarterProvider(
						fiscalYearStart: new DateTime(2025, 7, 5),
						firstDayOfWeek: DayOfWeek.Saturday);

		/// <summary>
		/// Provides test cases with custom Fiscal544QuarterProvider instances.
		/// </summary>
		public static IEnumerable<object[]> GetFiscal544QuarterTestData()
		{
			// Sunday start (2023-01-29)
			yield return new object[] { SundayProvider, new DateTime(2023, 2, 12), 1, new DateTime(2023, 1, 29), new DateTime(2023, 4, 29) };
			yield return new object[] { SundayProvider, new DateTime(2023, 5, 14), 2, new DateTime(2023, 4, 30), new DateTime(2023, 7, 29) };
			yield return new object[] { SundayProvider, new DateTime(2023, 8, 13), 3, new DateTime(2023, 7, 30), new DateTime(2023, 10, 28) };
			yield return new object[] { SundayProvider, new DateTime(2023, 11, 12), 4, new DateTime(2023, 10, 29), new DateTime(2024, 1, 27) };

			// Monday start (2023-01-30)
			yield return new object[] { MondayProvider, new DateTime(2023, 2, 13), 1, new DateTime(2023, 1, 30), new DateTime(2023, 4, 30) };
			yield return new object[] { MondayProvider, new DateTime(2023, 5, 15), 2, new DateTime(2023, 5, 1), new DateTime(2023, 7, 30) };
			yield return new object[] { MondayProvider, new DateTime(2023, 8, 14), 3, new DateTime(2023, 7, 31), new DateTime(2023, 10, 29) };
			yield return new object[] { MondayProvider, new DateTime(2023, 11, 13), 4, new DateTime(2023, 10, 30), new DateTime(2024, 1, 28) };

			// Saturday start (2023-01-28)
			yield return new object[] { SaturdayProvider, new DateTime(2023, 2, 11), 1, new DateTime(2023, 1, 28), new DateTime(2023, 4, 28) };
			yield return new object[] { SaturdayProvider, new DateTime(2023, 5, 13), 2, new DateTime(2023, 4, 29), new DateTime(2023, 7, 28) };
			yield return new object[] { SaturdayProvider, new DateTime(2023, 8, 12), 3, new DateTime(2023, 7, 29), new DateTime(2023, 10, 27) };
			yield return new object[] { SaturdayProvider, new DateTime(2023, 11, 11), 4, new DateTime(2023, 10, 28), new DateTime(2024, 1, 26) };

			// Saturday start (2025-07-05)
			yield return new object[] { AlternateProvider, new DateTime(2025, 7, 12), 1, new DateTime(2025, 7, 5), new DateTime(2025, 10, 3) };
			yield return new object[] { AlternateProvider, new DateTime(2025, 11, 1), 2, new DateTime(2025, 10, 4), new DateTime(2026, 1, 2) };
			yield return new object[] { AlternateProvider, new DateTime(2026, 2, 15), 3, new DateTime(2026, 1, 3), new DateTime(2026, 4, 3) };
			yield return new object[] { AlternateProvider, new DateTime(2026, 5, 30), 4, new DateTime(2026, 4, 4), new DateTime(2026, 7, 3) };
		}
	}
}