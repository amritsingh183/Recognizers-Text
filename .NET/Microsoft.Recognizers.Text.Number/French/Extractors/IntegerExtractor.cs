﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

using Microsoft.Recognizers.Definitions.French;

namespace Microsoft.Recognizers.Text.Number.French
{
    public class IntegerExtractor : CachedNumberExtractor
    {

        private const RegexOptions RegexFlags = RegexOptions.Singleline | RegexOptions.ExplicitCapture;

        private static readonly ConcurrentDictionary<string, IntegerExtractor> Instances =
            new ConcurrentDictionary<string, IntegerExtractor>();

        private readonly string keyPrefix;

        private IntegerExtractor(BaseNumberOptionsConfiguration config)
            : base(config.Options)
        {

            keyPrefix = string.Intern(ExtractType + "_" + config.Options + "_" + config.Placeholder + "_" + config.Culture);

            this.Regexes = new Dictionary<Regex, TypeTag>
            {
                {
                    new Regex(NumbersDefinitions.NumbersWithPlaceHolder(config.Placeholder), RegexFlags, RegexTimeOut),
                    RegexTagGenerator.GenerateRegexTag(Constants.INTEGER_PREFIX, Constants.NUMBER_SUFFIX)
                },
                {
                    new Regex(NumbersDefinitions.NumbersWithSuffix, RegexFlags, RegexTimeOut),
                    RegexTagGenerator.GenerateRegexTag(Constants.INTEGER_PREFIX, Constants.NUMBER_SUFFIX)
                },
                {
                    GenerateLongFormatNumberRegexes(LongFormatType.IntegerNumDot, config.Placeholder),
                    RegexTagGenerator.GenerateRegexTag(Constants.INTEGER_PREFIX, Constants.NUMBER_SUFFIX)
                },
                {
                    new Regex(NumbersDefinitions.RoundNumberIntegerRegexWithLocks, RegexFlags, RegexTimeOut),
                    RegexTagGenerator.GenerateRegexTag(Constants.INTEGER_PREFIX, Constants.NUMBER_SUFFIX)
                },
                {
                    new Regex(NumbersDefinitions.NumbersWithDozenSuffix, RegexFlags, RegexTimeOut),
                    RegexTagGenerator.GenerateRegexTag(Constants.INTEGER_PREFIX, Constants.NUMBER_SUFFIX)
                },
                {
                    new Regex(NumbersDefinitions.AllIntRegexWithLocks, RegexFlags, RegexTimeOut),
                    RegexTagGenerator.GenerateRegexTag(Constants.INTEGER_PREFIX, Constants.FRENCH)
                },
                {
                    new Regex(NumbersDefinitions.AllIntRegexWithDozenSuffixLocks, RegexFlags, RegexTimeOut),
                    RegexTagGenerator.GenerateRegexTag(Constants.INTEGER_PREFIX, Constants.FRENCH)
                },
                {
                    GenerateLongFormatNumberRegexes(LongFormatType.IntegerNumBlank, config.Placeholder, RegexFlags),
                    RegexTagGenerator.GenerateRegexTag(Constants.INTEGER_PREFIX, Constants.NUMBER_SUFFIX)
                },
                {
                    GenerateLongFormatNumberRegexes(LongFormatType.IntegerNumNoBreakSpace, config.Placeholder, RegexFlags),
                    RegexTagGenerator.GenerateRegexTag(Constants.INTEGER_PREFIX, Constants.NUMBER_SUFFIX)
                },
            }.ToImmutableDictionary();
        }

        internal sealed override ImmutableDictionary<Regex, TypeTag> Regexes { get; }

        protected sealed override string ExtractType { get; } = Constants.SYS_NUM_INTEGER; // "Integer";

        public static IntegerExtractor GetInstance(BaseNumberOptionsConfiguration config)
        {

            var extractorKey = config.Placeholder;

            if (!Instances.ContainsKey(extractorKey))
            {
                var instance = new IntegerExtractor(config);
                Instances.TryAdd(extractorKey, instance);
            }

            return Instances[extractorKey];
        }

        protected override object GenKey(string input)
        {
            return (keyPrefix, input);
        }
    }
}
