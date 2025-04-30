/* Unmerged change from project 'Bodu.Globalization.Calendar (net7.0)'
Before:
using System;
After:
using Bodu.Collections.Generic;
using Bodu.Extensions;
using System;
*/

using Bodu.Extensions;
using System.Collections.Concurrent;
using System.Data;

/* Unmerged change from project 'Bodu.Globalization.Calendar (net7.0)'
Before:
using Bodu.Collections.Generic;
using Bodu.Extensions;

using SysGlobal = System.Globalization;
After:
using SysGlobal = System.Globalization;
*/

namespace Bodu.Globalization.Calendar
{
	/// <summary>
	/// Provides a service for managing and querying notable dates based on predefined definitions. Dynamically generates and caches notable
	/// dates as needed when queries fall outside generated bounds.
	/// </summary>
	public sealed class NotableDateService : INotableDateService
	{
		private const string DefaultResourceName = "Bodu.Globalization.Calendar.NotableDates.xml";

		private readonly IReadOnlyList<NotableDateDefinition> _definitions;
		private readonly ConcurrentDictionary<DateTime, List<NotableDate>> _dateCache;
		private readonly CalendarWeekendDefinition _weekendDefinition;
		private readonly IWeekendDefinitionProvider? _weekendProvider;
		private readonly NotableDateResolver _resolver;
		private readonly NotableDateAdjuster _adjuster;
		private readonly object _generationLock = new();
		private readonly int _maximumOffsetDays;
		private DateTime minGeneraged = DateTime.MaxValue;
		private DateTime maxGenerated = DateTime.MinValue;

		private bool _isGeneratingDates;
		private readonly List<NotableDate> _inProgressDates = new();

		/// <summary>
		/// Initializes a new instance of the <see cref="NotableDateService" /> class using the default embedded calendar metadata.
		/// </summary>
		public NotableDateService()
			: this(new[] { new XmlResourceCalendarMetadataSource(DefaultResourceName) }, CalendarWeekendDefinition.SaturdaySunday, null)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="NotableDateService" /> class.
		/// </summary>
		/// <param name="definitionProviders">Sources of notable date definitions.</param>
		/// <param name="weekendDefinition">Weekend definition to apply when evaluating weekends.</param>
		/// <param name="weekendProvider">Optional custom weekend provider.</param>
		public NotableDateService(IEnumerable<INotableDateDefinitionProvider> definitionProviders,
								  CalendarWeekendDefinition weekendDefinition,
								  IWeekendDefinitionProvider? weekendProvider = null)
		{
			ThrowHelper.ThrowIfNull(definitionProviders);
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekendDefinition);
			ThrowHelper.ThrowIfConditionallyRequiredParameterIsNull(weekendProvider, weekendDefinition, CalendarWeekendDefinition.Custom);

			_definitions = definitionProviders.SelectMany(p => p.LoadDefinitions()).ToList();
			_weekendDefinition = weekendDefinition;
			_weekendProvider = weekendProvider;

			_resolver = new NotableDateResolver(_definitions);
			_adjuster = new NotableDateAdjuster(IsWeekend, IsNonWorkingDay, _weekendDefinition, _weekendProvider);
			_maximumOffsetDays = CalculateMaximumOffsetDays(_definitions);

			_dateCache = new ConcurrentDictionary<DateTime, List<NotableDate>>();
		}

		private static int CalculateMaximumOffsetDays(IEnumerable<NotableDateDefinition> definitions)
		{
			return definitions
				.SelectMany(def =>
					Enumerable.Concat(
						def.DefinitionType == NotableDateDefinitionType.OffsetFrom && def.OffsetDays.HasValue
							? new[] { Math.Abs(def.OffsetDays.Value) }
							: Array.Empty<int>(),
						def.AdjustmentRules.Select(r => Math.Abs(r.OffsetDays))
					))
				.DefaultIfEmpty(0)
				.Max();
		}

		/// <inheritdoc />
		public bool IsWeekend(DateTime date) =>
			date.IsWeekend(_weekendDefinition, _weekendProvider);

		/// <inheritdoc />
		public bool IsNonWorkingDay(DateTime date, string? territoryCode = null, Type? calendarType = null)
		{
			var targetDate = date.Date;
			IEnumerable<NotableDate> candidates;

			if (_isGeneratingDates)
			{
				candidates = _dateCache.TryGetValue(targetDate, out var committed)
					? committed.Concat(_inProgressDates.Where(d => d.Date == targetDate))
					: _inProgressDates.Where(d => d.Date == targetDate);
			}
			else
			{
				EnsureDatesGenerated(targetDate, targetDate);

				candidates = _dateCache.TryGetValue(targetDate, out var found)
					? found
					: Enumerable.Empty<NotableDate>();
			}

			return candidates.Any(d => MatchesContext(d, territoryCode, calendarType) && d.NonWorking)
				|| IsWeekend(targetDate);
		}

		/// <inheritdoc />
		public IReadOnlyList<NotableDate> GetNotableDates(int year, string? territoryCode = null, Type? calendarType = null)
		{
			var start = new DateTime(year, 1, 1);
			var end = new DateTime(year, 12, 31);
			return GetNotableDates(start, end, territoryCode, calendarType);
		}

		/// <inheritdoc />
		public IReadOnlyList<NotableDate> GetNotableDates(DateTime startDate, DateTime endDate, string? territoryCode = null, Type? calendarType = null)
		{
			EnsureDatesGenerated(startDate.Date, endDate.Date);

			return _dateCache
				.Where(kv => kv.Key >= startDate.Date && kv.Key <= endDate.Date)
				.SelectMany(kv => kv.Value)
				.Where(d => MatchesContext(d, territoryCode, calendarType))
				.OrderBy(d => d.Date)
				.ToList();
		}

		/// <inheritdoc />
		public IReadOnlyList<NotableDate> GetNotableDates(DateTime date, string? territoryCode = null, Type? calendarType = null)
		{
			return GetNotableDates(date.Date, date.Date, territoryCode, calendarType);
		}

		private void EnsureDatesGenerated(DateTime startDate, DateTime endDate)
		{
			if (startDate >= minGeneraged && endDate <= maxGenerated)
				return;

			lock (_generationLock)
			{
				var generateStart = ComparableHelper.Min(startDate, minGeneraged);
				var generateEnd = ComparableHelper.Max(endDate, maxGenerated);

				GenerateDatesForRange(generateStart, generateEnd);

				minGeneraged = startDate;
				maxGenerated = endDate;
			}
		}

		private void GenerateDatesForRange(DateTime startDate, DateTime endDate)
		{
			_isGeneratingDates = true;
			try
			{
				_inProgressDates.Clear();

				DateTimeExtensions.GetDateParts(startDate.AddDays(-_maximumOffsetDays), out int startYear, out int startMonth, out _);
				DateTimeExtensions.GetDateParts(endDate.AddDays(_maximumOffsetDays), out int endYear, out int endMonth, out _);

				for (int year = startYear; year <= endYear; year++)
				{
					foreach (var definition in _definitions)
					{
						if ((definition.FirstYear.HasValue && year < definition.FirstYear.Value) ||
							(definition.LastYear.HasValue && year > definition.LastYear.Value))
							continue;

						var baseDate = _resolver.ResolveBaseDate(definition, year);
						if (baseDate is null)
							continue;

						var territoryCodes = definition.TerritoryCode?.Length > 0
							? definition.TerritoryCode.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
							: new[] { (string?)null }; // No territory specified

						foreach (var territory in territoryCodes)
						{
							var baseNotable = new NotableDate
							{
								Date = baseDate.Value,
								Name = definition.Name,
								Kind = definition.NotableDateKind,
								NonWorking = definition.NonWorking ?? false,
								TerritoryCode = territory,
								CalendarType = definition.CalendarType,
								Comment = definition.Comment
							};

							foreach (var rule in definition.AdjustmentRules.OrderBy(r => r.Priority))
							{
								var ruleTerritories = rule.TerritoryCode?.Length > 0
									? rule.TerritoryCode.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()
									: new[] { (string?)null }; // No territory specified

								foreach (var ruleTerritory in ruleTerritories)
								{
									// Clone baseNotable and override Territory for rule if needed
									var ruleBase = baseNotable with { TerritoryCode = ruleTerritory ?? baseNotable.TerritoryCode };

									var (success, adjustedDate) = _adjuster.Apply(rule, ruleBase.Date);

									if (!success || adjustedDate.Date == ruleBase.Date.Date)
										continue;

									_inProgressDates.Add(ruleBase with
									{
										Date = adjustedDate,
										OriginalDate = ruleBase.Date,
										NonWorking = rule.NonWorking ?? ruleBase.NonWorking
									});
								}
							}

							_inProgressDates.Add(baseNotable);
						}
					}
				}

				foreach (var notable in _inProgressDates.Where(d => d.Date.IsInRange(startDate, endDate)))
				{
					AddToCache(notable);
				}
			}
			finally
			{
				_isGeneratingDates = false;
				_inProgressDates.Clear();
			}
		}

		/*
		private void GenerateDatesForRange(DateTime startDate, DateTime endDate)
		{
			_isGeneratingDates = true;
			try
			{
				_inProgressDates.Clear();

				DateTimeExtensions.GetDateParts(startDate.AddDays(-_maximumOffsetDays), out int startYear, out int startMonth, out _);
				DateTimeExtensions.GetDateParts(endDate.AddDays(_maximumOffsetDays), out int endYear, out int endMonth, out _);

				for (int year = startYear; year <= endYear; year++)
				{
					foreach (var definition in _definitions)
					{
						if ((definition.FirstYear.HasValue && year < definition.FirstYear.Value) ||
							(definition.LastYear.HasValue && year > definition.LastYear.Value))
							continue;

						var baseDate = _resolver.ResolveBaseDate(definition, year);
						if (baseDate == null)
							continue;

						var baseNotable = new NotableDate
						{
							Date = baseDate.Value,
							Name = definition.Name,
							Kind = definition.NotableDateKind,
							NonWorking = definition.NonWorking ?? false,
							TerritoryCode = definition.TerritoryCode,
							CalendarType = definition.CalendarType,
							Comment = definition.Comment
						};

						foreach (var rule in definition.AdjustmentRules.OrderBy(r => r.Priority))
						{
							var (success, adjustedDate) = _adjuster.Apply(rule, baseNotable.Date);

							if (!success || adjustedDate.Date == baseNotable.Date.Date)
								continue;

							_inProgressDates.Add(baseNotable with
							{
								Date = adjustedDate,
								OriginalDate = baseNotable.Date,
								NonWorking = rule.NonWorking ?? baseNotable.NonWorking
							});
						}

						_inProgressDates.Add(baseNotable);
					}
				}

				foreach (var notable in _inProgressDates.Where(d => d.Date.IsInRange(startDate, endDate)))
				{
					AddToCache(notable);
				}
			}
			finally
			{
				_isGeneratingDates = false;
				_inProgressDates.Clear();
			}
		}
		*/

		private void AddToCache(NotableDate notableDate)
		{
			var date = notableDate.Date.Date;
			_dateCache.AddOrUpdate(
				date,
				_ => new List<NotableDate> { notableDate },
				(_, list) =>
				{
					lock (list)
					{
						if (!list.Contains(notableDate))
						{
							list.Add(notableDate);
						}
						return list;
					}
				});
		}

		/// <summary>
		/// Determines whether a notable date matches the given territory and calendar context.
		/// </summary>
		/// <param name="date">The notable date to evaluate.</param>
		/// <param name="territoryCode">
		/// The requested ISO country code or country-region code (e.g., "AU", "AU-NSW"). If <c>null</c> or empty, matches all.
		/// </param>
		/// <param name="calendarType">Optional calendar type constraint. If <c>null</c>, matches all calendar types.</param>
		/// <returns><c>true</c> if the date matches the context; otherwise, <c>false</c>.</returns>
		private static bool MatchesContext(NotableDate date, string? territoryCode, Type? calendarType) =>
			(

				// No territory filter requested → match everything
				string.IsNullOrEmpty(territoryCode) ||

				// Date has no specific territory → match (considered general)
				string.IsNullOrEmpty(date.TerritoryCode) ||

				// Exact territory match (e.g., "AU" == "AU")
				date.TerritoryCode.Equals(territoryCode, StringComparison.OrdinalIgnoreCase) ||

				// Subregion match (e.g., "AU-NSW" starts with "AU-")
				date.TerritoryCode.StartsWith(territoryCode + "-", StringComparison.OrdinalIgnoreCase)
			) &&
			(

				// No calendar constraint → match
				calendarType == null ||

				// Date has no specific calendar → match
				date.CalendarType == null ||

				// Exact calendar type match (e.g., Gregorian == Gregorian)
				date.CalendarType == calendarType
			);
	}
}