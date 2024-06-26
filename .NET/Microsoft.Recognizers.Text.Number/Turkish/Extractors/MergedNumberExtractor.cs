﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Text.RegularExpressions;

using Microsoft.Recognizers.Definitions.Turkish;

namespace Microsoft.Recognizers.Text.Number.Turkish
{
    internal class MergedNumberExtractor : BaseMergedNumberExtractor
    {
        private const RegexOptions RegexFlags = RegexOptions.Singleline | RegexOptions.ExplicitCapture;

        private static readonly ConcurrentDictionary<(NumberMode, NumberOptions), MergedNumberExtractor> Instances =
            new ConcurrentDictionary<(NumberMode, NumberOptions), MergedNumberExtractor>();

        public MergedNumberExtractor(NumberMode mode, NumberOptions options)
        {
            NumberExtractor = Turkish.NumberExtractor.GetInstance(mode, options);
            RoundNumberIntegerRegexWithLocks =
                new Regex(NumbersDefinitions.RoundNumberIntegerRegexWithLocks, RegexFlags, RegexTimeOut);
            ConnectorRegex =
                new Regex(NumbersDefinitions.ConnectorRegex, RegexFlags, RegexTimeOut);
        }

        public sealed override BaseNumberExtractor NumberExtractor { get; set; }

        public sealed override Regex RoundNumberIntegerRegexWithLocks { get; set; }

        public sealed override Regex ConnectorRegex { get; set; }

        public static MergedNumberExtractor GetInstance(
            NumberMode mode = NumberMode.Default,
            NumberOptions options = NumberOptions.None)
        {
            var cacheKey = (mode, options);
            if (!Instances.ContainsKey(cacheKey))
            {
                var instance = new MergedNumberExtractor(mode, options);
                Instances.TryAdd(cacheKey, instance);
            }

            return Instances[cacheKey];
        }
    }
}
