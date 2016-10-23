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
