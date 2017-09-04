using pappab0t.Models;
using Xunit;

namespace pappab0t.Tests.Models
{
    public abstract class UrlMatchDataTests
    {
        public class ToString : UrlMatchDataTests
        {
            [Theory]
            [InlineData("http://imgur.com/")]
            [InlineData("http://imgur.com/folder/")]
            [InlineData("http://imgur.com/folder/file.txt")]
            [InlineData("http://imgur.com/folder/file.txt?param")]
            [InlineData("http://imgur.com/folder/file.txt?param#anc")]
            public void Should_return_original_url(string url)
            {
                var umd = new UrlParser().Parse(url);

                Assert.Equal(url,umd.ToString());
            }
        }

        public class Constructor : UrlMatchDataTests
        {
            [Fact]
            public void Should_set_protocol_to_http()
            {
                var umd = new UrlMatchData();
                Assert.Equal("http", umd.Protocol);
            }
        }
    }
}