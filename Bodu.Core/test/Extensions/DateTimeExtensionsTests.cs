using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		private sealed class FridayOnlyWeekendProvider : ICalendarWeekendProvider
		{
			public bool IsWeekend(DayOfWeek dayOfWeek) => dayOfWeek == DayOfWeek.Friday;
		}

		private sealed class InValidQuarterProvider : ICalendarQuarterProvider
		{
			/// <summary>
			/// Returns an invalid month number (outside the 1–12 range) for each valid quarter.
			/// </summary>
			/// <param name="quarter">The quarter (expected 1–4).</param>
			/// <returns>An invalid month number.</returns>
			public int GetStartMonthFromQuarter(int quarter)
			{
				return quarter switch
				{
					1 => 0,    // Invalid (below range)
					2 => 13,   // Invalid (above range)
					3 => -1,   // Invalid (negative)
					4 => 999,  // Invalid (far above range)
					_ => throw new ArgumentOutOfRangeException(nameof(quarter), "GetQuarter must be between 1 and 4.")
				};
			}

			/// <summary>
			/// Always returns an invalid quarter number (outside the 1–4 range).
			/// </summary>
			/// <param name="dateTime">The input <see cref="DateTime" />.</param>
			/// <returns>An invalid quarter number.</returns>
			public int GetQuarter(DateTime dateTime)
			{
				return dateTime.Month switch
				{
					1 => 0,    // Invalid (below range)
					2 => -5,   // Invalid (negative)
					3 => 5,    // Invalid (above range)
					4 => 999,  // Invalid (far above range)
					_ => 10    // Also invalid
				};
			}
		}

		private sealed class ValidQuarterProvider : ICalendarQuarterProvider
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

			/// <summary>
			/// Returns the first calendar month corresponding to the specified quarter, where quarters are defined as:
			/// <list type="bullet">
			/// <item>
			/// <term>Q1</term>
			/// <description>December–February (Starts in December)</description>
			/// </item>
			/// <item>
			/// <term>Q2</term>
			/// <description>March–May</description>
			/// </item>
			/// <item>
			/// <term>Q3</term>
			/// <description>June–August</description>
			/// </item>
			/// <item>
			/// <term>Q4</term>
			/// <description>September–November</description>
			/// </item>
			/// </list>
			/// </summary>
			/// <param name="quarter">The quarter number (1–4).</param>
			/// <returns>The first month of the specified quarter, where months are in the range 1 (January) to 12 (December).</returns>
			/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="quarter" /> is not in the range 1–4.</exception>
			/// <remarks>
			/// This implementation supports fiscal or retail calendars where the fiscal year begins in December, and Q1 spans December to February.
			/// </remarks>
			public int GetStartMonthFromQuarter(int quarter)
			{
				return quarter switch
				{
					1 => 12,
					2 => 3,
					3 => 6,
					4 => 9,
					_ => throw new ArgumentOutOfRangeException(nameof(quarter), "GetQuarter must be between 1 and 4.")
				};
			}

			/// <summary>
			/// Gets the quarter number (1–4) for the specified date where quarters are defined as: Q1 = Dec–Feb, Q2 = Mar–May, Q3 =
			/// Jun–Aug, Q4 = Sep–Nov.
			/// </summary>
			/// <param name="dateTime">The date to evaluate.</param>
			/// <returns>An integer from 1 to 4 indicating the quarter.</returns>
			public int GetQuarter(DateTime dateTime)
			{
				// Shift month index so that December = 0, January = 1, ..., November = 11
				int shifted = dateTime.Month % 12;
				return (shifted / 3) + 1;
			}
		}
	}
}