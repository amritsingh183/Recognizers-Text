﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

using Microsoft.Recognizers.Definitions.Chinese;
using Microsoft.Recognizers.Definitions.Utilities;

namespace Microsoft.Recognizers.Text.DateTime.Chinese
{
    public class ChineseTimePeriodExtractorConfiguration : BaseDateTimeOptionsConfiguration, ICJKTimePeriodExtractorConfiguration
    {
        public const string TimePeriodConnectWords = DateTimeDefinitions.TimePeriodTimePeriodConnectWords;

        // 五点十分四十八秒
        public static readonly string CJKTimeRegex = ChineseTimeExtractorConfiguration.CJKTimeRegex;

        // 六点 到 九点 | 六 到 九点
        public static readonly string LeftCJKTimeRegex = DateTimeDefinitions.TimePeriodLeftCJKTimeRegex;

        public static readonly string RightCJKTimeRegex = DateTimeDefinitions.TimePeriodRightCJKTimeRegex;

        // 2:45
        public static readonly string DigitTimeRegex = ChineseTimeExtractorConfiguration.DigitTimeRegex;

        public static readonly string LeftDigitTimeRegex = DateTimeDefinitions.TimePeriodLeftDigitTimeRegex;

        public static readonly string RightDigitTimeRegex = DateTimeDefinitions.TimePeriodRightDigitTimeRegex;

        public static readonly string ShortLeftCJKTimeRegex = DateTimeDefinitions.TimePeriodShortLeftCJKTimeRegex;

        public static readonly string ShortLeftDigitTimeRegex = DateTimeDefinitions.TimePeriodShortLeftDigitTimeRegex;

        private const RegexOptions RegexFlags = RegexOptions.Singleline | RegexOptions.ExplicitCapture;

        public ChineseTimePeriodExtractorConfiguration(IDateTimeOptionsConfiguration config)
            : base(config)
        {
            var regexes = new Dictionary<Regex, PeriodType>
            {
                {
                    new Regex(DateTimeDefinitions.TimePeriodRegexes1, RegexFlags, RegexTimeOut),
                    PeriodType.FullTime
                },
                {
                    new Regex(DateTimeDefinitions.TimePeriodRegexes2, RegexFlags, RegexTimeOut),
                    PeriodType.ShortTime
                },
                {
                    new Regex(DateTimeDefinitions.TimeOfDayRegex, RegexFlags, RegexTimeOut),
                    PeriodType.ShortTime
                },
            };

            Regexes = regexes.ToImmutableDictionary();
        }

        public ImmutableDictionary<Regex, PeriodType> Regexes { get; }

        public Dictionary<Regex, Regex> AmbiguityTimePeriodFiltersDict => DefinitionLoader.LoadAmbiguityFilters(DateTimeDefinitions.AmbiguityTimePeriodFiltersDict);

    }
}