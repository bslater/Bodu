using System;
using System.Globalization;

namespace Bodu.Extensions
{
	/// <summary>
	/// Contains unit tests for the <see cref="DateTimeExtensions" /> extension methods.
	/// </summary>
	[TestClass]
	public partial class DateTimeExtensionsTests
	{
		private static readonly DateTime UnixEpochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static CultureInfo TestCulture
		{
			get
			{
				var customCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
				customCulture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Wednesday;

				return customCulture;
			}
		}

		private sealed class FridayOnlyWeekendProvider : IWeekendDefinitionProvider
		{
			public bool IsWeekend(DayOfWeek dayOfWeek) => dayOfWeek == DayOfWeek.Friday;
		}

		private sealed class InValidQuarterProvider : IQuarterDefinitionProvider
		{
			/// <summary>
			/// Always returns an invalid quarter number (outside the expected range of 1–4).
			/// </summary>
			/// <param name="dateTime">The input <see cref="DateTime" />.</param>
			/// <returns>An invalid quarter number.</returns>
			public int GetQuarter(DateTime dateTime)
			{
				return dateTime.Month switch
				{
					1 => 0,     // Invalid (below range)
					2 => -5,    // Invalid (negative)
					3 => 5,     // Invalid (above range)
					4 => 999,   // Invalid (far above range)
					_ => 10     // Also invalid
				};
			}

			/// <summary>
			/// Always throws <see cref="ArgumentOutOfRangeException" /> to simulate an invalid quarter mapping.
			/// </summary>
			/// <param name="dateTime">The input <see cref="DateTime" />.</param>
			public DateTime GetStartDate(DateTime dateTime)
			{
				throw new ArgumentOutOfRangeException(nameof(dateTime), "This provider intentionally returns invalid quarter mappings.");
			}

			/// <summary>
			/// Always throws <see cref="ArgumentOutOfRangeException" /> to simulate an invalid quarter mapping.
			/// </summary>
			/// <param name="dateTime">The input <see cref="DateTime" />.</param>
			public DateTime GetEndDate(DateTime dateTime)
			{
				throw new ArgumentOutOfRangeException(nameof(dateTime), "This provider intentionally returns invalid quarter mappings.");
			}

			public DateTime GetStartDate(int quarter)
			{
				throw new ArgumentOutOfRangeException(nameof(quarter), "This provider intentionally returns invalid quarter mappings.");
			}

			public DateTime GetEndDate(int quarter)
			{
				throw new ArgumentOutOfRangeException(nameof(quarter), "This provider intentionally returns invalid quarter mappings.");
			}
		}

		private sealed class ValidQuarterProvider : IQuarterDefinitionProvider
		{
			public static IEnumerable<object[]> QuarterDefinitionTestData
			{
				get
				{
					yield return new object[] { new DateTime(2024, 01, 01), 1, new DateTime(2023, 12, 01), new DateTime(2024, 02, 29) };
					yield return new object[] { new DateTime(2024, 02, 01), 1, new DateTime(2023, 12, 01), new DateTime(2024, 02, 29) };
					yield return new object[] { new DateTime(2024, 03, 01), 2, new DateTime(2024, 03, 01), new DateTime(2024, 05, 31) };
					yield return new object[] { new DateTime(2024, 04, 01), 2, new DateTime(2024, 03, 01), new DateTime(2024, 05, 31) };
					yield return new object[] { new DateTime(2024, 05, 01), 2, new DateTime(2024, 03, 01), new DateTime(2024, 05, 31) };
					yield return new object[] { new DateTime(2024, 06, 01), 3, new DateTime(2024, 06, 01), new DateTime(2024, 08, 31) };
					yield return new object[] { new DateTime(2024, 07, 01), 3, new DateTime(2024, 06, 01), new DateTime(2024, 08, 31) };
					yield return new object[] { new DateTime(2024, 08, 01), 3, new DateTime(2024, 06, 01), new DateTime(2024, 08, 31) };
					yield return new object[] { new DateTime(2024, 09, 01), 4, new DateTime(2024, 09, 01), new DateTime(2024, 11, 30) };
					yield return new object[] { new DateTime(2024, 10, 01), 4, new DateTime(2024, 09, 01), new DateTime(2024, 11, 30) };
					yield return new object[] { new DateTime(2024, 11, 01), 4, new DateTime(2024, 09, 01), new DateTime(2024, 11, 30) };
					yield return new object[] { new DateTime(2023, 12, 01), 1, new DateTime(2023, 12, 01), new DateTime(2024, 02, 29) };
				}
			}

			public int GetQuarter(DateTime dateTime)
			{
				return dateTime.Month switch
				{
					12 => 1,
					1 or 2 => 1,
					3 or 4 or 5 => 2,
					6 or 7 or 8 => 3,
					9 or 10 or 11 => 4,
					_ => throw new ArgumentOutOfRangeException(nameof(dateTime.Month))
				};
			}

			public DateTime GetStartDate(DateTime dateTime)
			{
				return GetQuarter(dateTime) switch
				{
					1 => new DateTime(dateTime.Month == 12 ? dateTime.Year : dateTime.Year - 1, 12, 1),
					2 => new DateTime(dateTime.Year, 3, 1),
					3 => new DateTime(dateTime.Year, 6, 1),
					4 => new DateTime(dateTime.Year, 9, 1),
					_ => throw new ArgumentOutOfRangeException(nameof(dateTime))
				};
			}

			public DateTime GetEndDate(DateTime dateTime)
			{
				var quarter = GetQuarter(dateTime);
				var start = GetStartDate(dateTime);

				return quarter switch
				{
					1 => new DateTime(start.Year + 1, 2, DateTime.DaysInMonth(start.Year + 1, 2)),
					2 => new DateTime(start.Year, 5, 31),
					3 => new DateTime(start.Year, 8, 31),
					4 => new DateTime(start.Year, 11, 30),
					_ => throw new ArgumentOutOfRangeException(nameof(dateTime))
				};
			}

			public DateTime GetStartDate(int quarter)
			{
				return quarter switch
				{
					1 => new DateTime(2023, 12, 1), // assumes test year context
					2 => new DateTime(2024, 3, 1),
					3 => new DateTime(2024, 6, 1),
					4 => new DateTime(2024, 9, 1),
					_ => throw new ArgumentOutOfRangeException(nameof(quarter))
				};
			}

			public DateTime GetEndDate(int quarter)
			{
				return quarter switch
				{
					1 => new DateTime(2024, 2, DateTime.DaysInMonth(2024, 2)),
					2 => new DateTime(2024, 5, 31),
					3 => new DateTime(2024, 8, 31),
					4 => new DateTime(2024, 11, 30),
					_ => throw new ArgumentOutOfRangeException(nameof(quarter))
				};
			}
		}
	}
}