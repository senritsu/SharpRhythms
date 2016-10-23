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

    public class MultiTagTests
    {
        [Fact]
        public void TagWithFollowingTagIsParsedCorrectly()
        {
            var input = "#TAG:content;  \n #OTHER:other content;";
            var actual = MsdFormat.MsdTag.Parse(input);
            actual.Name.ShouldBe("TAG");
            actual.Content.ShouldHaveSingleItem();
            actual.Content.Single().ShouldBe("content");
        }

        [Fact]
        public void TagFollowingAnotherTagIsParsedCorrectly()
        {
            var input = "#OTHER:other content;  \n #TAG:content;";
            var parser =
                from tag1 in MsdFormat.MsdTag
                from tag2 in MsdFormat.MsdTag
                select tag2;

            var actual = parser.Parse(input);
            actual.Name.ShouldBe("TAG");
            actual.Content.ShouldHaveSingleItem();
            actual.Content.Single().ShouldBe("content");
        }

        [Fact]
        public void MultipleTagsAreParsedCorrectly()
        {
            var input = @"
#FIRST:first content;
#SECOND:second content;
#THIRD:third content;
#FOURTH:fourth content;
";
            var actual = MsdFormat.Parser.Parse(input).ToArray();
            actual.Length.ShouldBe(4);
            actual.Select(x => x.Name).ShouldBe(new[] {"FIRST", "SECOND", "THIRD", "FOURTH"});
            actual.Select(x => x.Content).ShouldAllBe(x => x.Count() == 1);
            actual.Select(x => x.Content.Single())
                .ShouldBe(new[] {"first content", "second content", "third content", "fourth content"});
        }
    }
}
