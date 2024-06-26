﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Recognizers.Definitions;
using Microsoft.Recognizers.Definitions.Dutch;
using Microsoft.Recognizers.Definitions.Utilities;
using Microsoft.Recognizers.Text.Number;
using Microsoft.Recognizers.Text.Number.Dutch;

namespace Microsoft.Recognizers.Text.NumberWithUnit.Dutch
{
    public abstract class DutchNumberWithUnitExtractorConfiguration : BaseNumberWithUnitExtractorConfiguration
    {
        private const RegexOptions RegexFlags = RegexOptions.Singleline | RegexOptions.ExplicitCapture;

        protected DutchNumberWithUnitExtractorConfiguration(CultureInfo ci)
            : base(
                  NumbersWithUnitDefinitions.CompoundUnitConnectorRegex,
                  BaseUnits.PmNonUnitRegex,
                  string.Empty,
                  RegexFlags)
        {
            this.CultureInfo = ci;

            var unitNumConfig = new BaseNumberOptionsConfiguration(ci.Name, NumberOptions.None, NumberMode.Unit);
            this.UnitNumExtractor = NumberExtractor.GetInstance(unitNumConfig);

            this.BuildPrefix = NumbersWithUnitDefinitions.BuildPrefix;
            this.BuildSuffix = NumbersWithUnitDefinitions.BuildSuffix;
            this.ConnectorToken = string.Empty;

            AmbiguityFiltersDict = DefinitionLoader.LoadAmbiguityFilters(NumbersWithUnitDefinitions.AmbiguityFiltersDict);
            TemperatureAmbiguityFiltersDict = DefinitionLoader.LoadAmbiguityFilters(NumbersWithUnitDefinitions.TemperatureAmbiguityFiltersDict);
            DimensionAmbiguityFiltersDict = DefinitionLoader.LoadAmbiguityFilters(NumbersWithUnitDefinitions.DimensionAmbiguityFiltersDict);
        }

        public override void ExpandHalfSuffix(string source, ref List<ExtractResult> result, IOrderedEnumerable<ExtractResult> numbers)
        {
        }
    }
}