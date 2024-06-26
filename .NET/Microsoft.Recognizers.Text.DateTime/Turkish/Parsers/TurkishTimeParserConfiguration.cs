﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Recognizers.Definitions.Turkish;
using Microsoft.Recognizers.Text.DateTime.Utilities;
using Microsoft.Recognizers.Text.Utilities;

namespace Microsoft.Recognizers.Text.DateTime.Turkish
{
    public class TurkishTimeParserConfiguration : BaseDateTimeOptionsConfiguration, ITimeParserConfiguration
    {
        private const RegexOptions RegexFlags = RegexOptions.Singleline | RegexOptions.ExplicitCapture;

        private static readonly Regex TimeSuffixFull =
            new Regex(DateTimeDefinitions.TimeSuffixFull, RegexFlags, RegexTimeOut);

        private static readonly Regex LunchRegex =
            new Regex(DateTimeDefinitions.LunchRegex, RegexFlags, RegexTimeOut);

        private static readonly Regex NightRegex =
            new Regex(DateTimeDefinitions.NightRegex, RegexFlags, RegexTimeOut);

        private static readonly Regex HalfTokenRegex =
            new Regex(DateTimeDefinitions.HalfTokenRegex, RegexFlags, RegexTimeOut);

        private static readonly Regex QuarterTokenRegex =
            new Regex(DateTimeDefinitions.QuarterTokenRegex, RegexFlags, RegexTimeOut);

        private static readonly Regex ToTokenRegex =
            new Regex(DateTimeDefinitions.ToTokenRegex, RegexFlags, RegexTimeOut);

        public TurkishTimeParserConfiguration(ICommonDateTimeParserConfiguration config)
         : base(config)
        {
            TimeTokenPrefix = DateTimeDefinitions.TimeTokenPrefix;
            AtRegex = TurkishTimeExtractorConfiguration.AtRegex;
            TimeRegexes = TurkishTimeExtractorConfiguration.TimeRegexList;
            UtilityConfiguration = config.UtilityConfiguration;
            Numbers = config.Numbers;
            TimeZoneParser = config.TimeZoneParser;
        }

        public string TimeTokenPrefix { get; }

        public Regex AtRegex { get; }

        public Regex MealTimeRegex { get; }

        public IEnumerable<Regex> TimeRegexes { get; }

        public IImmutableDictionary<string, int> Numbers { get; }

        public IDateTimeUtilityConfiguration UtilityConfiguration { get; }

        public IDateTimeParser TimeZoneParser { get; }

        public void AdjustByPrefix(string prefix, ref int hour, ref int min, ref bool hasMin)
        {
            int deltaMin;

            var trimedPrefix = prefix.Trim();

            if (HalfTokenRegex.IsMatch(trimedPrefix))
            {
                deltaMin = 30;
            }
            else if (QuarterTokenRegex.IsMatch(trimedPrefix))
            {
                deltaMin = 15;
            }
            else
            {
                var match = TurkishTimeExtractorConfiguration.LessThanOneHour.Match(trimedPrefix);
                var minStr = match.Groups["deltamin"].Value;
                if (!string.IsNullOrWhiteSpace(minStr))
                {
                    deltaMin = int.Parse(minStr, CultureInfo.InvariantCulture);
                }
                else
                {
                    minStr = match.Groups["deltaminnum"].Value;
                    if (!string.IsNullOrWhiteSpace(minStr))
                    {
                        deltaMin = Numbers[minStr];
                    }
                    else
                    {
                        return;
                    }

                }
            }

            if (ToTokenRegex.IsMatch(trimedPrefix))
            {
                deltaMin = -deltaMin;
            }

            min += deltaMin;
            if (min < 0)
            {
                min += 60;
                hour -= 1;
            }

            hasMin = true;
        }

        public void AdjustBySuffix(string suffix, ref int hour, ref int min, ref bool hasMin, ref bool hasAm, ref bool hasPm)
        {
            var deltaHour = 0;
            var match = TimeSuffixFull.MatchExact(suffix, trim: true);

            if (match.Success)
            {
                var matchAmStr = match.Groups[Constants.AmGroupName].Value;
                if (!string.IsNullOrEmpty(matchAmStr))
                {
                    if (hour >= Constants.HalfDayHourCount)
                    {
                        deltaHour = -Constants.HalfDayHourCount;
                    }
                    else
                    {
                        hasAm = true;
                    }
                }

                var matchPmStr = match.Groups[Constants.PmGroupName].Value;
                if (!string.IsNullOrEmpty(matchPmStr))
                {
                    if (hour < Constants.HalfDayHourCount)
                    {
                        deltaHour = Constants.HalfDayHourCount;
                    }

                    if (LunchRegex.IsMatch(matchPmStr))
                    {
                        if (hour >= 10 && hour <= Constants.HalfDayHourCount)
                        {
                            deltaHour = 0;
                            if (hour == Constants.HalfDayHourCount)
                            {
                                hasPm = true;
                            }
                            else
                            {
                                hasAm = true;
                            }
                        }
                        else
                        {
                            hasPm = true;
                        }
                    }
                    else if (NightRegex.IsMatch(matchPmStr))
                    {
                        if (hour <= 3 || hour == Constants.HalfDayHourCount)
                        {
                            if (hour == Constants.HalfDayHourCount)
                            {
                                hour = 0;
                            }

                            deltaHour = 0;
                            hasAm = true;
                        }
                        else
                        {
                            hasPm = true;
                        }
                    }
                    else
                    {
                        hasPm = true;
                    }
                }
            }

            hour = (hour + deltaHour) % 24;
        }
    }
}
