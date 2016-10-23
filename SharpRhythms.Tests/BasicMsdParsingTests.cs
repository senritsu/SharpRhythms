namespace SharpRhythms.Tests
{
    using System.Linq;
    using Parsers;
    using Sprache;
    using Xunit;

    public class BasicMsdParsingTests
    {
        [Fact]
        public void SimpleMsdTagIsParsedCorrectly()
        {
            var input = "#TAG:content;";
            var actual = MsdFormat.MsdTag.Parse(input);
            Assert.Equal("TAG", actual.Name);
            Assert.Equal("content", actual.Content.Single());
        }

        [Fact]
        public void MsdTagTerminatorIsParsedCorrectly()
        {
            foreach (
                var input in
                    new[]
                    {
                        "#TAG:content;#OTHER...;",
                        "#TAG:content;\n#OTHER...",
                        "#TAG:content;",
                        "#TAG:content;\n\n   ",
                        "#TAG:content\n\n;\n"
                    })
            {
                var actual = MsdFormat.MsdTag.Parse(input);
                Assert.Equal("TAG", actual.Name);
                Assert.Equal("content", actual.Content.Single());
            }
        }

        [Fact]
        public void SimpleMsdTagWithMultiContentIsParsedCorrectly()
        {
            var input = "#TAG:content1;content2;";
            var actual = MsdFormat.MsdTag.Parse(input);
            Assert.Equal("TAG", actual.Name);
            Assert.Equal("content1", actual.Content.First());
            Assert.Equal("content2", actual.Content.Last());
        }

        [Fact]
        public void SimpleMsdTagWithFollowingTagIsParsedCorrectly()
        {
            var input = "#TAG:content1;\n#OTHERTAG:content2;";
            var actual = MsdFormat.MsdTag.Parse(input);
            Assert.Equal("TAG", actual.Name);
            Assert.Equal("content1", actual.Content.Single());
        }
    }
}
