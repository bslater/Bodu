// ---------------------------------------------------------------------------------------------------------------
// <auto-generated />
// ---------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Extensions;

namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{
		[DataTestMethod]
		[DataRow(-5, DateTimeKind.Utc)]
		[DataRow(-10, DateTimeKind.Local)]
		public void GetElapsedTimeSince_WhenPastTime_ShouldReturnPositiveTimeSpan(int minutesOffset, DateTimeKind kind)
		{
			DateTime past = kind == DateTimeKind.Utc
				? DateTime.UtcNow.AddMinutes(minutesOffset)
				: DateTime.Now.AddMinutes(minutesOffset);

			past = DateTime.SpecifyKind(past, kind);

			TimeSpan elapsed = past.GetElapsedTimeSince();

			Assert.IsTrue(elapsed.TotalMinutes >= Math.Abs(minutesOffset) - 0.01,
				$"Expected at least {Math.Abs(minutesOffset)} minutes elapsed, got {elapsed.TotalMinutes:F2}");
			Assert.IsTrue(elapsed.TotalMilliseconds > 0, "Elapsed should be positive for past dates.");
		}

		[DataTestMethod]
		[DataRow(1, DateTimeKind.Utc)]
		[DataRow(5, DateTimeKind.Local)]
		public void GetElapsedTimeSince_WhenFutureTime_ShouldReturnNegativeTimeSpan(int minutesOffset, DateTimeKind kind)
		{
			DateTime future = kind == DateTimeKind.Utc
				? DateTime.UtcNow.AddMinutes(minutesOffset)
				: DateTime.Now.AddMinutes(minutesOffset);

			future = DateTime.SpecifyKind(future, kind);

			TimeSpan elapsed = future.GetElapsedTimeSince();

			Assert.IsTrue(elapsed.TotalMilliseconds < 0, "Elapsed should be negative for future timestamps.");
			Assert.IsTrue(elapsed.TotalMinutes <= -minutesOffset + 0.01,
				$"Expected approx. -{minutesOffset} min but got {elapsed.TotalMinutes:F3} min.");
		}

		[TestMethod]
		public void GetElapsedTimeSince_WhenKindIsUnspecified_ShouldThrowExactly()
		{
			DateTime input = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

			var ex = Assert.ThrowsExactly<ArgumentException>(() =>
			{
				_ = input.GetElapsedTimeSince();
			});

			StringAssert.Contains(ex.Message, "Kind");
			StringAssert.Contains(ex.Message, "Utc or Local");
		}

		[DataTestMethod]
		[DataRow(DateTimeKind.Utc)]
		[DataRow(DateTimeKind.Local)]
		public void GetElapsedTimeSince_WhenNow_ShouldReturnNearZero(DateTimeKind kind)
		{
			DateTime anchor = kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;
			anchor = DateTime.SpecifyKind(anchor, kind);

			Thread.Sleep(1); // Ensure measurable elapsed time

			TimeSpan elapsed = anchor.GetElapsedTimeSince();

			Assert.IsTrue(elapsed.TotalMilliseconds >= 0);
			Assert.IsTrue(elapsed.TotalMilliseconds < 50,
				$"Elapsed was {elapsed.TotalMilliseconds:F3}ms, expected < 50ms.");
		}
	}

}
