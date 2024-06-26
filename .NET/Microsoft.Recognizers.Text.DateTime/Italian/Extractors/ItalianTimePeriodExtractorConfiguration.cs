﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Recognizers.Definitions.Italian;
using Microsoft.Recognizers.Text.DateTime.Italian.Utilities;
using Microsoft.Recognizers.Text.DateTime.Utilities;
using Microsoft.Recognizers.Text.Utilities;

namespace Microsoft.Recognizers.Text.DateTime.Italian
{
    public class ItalianTimePeriodExtractorConfiguration : BaseDateTimeOptionsConfiguration, ITimePeriodExtractorConfiguration
    {
        public static readonly string ExtractorName = Constants.SYS_DATETIME_TIMEPERIOD; // "TimePeriod";

        public static readonly Regex TillRegex =
            new Regex(DateTimeDefinitions.RestrictedTillRegex, RegexFlags, RegexTimeOut);

        public static readonly Regex FullTillRegex =
            new Regex(DateTimeDefinitions.TillRegex, RegexFlags, RegexTimeOut);

        public static readonly Regex HourRegex =
            new Regex(DateTimeDefinitions.HourRegex, RegexFlags, RegexTimeOut);

        public static readonly Regex PeriodHourNumRegex =
            new Regex(DateTimeDefinitions.PeriodHourNumRegex, RegexFlags, RegexTimeOut);

        public static readonly Regex PeriodDescRegex =
            new Regex(DateTimeDefinitions.PeriodDescRegex, RegexFlags, RegexTimeOut);

        public static readonly Regex PmRegex =
            new Regex(DateTimeDefinitions.PmRegex, RegexFlags, RegexTimeOut);

        public static readonly Regex AmRegex =
            new Regex(DateTimeDefinitions.AmRegex, RegexFlags, RegexTimeOut);

        public static readonly Regex PureNumFromTo =
            new Regex(DateTimeDefinitions.PureNumFromTo, RegexFlags, RegexTimeOut);

        public static readonly Regex PureNumBetweenAnd =
            new Regex(DateTimeDefinitions.PureNumBetweenAnd, RegexFlags, RegexTimeOut);

        public static readonly Regex SpecificTimeFromTo =
            new Regex(DateTimeDefinitions.SpecificTimeFromTo, RegexFlags, RegexTimeOut);

        public static readonly Regex SpecificTimeBetweenAnd =
            new Regex(DateTimeDefinitions.SpecificTimeBetweenAnd, RegexFlags, RegexTimeOut);

        public static readonly Regex PrepositionRegex =
            new Regex(DateTimeDefinitions.PrepositionRegex, RegexFlags, RegexTimeOut);

        public static readonly Regex TimeOfDayRegex =
            new Regex(DateTimeDefinitions.TimeOfDayRegex, RegexFlags, RegexTimeOut);

        public static readonly Regex SpecificTimeOfDayRegex =
            new Regex(DateTimeDefinitions.SpecificTimeOfDayRegex, RegexFlags, RegexTimeOut);

        public static readonly Regex TimeUnitRegex =
            new Regex(DateTimeDefinitions.TimeUnitRegex, RegexFlags, RegexTimeOut);

        public static readonly Regex TimeFollowedUnit =
            new Regex(DateTimeDefinitions.TimeFollowedUnit, RegexFlags, RegexTimeOut);

        public static readonly Regex TimeNumberCombinedWithUnit =
            new Regex(DateTimeDefinitions.TimeNumberCombinedWithUnit, RegexFlags, RegexTimeOut);

        public static readonly Regex GeneralEndingRegex =
            new Regex(DateTimeDefinitions.GeneralEndingRegex, RegexFlags, RegexTimeOut);

        private const RegexOptions RegexFlags = RegexOptions.Singleline | RegexOptions.ExplicitCapture;

        private static readonly Regex FromRegex =
            new Regex(DateTimeDefinitions.FromRegex, RegexFlags, RegexTimeOut);

        private static readonly Regex RangePrefixRegex =
            new Regex(DateTimeDefinitions.RangePrefixRegex, RegexFlags, RegexTimeOut);

        private static readonly Regex ConnectorAndRegex =
            new Regex(DateTimeDefinitions.ConnectorAndRegex, RegexFlags, RegexTimeOut);

        private static readonly Regex BeforeRegex =
            new Regex(DateTimeDefinitions.BeforeRegex, RegexFlags, RegexTimeOut);

        private static readonly Regex RangePmRegex =
            new Regex(DateTimeDefinitions.RangePmRegex, RegexFlags, RegexTimeOut);

        public ItalianTimePeriodExtractorConfiguration(IDateTimeOptionsConfiguration config)
            : base(config)
        {
            TokenBeforeDate = DateTimeDefinitions.TokenBeforeDate;
            SingleTimeExtractor = new BaseTimeExtractor(new ItalianTimeExtractorConfiguration(this));
            UtilityConfiguration = new ItalianDatetimeUtilityConfiguration();
            IntegerExtractor = Number.Italian.IntegerExtractor.GetInstance();
            TimeZoneExtractor = new BaseTimeZoneExtractor(new ItalianTimeZoneExtractorConfiguration(this));
        }

        public string TokenBeforeDate { get; }

        public IDateTimeUtilityConfiguration UtilityConfiguration { get; }

        public IDateTimeExtractor SingleTimeExtractor { get; }

        public IExtractor IntegerExtractor { get; }

        public IDateTimeExtractor TimeZoneExtractor { get; }

        public IEnumerable<Regex> SimpleCasesRegex => new[]
        {
            PureNumFromTo, PureNumBetweenAnd, AmRegex, RangePmRegex, SpecificTimeFromTo, SpecificTimeBetweenAnd,
        };

        public IEnumerable<Regex> PureNumberRegex => new[] { PureNumFromTo, PureNumBetweenAnd };

        bool ITimePeriodExtractorConfiguration.CheckBothBeforeAfter => DateTimeDefinitions.CheckBothBeforeAfter;

        Regex ITimePeriodExtractorConfiguration.TillRegex => TillRegex;

        Regex ITimePeriodExtractorConfiguration.TimeOfDayRegex => TimeOfDayRegex;

        Regex ITimePeriodExtractorConfiguration.GeneralEndingRegex => GeneralEndingRegex;

        public static bool HasConnectorToken(string text)
        {
            return ConnectorAndRegex.IsMatch(text);
        }

        public bool GetFromTokenIndex(string text, out int index)
        {
            index = -1;
            var fromMatch = FromRegex.Match(text);
            if (fromMatch.Success)
            {
                index = fromMatch.Index;
            }

            return fromMatch.Success;
        }

        public bool GetBetweenTokenIndex(string text, out int index)
        {
            index = -1;
            var beforeMatch = BeforeRegex.MatchEnd(text, false);
            var fromMatch = RangePrefixRegex.MatchEnd(text, false);

            if (beforeMatch.Success)
            {
                index = beforeMatch.Index;
            }
            else if (fromMatch.Success)
            {
                index = fromMatch.Index;

                return fromMatch.Success;
            }

            return beforeMatch.Success;
        }

        public bool IsConnectorToken(string text)
        {
            return ConnectorAndRegex.IsMatch(text) || FullTillRegex.IsExactMatch(text, false);
        }

        public List<ExtractResult> ApplyPotentialPeriodAmbiguityHotfix(string text, List<ExtractResult> timePeriodErs) => TimePeriodFunctions.ApplyPotentialPeriodAmbiguityHotfix(text, timePeriodErs);
    }
}
