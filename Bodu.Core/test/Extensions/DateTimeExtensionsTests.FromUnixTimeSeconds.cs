// ---------------------------------------------------------------------------------------------------------------
// <auto-generated />
// ---------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Extensions;
using System.Globalization;

namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{

		[DataTestMethod]
		[DataRow(0, "1970-01-01T00:00:00.0000000Z")]                      // Epoch
		[DataRow(1, "1970-01-01T00:00:01.0000000Z")]                      // +1 second
		[DataRow(60, "1970-01-01T00:01:00.0000000Z")]                     // +1 minute
		[DataRow(3600, "1970-01-01T01:00:00.0000000Z")]                   // +1 hour
		[DataRow(-1, "1969-12-31T23:59:59.0000000Z")]                     // -1 second (before epoch)
		[DataRow(946684800, "2000-01-01T00:00:00.0000000Z")]              // Y2K
		[DataRow(2147483647, "2038-01-19T03:14:07.0000000Z")]             // 32-bit signed int max
		[DataRow(-62135596800, "0001-01-01T00:00:00.0000000Z")]           // DateTime.MinValue
		[DataRow(253402300799, "9999-12-31T23:59:59.0000000Z")]           // DateTime.MaxValue
		public void FromUnixTimeSeconds_WhenValidInput_ShouldReturnExpected(long input, string expectedUtcIso)
		{
			DateTime actual = input.FromUnixTimeSeconds();
			DateTime expected = DateTime.ParseExact(
				expectedUtcIso,
				"yyyy-MM-ddTHH:mm:ss.fffffffZ",
				CultureInfo.InvariantCulture,
				DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

			Assert.AreEqual(expected, actual);
			Assert.AreEqual(DateTimeKind.Utc, actual.Kind);
		}

		[TestMethod]
		public void FromUnixTimeSeconds_WhenBelowMinimum_ShouldThrowExactly()
		{
			long belowMin = -62135596801; // 1 second before DateTime.MinValue

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = belowMin.FromUnixTimeSeconds();
			});
		}

		[TestMethod]
		public void FromUnixTimeSeconds_WhenAboveMaximum_ShouldThrowExactly()
		{
			long aboveMax = 253402300800; // 1 second after DateTime.MaxValue

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = aboveMax.FromUnixTimeSeconds();
			});
		}
	}
}