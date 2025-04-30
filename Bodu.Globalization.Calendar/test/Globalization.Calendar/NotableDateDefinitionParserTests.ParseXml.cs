using Bodu.Extensions;
using System.Collections.Immutable;
using System.Xml.Linq;

namespace Bodu.Globalization.Calendar
{
	public partial class NotableDateDefinitionParserTests
	{
		[TestMethod]
		public void ParseXml_WhenGivenValidComplexXml_ShouldReturnExpectedNotableDates()
		{
			var xmlDoc = XDocument.Parse(NotableDateDefinitionParserTests.ComplexNotableDatesXml);
			var expected = NotableDateDefinitionParserTests.GetComplexExpectedDefinitions().ToImmutableArray();

			var actual = NotableDateDefinitionParser.ParseXml(xmlDoc).OrderBy(d => d.Name).ToImmutableArray();

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ParseXml_FixedDefinition_ShouldPopulateName()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.FixedDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual("Fixed Date Test", definition.Name);
		}

		[TestMethod]
		public void ParseXml_FixedDefinition_ShouldPopulateFirstYear()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.FixedDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual(2000, definition.FirstYear);
		}

		[TestMethod]
		public void ParseXml_FixedDefinition_ShouldPopulateLastYear()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.FixedDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual(2100, definition.LastYear);
		}

		[TestMethod]
		public void ParseXml_FixedDefinition_ShouldPopulateCountry()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.FixedDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual("AU", definition.Country);
		}

		[TestMethod]
		public void ParseXml_DynamicDefinition_ShouldPopulateProviderTypeName()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.DynamicDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual("Bodu.Globalization.Calendar.Calculators.EasterSundayNotableDateCalculator", definition.ProviderTypeName);
		}

		[TestMethod]
		public void ParseXml_DynamicDefinition_ShouldPopulateProviderAssembly()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.DynamicDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual("Bodu.Globalization.Calendar", definition.NotableDateCalculatorType);
		}

		[TestMethod]
		public void ParseXml_RuleBasedDefinition_ShouldPopulateDayOfWeek()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.RuleBasedDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual(DayOfWeek.Monday, definition.DayOfWeek);
		}

		[TestMethod]
		public void ParseXml_RuleBasedDefinition_ShouldPopulateWeekOrdinal()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.RuleBasedDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual(WeekOfMonthOrdinal.Second, definition.WeekOrdinal);
		}

		[TestMethod]
		public void ParseXml_OffsetFromDefinition_ShouldPopulateBaseNotableDateName()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.OffsetFromDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual("Easter Sunday", definition.BaseNotableDateName);
		}

		[TestMethod]
		public void ParseXml_OffsetFromDefinition_ShouldPopulateOffsetDays()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.OffsetFromDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual(-2, definition.OffsetDays);
		}

		[TestMethod]
		public void ParseXml_FixedDefinition_ShouldPopulateDefinitionType()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.FixedDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual(NotableDateDefinitionType.Fixed, definition.DefinitionType);
		}

		[TestMethod]
		public void ParseXml_FixedDefinition_ShouldPopulateNotableDateKind()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.FixedDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual(NotableDateKind.Holiday, definition.NotableDateKind);
		}

		[TestMethod]
		public void ParseXml_FixedDefinition_ShouldPopulateNonWorking()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.FixedDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.IsTrue(definition.NonWorking ?? false);
		}

		[TestMethod]
		public void ParseXml_FixedDefinition_ShouldPopulateDayAndMonth()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.FixedDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual(1, definition.Day);
			Assert.AreEqual(1, definition.Month);
		}

		[TestMethod]
		public void ParseXml_FixedDefinition_ShouldPopulateCalendarType()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.FixedDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual("System.Globalization.GregorianCalendar", definition.Calendar);
		}

		[TestMethod]
		public void ParseXml_FixedDefinition_ShouldPopulateRegion()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.FixedDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual("NSW", definition.Region);
		}

		[TestMethod]
		public void ParseXml_FixedDefinition_ShouldPopulateOccurrenceYears()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.FixedDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual(4, definition.OccurrenceYears);
		}

		[TestMethod]
		public void ParseXml_FixedDefinition_ShouldPopulateComment()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.FixedDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual("Fixed date comment.", definition.Comment);
		}

		[TestMethod]
		public void ParseXml_FixedDefinition_ShouldParseAdjustmentRules()
		{
			var doc = XDocument.Parse(NotableDateDefinitionParserTests.FixedDateXml);
			var definition = NotableDateDefinitionParser.ParseXml(doc).Single();

			Assert.AreEqual(1, definition.AdjustmentRules.Count());
			Assert.AreEqual(NotableDateAdjustmentRuleType.IfWeekend, definition.AdjustmentRules[0].AdjustmentRule);
			Assert.AreEqual(NotableDateAdjustmentActionType.MoveToNextWeekday, definition.AdjustmentRules[0].Action);
		}
	}
}