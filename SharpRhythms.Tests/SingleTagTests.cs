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

    public class SingleTagTests
    {
        [Fact]
        public void TagWithLeadingWhitespaceIsParsedCorrectly()
        {
            var input = "    #TAG:content;";
            var actual = MsdFormat.MsdTag.Parse(input);
            actual.Name.ShouldBe("TAG");
        }

        [Fact]
        public void TagWithLeadingNewlineIsParsedCorrectly()
        {
            var input = "\n#TAG:content;";
            var actual = MsdFormat.MsdTag.Parse(input);
            actual.Name.ShouldBe("TAG");
        }

        [Fact]
        public void SimpleTagIsParsedCorrectly()
        {
            var input = "#TAG:content;";
            var actual = MsdFormat.MsdTag.Parse(input);
            actual.Name.ShouldBe("TAG");
            actual.Content.ShouldHaveSingleItem();
            actual.Content.Single().ShouldBe("content");
        }

        [Fact]
        public void TagWithTrailingWhitespaceIsParsedCorrectly()
        {
            var input = "#TAG:content;    ";
            var actual = MsdFormat.MsdTag.Parse(input);
            actual.Name.ShouldBe("TAG");
        }

        [Fact]
        public void TagWithTrailingNewlineIsParsedCorrectly()
        {
            var input = "\n#TAG:content;\n";
            var actual = MsdFormat.MsdTag.Parse(input);
            actual.Name.ShouldBe("TAG");
        }

        [Fact]
        public void TagWithFollowingGarbageThrowsException()
        {
            var input = "\n#TAG:content;garbage";
            Should.Throw<ParseException>(() => MsdFormat.MsdTag.Parse(input));
        }

        [Fact]
        public void UnterminatedTagThrowsException()
        {
            var input = "\n#TAG:content";
            Should.Throw<ParseException>(() => MsdFormat.MsdTag.Parse(input));
        }
    }
}
