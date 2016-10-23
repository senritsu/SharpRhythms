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