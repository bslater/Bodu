using Bodu.Extensions;

namespace Bodu.Globalization.Calendar
{
	public partial class NotableDateDefinitionParserTests
	{
		[TestMethod]
		public void ParseJson_WhenGivenValidComplexJson_ShouldReturnExpectedNotableDates()
		{
			var jsonString = NotableDateDefinitionParserTests.ComplexNotableDatesJson;
			var expected = NotableDateDefinitionParserTests.GetComplexExpectedDefinitions();

			var actual = NotableDateDefinitionParser.ParseJson(jsonString).OrderBy(d => d.Name).ToList();

			Assert.AreEqual(expected.Count, actual.Count);

			for (int i = 0; i < expected.Count; i++)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}

		[TestMethod]
		public void ParseJson_FixedDefinition_ShouldPopulateName()
		{
			var json = NotableDateDefinitionParserTests.FixedDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual("Fixed Date Test", definition.Name);
		}

		[TestMethod]
		public void ParseJson_FixedDefinition_ShouldPopulateFirstYear()
		{
			var json = NotableDateDefinitionParserTests.FixedDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual(2000, definition.FirstYear);
		}

		[TestMethod]
		public void ParseJson_FixedDefinition_ShouldPopulateLastYear()
		{
			var json = NotableDateDefinitionParserTests.FixedDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual(2100, definition.LastYear);
		}

		[TestMethod]
		public void ParseJson_FixedDefinition_ShouldPopulateCountry()
		{
			var json = NotableDateDefinitionParserTests.FixedDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual("AU", definition.Country);
		}

		[TestMethod]
		public void ParseJson_DynamicDefinition_ShouldPopulateProviderTypeName()
		{
			var json = NotableDateDefinitionParserTests.DynamicDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual("Bodu.Globalization.Calendar.Calculators.EasterSundayNotableDateCalculator", definition.ProviderTypeName);
		}

		[TestMethod]
		public void ParseJson_DynamicDefinition_ShouldPopulateProviderAssembly()
		{
			var json = NotableDateDefinitionParserTests.DynamicDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual("Bodu.Globalization.Calendar", definition.ProviderAssembly);
		}

		[TestMethod]
		public void ParseJson_RuleBasedDefinition_ShouldPopulateDayOfWeek()
		{
			var json = NotableDateDefinitionParserTests.RuleBasedDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual(DayOfWeek.Monday, definition.DayOfWeek);
		}

		[TestMethod]
		public void ParseJson_RuleBasedDefinition_ShouldPopulateWeekOrdinal()
		{
			var json = NotableDateDefinitionParserTests.RuleBasedDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual(WeekOfMonthOrdinal.Second, definition.WeekOrdinal);
		}

		[TestMethod]
		public void ParseJson_OffsetFromDefinition_ShouldPopulateBaseNotableDateName()
		{
			var json = NotableDateDefinitionParserTests.OffsetFromDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual("Easter Sunday", definition.BaseNotableDateName);
		}

		[TestMethod]
		public void ParseJson_OffsetFromDefinition_ShouldPopulateOffsetDays()
		{
			var json = NotableDateDefinitionParserTests.OffsetFromDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual(-2, definition.OffsetDays);
		}

		[TestMethod]
		public void ParseJson_FixedDefinition_ShouldPopulateDefinitionType()
		{
			var json = NotableDateDefinitionParserTests.FixedDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual(NotableDateDefinitionType.Fixed, definition.DefinitionType);
		}

		[TestMethod]
		public void ParseJson_FixedDefinition_ShouldPopulateNotableDateKind()
		{
			var json = NotableDateDefinitionParserTests.FixedDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual(NotableDateKind.Holiday, definition.NotableDateKind);
		}

		[TestMethod]
		public void ParseJson_FixedDefinition_ShouldPopulateNonWorking()
		{
			var json = NotableDateDefinitionParserTests.FixedDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.IsTrue(definition.NonWorking ?? false);
		}

		[TestMethod]
		public void ParseJson_FixedDefinition_ShouldPopulateDayAndMonth()
		{
			var json = NotableDateDefinitionParserTests.FixedDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual(1, definition.Day);
			Assert.AreEqual(1, definition.Month);
		}

		[TestMethod]
		public void ParseJson_FixedDefinition_ShouldPopulateCalendarType()
		{
			var json = NotableDateDefinitionParserTests.FixedDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual("System.Globalization.GregorianCalendar", definition.CalendarType);
		}

		[TestMethod]
		public void ParseJson_FixedDefinition_ShouldPopulateRegion()
		{
			var json = NotableDateDefinitionParserTests.FixedDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual("NSW", definition.Region);
		}

		[TestMethod]
		public void ParseJson_FixedDefinition_ShouldPopulateOccurrenceYears()
		{
			var json = NotableDateDefinitionParserTests.FixedDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual(4, definition.OccurrenceYears);
		}

		[TestMethod]
		public void ParseJson_FixedDefinition_ShouldPopulateComment()
		{
			var json = NotableDateDefinitionParserTests.FixedDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual("Test comment", definition.Comment);
		}

		[TestMethod]
		public void ParseJson_FixedDefinition_ShouldParseAdjustmentRules()
		{
			var json = NotableDateDefinitionParserTests.FixedDateJson;
			var definition = NotableDateDefinitionParser.ParseJson(json).Single();

			Assert.AreEqual(1, definition.AdjustmentRules.Count());
			Assert.AreEqual(NotableDateAdjustmentRuleType.IfWeekend, definition.AdjustmentRules[0].AdjustmentRule);
			Assert.AreEqual(NotableDateAdjustmentActionType.MoveToNextWeekday, definition.AdjustmentRules[0].Action);
		}
	}
}