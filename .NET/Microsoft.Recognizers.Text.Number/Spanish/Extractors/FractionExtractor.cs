﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

using Microsoft.Recognizers.Definitions.Spanish;

namespace Microsoft.Recognizers.Text.Number.Spanish
{
    public class FractionExtractor : BaseNumberExtractor
    {

        private const RegexOptions RegexFlags = RegexOptions.Singleline | RegexOptions.ExplicitCapture;

        private static readonly ConcurrentDictionary<(NumberMode, NumberOptions), FractionExtractor> Instances =
            new ConcurrentDictionary<(NumberMode, NumberOptions), FractionExtractor>();

        private FractionExtractor(BaseNumberOptionsConfiguration config)
            : base(config.Options)
        {

            var regexes = new Dictionary<Regex, TypeTag>
            {
                {
                    new Regex(NumbersDefinitions.FractionNotationWithSpacesRegex, RegexFlags, RegexTimeOut),
                    RegexTagGenerator.GenerateRegexTag(Constants.FRACTION_PREFIX, Constants.NUMBER_SUFFIX)
                },
                {
                    new Regex(NumbersDefinitions.FractionNotationRegex, RegexFlags, RegexTimeOut),
                    RegexTagGenerator.GenerateRegexTag(Constants.FRACTION_PREFIX, Constants.NUMBER_SUFFIX)
                },
                {
                    new Regex(NumbersDefinitions.FractionNounRegex, RegexFlags, RegexTimeOut),
                    RegexTagGenerator.GenerateRegexTag(Constants.FRACTION_PREFIX, Constants.SPANISH)
                },
                {
                    new Regex(NumbersDefinitions.FractionNounWithArticleRegex, RegexFlags, RegexTimeOut),
                    RegexTagGenerator.GenerateRegexTag(Constants.FRACTION_PREFIX, Constants.SPANISH)
                },
            };

            // Not add FractionPrepositionRegex when the mode is Unit to avoid wrong recognize cases like "$1000 over 3"
            if (config.Mode != NumberMode.Unit)
            {
                regexes.Add(
                    new Regex(NumbersDefinitions.FractionPrepositionRegex, RegexFlags, RegexTimeOut),
                    RegexTagGenerator.GenerateRegexTag(Constants.FRACTION_PREFIX, Constants.SPANISH));
            }

            Regexes = regexes.ToImmutableDictionary();
        }

        internal sealed override ImmutableDictionary<Regex, TypeTag> Regexes { get; }

        protected sealed override string ExtractType { get; } = Constants.SYS_NUM_FRACTION; // "Fraction";

        public static FractionExtractor GetInstance(BaseNumberOptionsConfiguration config)
        {
            var extractorKey = (config.Mode, config.Options);

            if (!Instances.ContainsKey(extractorKey))
            {
                var instance = new FractionExtractor(config);
                Instances.TryAdd(extractorKey, instance);
            }

            return Instances[extractorKey];
        }
    }
}