using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu
{
	public partial class DaysOfWeekSetTests
	{
		[DataTestMethod]
		[DynamicData(nameof(GetValidDays), DynamicDataSourceType.Method)]
		public void Indexer_WhenSettingValidDay_ShouldReflectChange(DayOfWeek day)
		{
			var set = new DaysOfWeekSet();
			set[day] = true;
			Assert.IsTrue(set[day]);
		}

		[DataTestMethod]
		[DataRow(-1)]
		[DataRow(7)]
		[DataRow(10)]
		public void IndexerGet_WhenInvalidDayIndex_ShouldThrowExactly(int invalidDayIndex)
		{
			var set = new DaysOfWeekSet();
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				var _ = set[(DayOfWeek)invalidDayIndex];
			});
		}

		[DataTestMethod]
		[DataRow(-1)]
		[DataRow(7)]
		[DataRow(10)]
		public void IndexerSet_WhenInvalidDayIndex_ShouldThrowExactly(int invalidDayIndex)
		{
			var set = new DaysOfWeekSet();
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				set[(DayOfWeek)invalidDayIndex] = true;
			});
		}

		private static IEnumerable<object[]> GetValidDays()
		{
			foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
				yield return new object[] { day };
		}
	}
}