﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.RegularExpressions;

using Microsoft.Recognizers.Definitions;

namespace Microsoft.Recognizers.Text.Sequence
{
    public class BaseGUIDExtractor : BaseSequenceExtractor
    {
        public BaseGUIDExtractor()
        {
            var regexes = new Dictionary<Regex, string>
            {
                {
                    new Regex(BaseGUID.GUIDRegex, RegexOptions.None, RegexTimeOut),
                    Constants.GUID_REGEX
                },
            };

            Regexes = regexes.ToImmutableDictionary();
        }

        internal override ImmutableDictionary<Regex, string> Regexes { get; }

        protected static TimeSpan RegexTimeOut => SequenceRecognizer.GetTimeout(MethodBase.GetCurrentMethod().DeclaringType);

        protected sealed override string ExtractType { get; } = Constants.SYS_GUID;
    }
}
