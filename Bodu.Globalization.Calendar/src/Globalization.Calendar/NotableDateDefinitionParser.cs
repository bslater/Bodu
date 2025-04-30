using Bodu.Extensions;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using SysGlobal = System.Globalization;

namespace Bodu.Globalization.Calendar
{
	/// <summary>
	/// Provides functionality to validate and parse notable date definitions from XML content into strongly typed objects.
	/// </summary>
	/// <remarks>
	/// All XML parsed by this class must conform to the embedded notable dates schema (NotableDates.xsd). Validation is always enforced
	/// before parsing.
	/// </remarks>
	/// <summary>
	/// Provides functionality for parsing notable date definitions from XML metadata.
	/// </summary>
	public static class NotableDateDefinitionParser
	{
		private static readonly XNamespace Namespace = "urn:bodu:globalization:calendar";
		private static readonly XmlSchemaSet SchemaSet = LoadSchema();

		/// <summary>
		/// Parses notable date definitions from an XML string after validating against the embedded schema.
		/// </summary>
		public static List<NotableDateDefinition> ParseXml(string xml)
		{
			if (string.IsNullOrWhiteSpace(xml))
				throw new ArgumentNullException(nameof(xml));

			using var stringReader = new StringReader(xml);
			using var xmlReader = XmlReader.Create(stringReader, CreateValidationSettings());

			var document = XDocument.Load(xmlReader);
			return ParseInternal(document);
		}

		/// <summary>
		/// Parses notable date definitions from a preloaded <see cref="XDocument" /> after validating against the embedded schema.
		/// </summary>
		public static List<NotableDateDefinition> ParseXml(XDocument document)
		{
			if (document is null)
				throw new ArgumentNullException(nameof(document));

			ValidateDocument(document);
			return ParseInternal(document);
		}

		private static List<NotableDateDefinition> ParseInternal(XDocument document)
		{
			return document
				.Descendants(Namespace + "NotableDate")
				.SelectMany(ParseNotableDate)
				.ToList();
		}

		private static IEnumerable<NotableDateDefinition> ParseNotableDate(XElement notableDateElement)
		{
			var name = GetRequiredAttributeValue(notableDateElement, "name");

			Debug.Assert(name != "ANZAC Day");

			return notableDateElement.Elements(Namespace + "Definition")
				.Select(definitionElement =>
				{
					var definitionTypeElement = definitionElement.Elements().FirstOrDefault()
						?? throw new InvalidOperationException($"Definition element for notable date '{name}' has no child element specifying the definition type.");

					var definitionType = definitionTypeElement.Name.LocalName switch
					{
						"Fixed" => NotableDateDefinitionType.Fixed,
						"Rule" => NotableDateDefinitionType.Rule,
						"Dynamic" => NotableDateDefinitionType.Dynamic,
						"OffsetFrom" => NotableDateDefinitionType.OffsetFrom,
						_ => throw new InvalidOperationException($"Unknown definition type '{definitionTypeElement.Name.LocalName}' for notable date '{name}'.")
					};

					return ParseDefinitionSpecifics(new NotableDateDefinition
					{
						Name = GetOptionalAttributeValue(definitionElement, "name") ?? name,
						TerritoryCode = GetOptionalAttributeValue(definitionElement, "territory"),
						FirstYear = ParseOptionalIntAttr(definitionElement, "firstYear"),
						LastYear = ParseOptionalIntAttr(definitionElement, "lastYear"),
						NonWorking = ParseOptionalBoolAttr(definitionElement, "nonWorking") ?? false,
						Comment = GetOptionalAttributeValue(definitionElement, "comment"),
						CalendarType = ParseOptionalTypeValue<SysGlobal.Calendar>(definitionElement, "calendarType"),
						DefinitionType = definitionType,
						NotableDateKind = ParseOptionalEnumValue<NotableDateKind>(definitionElement, "kind") ?? NotableDateKind.None,
						AdjustmentRules = definitionElement.Elements(Namespace + "AdjustmentRule")
							.Select(ParseAdjustmentRule)
							.ToImmutableArray()
					}, definitionTypeElement);
				});
		}

		private static NotableDateDefinition ParseDefinitionSpecifics(NotableDateDefinition definition, XElement definitionElement)
		{
			return definition.DefinitionType switch
			{
				NotableDateDefinitionType.Fixed => definition with
				{
					Month = ParseMonth(GetRequiredAttributeValue(definitionElement, "month")),
					Day = int.Parse(GetRequiredAttributeValue(definitionElement, "day"), CultureInfo.InvariantCulture)
				},

				NotableDateDefinitionType.Dynamic => definition with
				{
					NotableDateCalculatorType = ParseRequiredTypeValue<INotableDateCalculator>(definitionElement, "providerType")
				},

				NotableDateDefinitionType.Rule => definition with
				{
					Month = ParseMonth(GetRequiredAttributeValue(definitionElement, "month")),
					WeekOrdinal = ParseRequiredEnumValue<WeekOfMonthOrdinal>(definitionElement, "weekOrdinal"),
					DayOfWeek = ParseRequiredEnumValue<DayOfWeek>(definitionElement, "dayOfWeek")
				},

				NotableDateDefinitionType.OffsetFrom => definition with
				{
					BaseNotableDateName = GetRequiredAttributeValue(definitionElement, "name"),
					OffsetDays = int.Parse(GetRequiredAttributeValue(definitionElement, "offset"), CultureInfo.InvariantCulture)
				},

				_ => throw new NotSupportedException($"Unsupported NotableDateDefinitionType: {definition.DefinitionType}")
			};
		}

		private static NotableDateAdjustmentRule ParseAdjustmentRule(XElement adjustmentElement)
		{
			return new NotableDateAdjustmentRule
			{
				AdjustmentRule = ParseRequiredEnumValue<NotableDateAdjustmentRuleType>(adjustmentElement, "when"),
				TerritoryCode = GetOptionalAttributeValue(adjustmentElement, "territory"),
				Action = ParseRequiredEnumValue<NotableDateAdjustmentActionType>(adjustmentElement, "action"),
				Priority = ParseOptionalIntAttr(adjustmentElement, "priority") ?? 100,
				DayOfWeek = ParseOptionalEnumValue<DayOfWeek>(adjustmentElement, "dayOfWeek"),
				NonWorking = ParseOptionalBoolAttr(adjustmentElement, "nonWorking"),
				CalendarType = ParseOptionalTypeValue<SysGlobal.Calendar>(adjustmentElement, "calendarType"),
				TargetNotableDateName = GetOptionalAttributeValue(adjustmentElement, "target"),
				CustomHandler = GetOptionalAttributeValue(adjustmentElement, "customHandler"),
				OffsetDays = ParseOptionalIntAttr(adjustmentElement, "days") ?? 0,
			};
		}

		/// <summary>
		/// Parses a required attribute into a <see cref="Type" /> that is assignable to the specified base type.
		/// </summary>
		/// <typeparam name="TBase">The base type that the resolved type must be assignable to.</typeparam>
		/// <param name="element">The XML element containing the attribute.</param>
		/// <param name="attributeName">The name of the attribute containing the fully qualified type name.</param>
		/// <returns>The resolved <see cref="Type" />.</returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the attribute is missing, the type cannot be resolved, or the type is not assignable to <typeparamref name="TBase" />.
		/// </exception>
		private static Type ParseRequiredTypeValue<TBase>(XElement element, string attributeName)
		{
			var typeName = GetRequiredAttributeValue(element, attributeName);

			var type = Type.GetType(typeName, throwOnError: false);

			if (type == null)
				throw new InvalidOperationException($"Attribute '{attributeName}' on element '{element.Name.LocalName}' specifies type '{typeName}' that could not be resolved.");

			if (!typeof(TBase).IsAssignableFrom(type))
				throw new InvalidOperationException($"Type '{typeName}' from attribute '{attributeName}' is not assignable to {typeof(TBase).FullName}.");

			return type;
		}

		private static Type? ParseOptionalTypeValue<TBase>(XElement element, string attributeName)
		{
			var typeName = GetOptionalAttributeValue(element, attributeName);
			if (string.IsNullOrWhiteSpace(typeName))
				return null;

			var type = Type.GetType(typeName, throwOnError: false);
			if (type == null || !typeof(TBase).IsAssignableFrom(type))
				return null;

			return type;
		}

		private static string GetRequiredAttributeValue(XElement element, string attributeName)
			=> element.Attribute(attributeName)?.Value
			   ?? throw new InvalidOperationException($"Missing required attribute '{attributeName}' on element '{element.Name.LocalName}'.");

		private static string? GetOptionalAttributeValue(XElement element, string attributeName)
			=> element.Attribute(attributeName)?.Value;

		private static TEnum ParseRequiredEnumValue<TEnum>(XElement element, string attributeName) where TEnum : struct, Enum
			=> Enum.TryParse<TEnum>(GetRequiredAttributeValue(element, attributeName), ignoreCase: true, out var result)
				? result
				: throw new InvalidOperationException($"Invalid value for attribute '{attributeName}' on element '{element.Name.LocalName}'.");

		private static TEnum? ParseOptionalEnumValue<TEnum>(XElement element, string attributeName) where TEnum : struct, Enum
		{
			var value = GetOptionalAttributeValue(element, attributeName);
			return value != null && Enum.TryParse<TEnum>(value, ignoreCase: true, out var result) ? result : null;
		}

		private static int? ParseOptionalIntAttr(XElement element, string attributeName)
		{
			var value = GetOptionalAttributeValue(element, attributeName);
			return value != null && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : (int?)null;
		}

		private static bool? ParseOptionalBoolAttr(XElement element, string attributeName)
		{
			var value = GetOptionalAttributeValue(element, attributeName);
			return value != null && bool.TryParse(value, out var result) ? result : (bool?)null;
		}

		private static int ParseMonth(string monthName)
		{
			ThrowHelper.ThrowIfNullOrEmpty(monthName);

			if (DateTime.TryParseExact(monthName, "MMMM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
				return result.Month;

			throw new FormatException($"Invalid month name '{monthName}'. Must be full month name, e.g., 'January'.");
		}

		private static XmlSchemaSet LoadSchema()
		{
			var assembly = Assembly.GetExecutingAssembly();
			const string schemaResourceName = "Bodu.Globalization.Calendar.NotableDates.xsd";

			using var stream = assembly.GetManifestResourceStream(schemaResourceName)
				?? throw new FileNotFoundException($"Embedded schema resource '{schemaResourceName}' not found in assembly '{assembly.FullName}'.");

			var schemaSet = new XmlSchemaSet();
			schemaSet.Add(null, XmlReader.Create(stream));
			return schemaSet;
		}

		private static XmlReaderSettings CreateValidationSettings()
		{
			var settings = new XmlReaderSettings
			{
				ValidationType = ValidationType.Schema,
				Schemas = SchemaSet,
				ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
			};
			settings.ValidationEventHandler += HandleValidationEvent;
			return settings;
		}

		private static void ValidateDocument(XDocument document)
		{
			using var reader = document.CreateReader();
			using var validatingReader = XmlReader.Create(reader, CreateValidationSettings());

			while (validatingReader.Read()) { }
		}

		private static void HandleValidationEvent(object? sender, ValidationEventArgs e)
		{
			if (e.Severity == XmlSeverityType.Error)
				throw new XmlSchemaValidationException($"Schema validation error: {e.Message}", e.Exception);
		}
	}

	/*

	/// <summary>
	/// Provides functionality to parse notable date definitions from XML or JSON inputs.
	/// </summary>
	public static class NotableDateDefinitionParser
	{
		private static readonly XNamespace ns = "urn:bodu:globalization:calendar";
		private static readonly XmlNamespaceResolver resolver = new XmlNamespaceResolver(new XElement(ns + "NotableDates"));

		/// <summary>
		/// Parses an XML document containing notable date definitions.
		/// </summary>
		/// <param name="doc">The XDocument representing the XML structure.</param>
		/// <returns>An enumerable of <see cref="NotableDateDefinition" /> records.</returns>
		/// <exception cref="InvalidDataException">Thrown if required attributes or values are missing or invalid.</exception>
		public static IEnumerable<NotableDateDefinition> ParseXml(XDocument doc)
		{
			ThrowHelper.ThrowIfNull(doc);
			XmlNamespaceResolver resolver = new XmlNamespaceResolver(doc.Root);

			foreach (var node in resolver.Elements(doc.Root, "NotableDate"))
			{
				var name = node.Attribute("name")?.Value ?? throw new InvalidDataException("Missing 'name' attribute on NotableDate.");

				foreach (var definition in resolver.Elements(node, "Definition"))
				{
					yield return ParseDefinition(resolver, definition, name);
				}
			}
		}

		/// <summary>
		/// Parses a JSON string containing notable date definitions.
		/// </summary>
		/// <param name="json">The JSON string to parse.</param>
		/// <returns>An enumerable of <see cref="NotableDateDefinition" /> records.</returns>
		public static IEnumerable<NotableDateDefinition> ParseJson(string json)
		{
			ThrowHelper.ThrowIsNullOrWhiteSpace(json);

			var definitions = JsonSerializer.Deserialize<List<JsonNotableDateDefinition>>(json)
							  ?? throw new InvalidDataException("Failed to parse JSON definitions.");

			foreach (var item in definitions)
				yield return item.ToDefinition();
		}

		private static NotableDateDefinition ParseDefinition(XmlNamespaceResolver resolver, XElement element, string name)
		{
			var firstChild = element.Elements().FirstOrDefault() ?? throw new InvalidDataException("Missing NotableDate definition type element.");
			var typeAttr = firstChild.Name.LocalName;

			if (!Enum.TryParse(typeAttr, true, out NotableDateDefinitionType type))
				throw new InvalidDataException($"Invalid {nameof(NotableDateDefinitionType)}: {typeAttr}");

			var common = new NotableDateDefinition
			{
				Name = name,
				DefinitionType = type,
				NotableDateKind = ParseEnumAttr<NotableDateKind>(element, "type") ?? NotableDateKind.Holiday,
				FirstYear = ParseIntAttr(element, "firstYear"),
				LastYear = ParseIntAttr(element, "lastYear"),
				NonWorking = ParseBoolAttr(element, "nonWorking"),
				CalendarType = element.Attribute("calendarType")?.Value,
				Country = element.Attribute("country")?.Value,
				Region = element.Attribute("subRegion")?.Value,
				OccurrenceYears = ParseIntAttr(element, "occurrenceYears"),
				Comment = element.Attribute("comment")?.Value, // ➡️ New
				AdjustmentRules = ParseAdjustmentRules(resolver, element).ToImmutableArray()
			};

			if (resolver.Element(element, "Fixed") is XElement fixedElem)
			{
				return common with
				{
					Month = ParseMonthName(fixedElem.Attribute("month")?.Value),
					Day = ParseIntAttr(fixedElem, "day", required: true)
				};
			}

			if (resolver.Element(element, "Dynamic") is XElement dynElem)
			{
				return common with
				{
					ProviderTypeName = dynElem.Attribute("providerType")?.Value
						?? throw new InvalidDataException("Missing 'providerType' in Dynamic definition."),
					ProviderAssembly = dynElem.Attribute("providerAssembly")?.Value
				};
			}

			if (resolver.Element(element, "Rule") is XElement ruleElem)
			{
				return common with
				{
					Month = ParseMonthName(ruleElem.Attribute("month")?.Value),
					DayOfWeek = ParseEnumAttr<DayOfWeek>(ruleElem, "dayOfWeek"),
					WeekOrdinal = ParseEnumAttr<WeekOfMonthOrdinal>(ruleElem, "ordinal")
				};
			}

			if (resolver.Element(element, "OffsetFrom") is XElement offsetElem)
			{
				return common with
				{
					BaseNotableDateName = offsetElem.Attribute("name")?.Value
						?? throw new InvalidDataException("Missing 'name' in OffsetFrom definition."),
					OffsetDays = ParseIntAttr(offsetElem, "offset", required: true)
				};
			}

			throw new InvalidDataException("Missing or unsupported NotableDate definition type.");
		}

		private static List<NotableDateAdjustmentRule> ParseAdjustmentRules(XmlNamespaceResolver resolver, XElement definitionElement)
		{
			var result = new List<NotableDateAdjustmentRule>();

			foreach (var rule in resolver.Elements(definitionElement, "AdjustmentRule"))
			{
				var when = ParseEnumAttr<NotableDateAdjustmentRuleType>(rule, "when")
					?? throw new InvalidDataException("Missing or invalid 'when' attribute in AdjustmentRule.");
				var action = ParseEnumAttr<NotableDateAdjustmentActionType>(rule, "action")
					?? throw new InvalidDataException("Missing or invalid 'action' attribute in AdjustmentRule.");

				result.Add(new NotableDateAdjustmentRule
				{
					AdjustmentRule = when,
					Action = action,
					DayOfWeek = ParseEnumAttr<DayOfWeek>(rule, "dayOfWeek"),
					Priority = ParseIntAttr(rule, "priority") ?? 100,
					NonWorking = ParseBoolAttr(rule, "nonWorking"),
					CountryCode = rule.Attribute("country")?.Value,
					Region = rule.Attribute("region")?.Value,
					CalendarSystem = rule.Attribute("calendarSystem")?.Value,
					TargetNotableDateName = rule.Attribute("target")?.Value,
					CustomHandler = rule.Attribute("customHandler")?.Value
				});
			}

			return result;
		}

		private static void ValidateConditionAttributes(
			XElement rule,
			NotableDateAdjustmentRuleType when,
			NotableDateAdjustmentActionType action,
			int? offset,
			DayOfWeek? dayOfWeek,
			string? target,
			string? customHandler)
		{
			switch (when)
			{
				case NotableDateAdjustmentRuleType.IfDayOfWeek:
					if (dayOfWeek is null)
						throw new InvalidDataException("The 'dayOfWeek' attribute is required when condition is 'ShiftIfDayOfWeek'.");
					break;
			}
			switch (action)
			{
				case NotableDateAdjustmentActionType.AddDays:
					if (offset is null)
						throw new InvalidDataException("The 'offset' attribute is required when action is 'AddDays'.");
					break;

				case NotableDateAdjustmentActionType.ReplaceWithNamedDate:
					if (string.IsNullOrWhiteSpace(target))
						throw new InvalidDataException("The 'target' attribute is required when action is 'ReplaceWithNamedDate'.");
					break;

				case NotableDateAdjustmentActionType.Custom:
					if (string.IsNullOrWhiteSpace(customHandler))
						throw new InvalidDataException("The 'customHandler' attribute is required when action is 'Custom'.");
					break;
			}
		}

		private static int? ParseMonthName(string? value)
		{
			if (Enum.TryParse<Month>(value, out var month))
				return (int)month;
			throw new InvalidDataException($"Invalid month name: {value}");
		}

		private static TEnum? ParseEnumAttr<TEnum>(XElement element, string attr) where TEnum : struct
					=> Enum.TryParse<TEnum>(element.Attribute(attr)?.Value, true, out var result) ? result : null;

		private static int? ParseIntAttr(XElement element, string attr, bool required = false)
		{
			var val = element.Attribute(attr)?.Value;
			if (string.IsNullOrWhiteSpace(val))
			{
				if (required) throw new InvalidDataException($"Missing required attribute '{attr}'.");
				return null;
			}
			return int.TryParse(val, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
				? result
				: throw new InvalidDataException($"Invalid integer for '{attr}': {val}");
		}

		private static bool? ParseBoolAttr(XElement element, string attr)
		{
			var val = element.Attribute(attr)?.Value;
			return bool.TryParse(val, out var result) ? result : null;
		}

		/// <summary>
		/// Represents a month using names from the XSD enumeration.
		/// </summary>
		private enum Month
		{
			January = 1, February, March, April, May, June,
			July, August, September, October, November, December
		}

		internal class JsonNotableDateDefinition
		{
			public string Name { get; set; } = "";

			public string Type { get; set; } = "";

			public string Kind { get; set; } = "";

			public int? FirstYear { get; set; }

			public int? LastYear { get; set; }

			public string? CalendarType { get; set; }

			public string? Country { get; set; }

			public string? Region { get; set; }

			public int? OccurrenceYears { get; set; }

			public int? Day { get; set; }

			public int? Month { get; set; }

			public string? BaseNotableDateName { get; set; }

			public int? OffsetDays { get; set; }

			public string? ProviderAssembly { get; set; }

			public string? ProviderTypeName { get; set; }

			public bool? NonWorking { get; set; } // ➡️ NEW: matches <Definition nonWorking="..."/>

			public string? Comment { get; set; } // ➡️ (already present, keep)

			public ImmutableArray<NotableDateAdjustmentRule>? AdjustmentRules { get; set; }

			public NotableDateDefinition ToDefinition() => new()
			{
				Name = Name,
				DefinitionType = Enum.Parse<NotableDateDefinitionType>(Type, true),
				NotableDateKind = Enum.Parse<NotableDateKind>(Kind, true),
				FirstYear = FirstYear,
				LastYear = LastYear,
				CalendarType = CalendarType,
				Country = Country,
				Region = Region,
				OccurrenceYears = OccurrenceYears,
				Day = Day,
				Month = Month,
				BaseNotableDateName = BaseNotableDateName,
				OffsetDays = OffsetDays,
				ProviderAssembly = ProviderAssembly,
				ProviderTypeName = ProviderTypeName,
				NonWorking = NonWorking,     // ➡️ NEW: mapped properly
				Comment = Comment,           // ➡️ (already mapped)
				AdjustmentRules = AdjustmentRules ?? ImmutableArray<NotableDateAdjustmentRule>.Empty
			};
		}
	}
	*/
}