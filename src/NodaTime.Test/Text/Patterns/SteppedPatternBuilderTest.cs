﻿// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Text;
using NodaTime.Globalization;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Text.Patterns
{
    /// <summary>
    /// Tests for SteppedPatternBuilder, often using OffsetPatternParser as this is known
    /// to use SteppedPatternBuilder.
    /// </summary>
    public class SteppedPatternBuilderTest
    {
        private static readonly IPartialPattern<Offset> SimpleOffsetPattern =
            (IPartialPattern<Offset>)new OffsetPatternParser().ParsePattern("HH:mm", NodaFormatInfo.InvariantInfo);

        [Test]
        public void ParsePartial_ValidInMiddle()
        {
            var value = new ValueCursor("x17:30y");
            value.MoveNext();
            value.MoveNext();
            // Start already looking at the value to parse
            Assert.AreEqual('1', value.Current);
            var result = SimpleOffsetPattern.ParsePartial(value);
            Assert.AreEqual(Offset.FromHoursAndMinutes(17, 30), result.Value);
            // Finish just after the value
            Assert.AreEqual('y', value.Current);
        }

        [Test]
        public void ParsePartial_ValidAtEnd()
        {
            var value = new ValueCursor("x17:30");
            value.MoveNext();
            value.MoveNext();
            var result = SimpleOffsetPattern.ParsePartial(value);
            Assert.AreEqual(Offset.FromHoursAndMinutes(17, 30), result.Value);
            // Finish just after the value, which in this case is at the end.
            Assert.AreEqual(TextCursor.Nul, value.Current);
        }

        [Test]
        public void Parse_Partial_Invalid()
        {
            var value = new ValueCursor("x17:y");
            value.MoveNext();
            value.MoveNext();
            var result = SimpleOffsetPattern.ParsePartial(value);
            Assert.Throws<UnparsableValueException>(() => result.GetValueOrThrow());
        }

        [Test]
        public void AppendFormat()
        {
            var builder = new StringBuilder("x");
            var offset = Offset.FromHoursAndMinutes(17, 30);
            SimpleOffsetPattern.AppendFormat(offset, builder);
            Assert.AreEqual("x17:30", builder.ToString());
        }
    }
}
