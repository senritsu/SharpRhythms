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
