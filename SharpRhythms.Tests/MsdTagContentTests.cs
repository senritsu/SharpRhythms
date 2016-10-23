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
    using Parsers;
    using Shouldly;
    using Sprache;
    using Xunit;

    public class MsdTagContentTests
    {
        [Fact]
        public void MultipartContentIsParsedCorrectly()
        {
            var input = "#TAG:content1;content2;content3;";
            var actual = MsdFormat.MsdTag.Parse(input).Content.ToArray();
            actual.ShouldBe(new [] {"content1", "content2", "content3"});
        }

        [Fact]
        public void EmptyContentIsParsedCorrectly()
        {
            var input = "#TAG:;";
            var actual = MsdFormat.MsdTag.Parse(input);
            actual.Content.ShouldHaveSingleItem();
            actual.Content.Single().ShouldBe("");
        }

        [Fact]
        public void ContentWithNewlinesIsParsedCorrectly()
        {
            var input = "#TAG:a\nb\n;";
            var actual = MsdFormat.MsdTag.Parse(input);
            actual.Content.ShouldHaveSingleItem();
            actual.Content.Single().ShouldBe("a\nb\n");
        }

        [Fact]
        public void ContentWithHashesIsParsedCorrectly()
        {
            var input = "#TAG:a#b;";
            var actual = MsdFormat.MsdTag.Parse(input);
            actual.Content.ShouldHaveSingleItem();
            actual.Content.Single().ShouldBe("a#b");
        }
    }
}