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
		public void Weekend_WhenAccessed_ShouldContainWeekendDays()
		{
			var set = DaysOfWeekSet.Weekend;
			Assert.AreEqual(2, set.Count);
			Assert.IsTrue(set[DayOfWeek.Saturday]);
			Assert.IsTrue(set[DayOfWeek.Sunday]);
			Assert.IsFalse(set[DayOfWeek.Monday]);
		}
	}
}