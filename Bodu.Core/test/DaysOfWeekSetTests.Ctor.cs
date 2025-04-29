using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu
{
	public partial class DaysOfWeekSetTests
	{
		[TestMethod]
		public void Constructor_WhenNoDaysProvided_ShouldBeEmpty()
		{
			var set = new DaysOfWeekSet();
			Assert.AreEqual(0, set.Count);
		}

		[TestMethod]
		public void Constructor_WhenNullArrayProvided_ShouldBeEmpty()
		{
			var set = new DaysOfWeekSet(null);
			Assert.AreEqual(0, set.Count);
		}

		[DataTestMethod]
		[DataRow(DayOfWeek.Sunday)]
		[DataRow(DayOfWeek.Monday)]
		[DataRow(DayOfWeek.Tuesday)]
		[DataRow(DayOfWeek.Wednesday)]
		[DataRow(DayOfWeek.Thursday)]
		[DataRow(DayOfWeek.Friday)]
		[DataRow(DayOfWeek.Saturday)]
		public void Constructor_WhenSingleDayProvided_ShouldContainDay(DayOfWeek day)
		{
			var set = new DaysOfWeekSet(day);
			Assert.IsTrue(set[day]);
			Assert.AreEqual(1, set.Count);
		}
	}
}