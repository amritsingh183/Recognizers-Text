﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Recognizers.Text.DataDrivenTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Recognizers.Text.Sequence.Tests
{
    [TestClass]
    public class TestSequence_Swedish : TestBase
    {
        [NetCoreTestDataSource]
        [TestMethod]
        public void QuotedTextModel(TestModel testSpec)
        {
            TestQuotedText(testSpec);
        }
    }
}
