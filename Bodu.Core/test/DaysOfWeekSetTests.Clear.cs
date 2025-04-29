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
		public void Clear_WhenCalled_ShouldRemoveAllDays()
		{
			var set = new DaysOfWeekSet(DayOfWeek.Monday, DayOfWeek.Tuesday);
			set.Clear();
			Assert.AreEqual(0, set.Count);
		}
	}
}