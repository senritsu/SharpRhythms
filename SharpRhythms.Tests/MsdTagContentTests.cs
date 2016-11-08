/***************************************************************************\
The MIT License (MIT)

Copyright (c) 2016 senritsu (https://github.com/senritsu)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
\***************************************************************************/

namespace SharpRhythms.Tests
{
    using System.Linq;
    using Parsers.Msd.Base;
    using Shouldly;
    using Sprache;
    using Xunit;

    public class MsdTagContentTests
    {
        [Fact]
        public void MultipartContentIsParsedCorrectly()
        {
            var input = "#TAG:content1;content2;content3;";
            var actual = MsdParser.MsdTag.Parse(input).Content;
            actual.ShouldBe("content1;content2;content3");
        }

        [Fact]
        public void EmptyContentIsParsedCorrectly()
        {
            var input = "#TAG:;";
            var actual = MsdParser.MsdTag.Parse(input);
            actual.Content.ShouldBe("");
        }

        [Fact]
        public void ContentWithNewlinesIsParsedCorrectly()
        {
            var input = "#TAG:a\nb\n;";
            var actual = MsdParser.MsdTag.Parse(input);
            actual.Content.ShouldBe("a\nb\n");
        }

        [Fact]
        public void ContentWithHashesIsParsedCorrectly()
        {
            var input = "#TAG:a#b;";
            var actual = MsdParser.MsdTag.Parse(input);
            actual.Content.ShouldBe("a#b");
        }

        [Fact]
        public void ComplexContentIsParsedCorrectly()
        {
            var input = "#TAG:a:b:c:d;";
            var tag = MsdParser.MsdTag.Parse(input);
            var actual = MsdTagContentParser.ComplexContent.Parse(tag.Content);
            actual.ShouldBe(new [] {"a", "b", "c", "d"});
        }

        [Fact]
        public void ListContentIsParsedCorrectly()
        {
            var input = "#TAG:a,b,c,d;";
            var tag = MsdParser.MsdTag.Parse(input);
            var actual = MsdTagContentParser.ListContent(Parse.Letter.Many().Text()).Parse(tag.Content);
            actual.ShouldBe(new[] { "a", "b", "c", "d" });
        }

        [Fact]
        public void ListContentWithNewlinesIsParsedCorrectly()
        {
            var input = @"#TAG:a
,b
,
c
,
d;";
            var tag = MsdParser.MsdTag.Parse(input);
            var actual = MsdTagContentParser.ListContent(Parse.Letter.Many().Text()).Parse(tag.Content);
            actual.ShouldBe(new[] { "a", "b", "c", "d" });
        }

        [Fact]
        public void DoubleContentIsParsedCorrectly()
        {
            var input = "#TAG:0.000;";
            var tag = MsdParser.MsdTag.Parse(input);
            var actual = tag.AsDouble();
            actual.ShouldBe(0);
        }

        [Fact]
        public void TimeIndexedValueIsParsedCorrectly()
        {
            var input = "#TAG:1.000=2.000;";
            var tag = MsdParser.MsdTag.Parse(input);
            var actual = MsdTagContentParser.TimeIndexedValue.Parse(tag.Content);
            actual.Time.ShouldBe(1);
            actual.Value.ShouldBe(2);
        }

        [Fact]
        public void TimeIndexedValueListIsParsedCorrectly()
        {
            var input = "#TAG:1.0=2.0,3.0=4.0,5.0=6.0;";
            var tag = MsdParser.MsdTag.Parse(input);
            var actual =
                MsdTagContentParser.ListContent(MsdTagContentParser.TimeIndexedValue)
                    .Parse(tag.Content)
                    .ToArray();
            actual.Length.ShouldBe(3);
            actual[0].Time.ShouldBe(1);
            actual[0].Value.ShouldBe(2);
            actual[1].Time.ShouldBe(3);
            actual[1].Value.ShouldBe(4);
            actual[2].Time.ShouldBe(5);
            actual[2].Value.ShouldBe(6);
        }
    }
}