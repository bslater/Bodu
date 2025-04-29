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
		public void OrOperator_WhenCombinedWithEmpty_ShouldReturnOriginal()
		{
			var original = new DaysOfWeekSet(DayOfWeek.Monday, DayOfWeek.Tuesday);
			var result = original | DaysOfWeekSet.Empty;
			Assert.AreEqual(original, result);
		}

		[TestMethod]
		public void OrOperator_WhenCombinedWithItself_ShouldReturnSame()
		{
			var original = new DaysOfWeekSet(DayOfWeek.Monday);
			var result = original | original;
			Assert.AreEqual(original, result);
		}

		[TestMethod]
		public void AndOperator_WhenCombinedWithEmpty_ShouldReturnEmpty()
		{
			var original = new DaysOfWeekSet(DayOfWeek.Monday);
			var result = original & DaysOfWeekSet.Empty;
			Assert.AreEqual(DaysOfWeekSet.Empty, result);
		}

		[TestMethod]
		public void AndOperator_WhenCombinedWithItself_ShouldReturnSame()
		{
			var original = new DaysOfWeekSet(DayOfWeek.Wednesday);
			var result = original & original;
			Assert.AreEqual(original, result);
		}

		[TestMethod]
		public void XorOperator_WhenCombinedWithEmpty_ShouldReturnOriginal()
		{
			var original = new DaysOfWeekSet(DayOfWeek.Friday);
			var result = original ^ DaysOfWeekSet.Empty;
			Assert.AreEqual(original, result);
		}

		[TestMethod]
		public void XorOperator_WhenCombinedWithItself_ShouldReturnEmpty()
		{
			var original = new DaysOfWeekSet(DayOfWeek.Thursday);
			var result = original ^ original;
			Assert.AreEqual(DaysOfWeekSet.Empty, result);
		}

		[TestMethod]
		public void ComplementOperator_WhenAppliedToEmpty_ShouldReturnFullSet()
		{
			var result = ~DaysOfWeekSet.Empty;
			Assert.AreEqual(DaysOfWeekSet.FromByte(0b1111111), result);
		}

		[TestMethod]
		public void ComplementOperator_WhenAppliedToFullSet_ShouldReturnEmpty()
		{
			var result = ~DaysOfWeekSet.FromByte(0b1111111);
			Assert.AreEqual(DaysOfWeekSet.Empty, result);
		}

		[DataTestMethod]
		[DataRow((byte)0)]
		[DataRow((byte)1)]
		[DataRow((byte)5)]
		[DataRow((byte)127)]
		public void ImplicitConversion_WhenRoundTripped_ShouldMatchOriginal(byte original)
		{
			DaysOfWeekSet set = DaysOfWeekSet.FromByte(original);
			byte roundTripped = (byte)set;
			Assert.AreEqual(original, roundTripped);
		}
	}
}