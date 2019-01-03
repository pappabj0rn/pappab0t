using pappab0t.Models;
using Xunit;

namespace pappab0t.Tests
{
    public abstract class UrlParserTests
    {
        private readonly IUrlParser _parser = new UrlParser();

        public class Parse : UrlParserTests
        {
            [Fact]
            public void Should_return_empty_result_for_string_missing_protocol()
            {
                var result = _parser.Parse("imgur.com");

                Assert.Equal(UrlMatchData.Empty, result);
            }

            [Theory]
            [InlineData("http://imgur.com","http")]
            [InlineData("https://imgur.com", "https")]
            [InlineData("ftp://imgur.com", "ftp")]
            public void Should_parse_protocol(string url, string protocol)
            {
                var result = _parser.Parse(url);

                Assert.Equal(protocol, result.Protocol);
            }

            [Theory]
            [InlineData("http://imgur.com", "imgur.com")]
            [InlineData("http://imgur.com/", "imgur.com")]
            public void Should_parse_domain(string url, string domain)
            {
                var result = _parser.Parse(url);

                Assert.Equal(domain, result.Domain);
            }

            [Theory]
            [InlineData("http://imgur.com/file.txt", "file.txt")]
            [InlineData("http://imgur.com/folder/file.txt", "file.txt")]
            [InlineData("http://imgur.com/folder/folder1/file.txt", "file.txt")]
            public void Should_parse_file_name(string url, string fileName)
            {
                var result = _parser.Parse(url);

                Assert.Equal(fileName, result.FileName);
            }

            [Theory]
            [InlineData("http://imgur.com", "/")]
            [InlineData("http://imgur.com/", "/")]
            [InlineData("http://imgur.com/file.txt", "/")]
            [InlineData("http://imgur.com/folder", "/folder/")]
            [InlineData("http://imgur.com/folder/", "/folder/")]
            [InlineData("http://imgur.com/folder/file.txt", "/folder/")]
            [InlineData("http://imgur.com/folder/folder2", "/folder/folder2/")]
            [InlineData("http://imgur.com/folder/folder2/", "/folder/folder2/")]
            [InlineData("http://imgur.com/folder/folder2/file.txt", "/folder/folder2/")]
            [InlineData("http://imgur.com/folder/folder2/#anc", "/folder/folder2/")]
            [InlineData("http://imgur.com/folder/folder2/file.txt#anc", "/folder/folder2/")]
            public void Should_parse_path(string url, string path)
            {
                var result = _parser.Parse(url);

                Assert.Equal(path, result.Path);
            }

            [Theory]
            [InlineData("http://imgur.com/?foo=bar", "foo=bar")]
            [InlineData("http://imgur.com/file.txt?foo=bar", "foo=bar")]
            [InlineData("http://imgur.com/folder/file.txt?foo=bar", "foo=bar")]
            [InlineData("http://imgur.com/folder?foo=bar", "foo=bar")]
            public void Should_parse_query(string url, string query)
            {
                var result = _parser.Parse(url);

                Assert.Equal(query, result.Query);
            }

            [Theory]
            [InlineData("http://imgur.com/#anc", "anc")]
            [InlineData("http://imgur.com#anc", "anc")]
            [InlineData("http://imgur.com/file.txt#anc", "anc")]
            [InlineData("http://imgur.com/folder#anc", "anc")]
            [InlineData("http://imgur.com/folder/#anc", "anc")]
            [InlineData("http://imgur.com/folder/file.txt#anc", "anc")]
            public void Should_parse_anchor(string url, string anchor)
            {
                var result = _parser.Parse(url);

                Assert.Equal(anchor, result.Anchor);
            }

            [Theory]
            [InlineData("<http://www.imgur.com/>", "www.imgur.com")]
            [InlineData("<http://www.imgur.com|www.imgur.com>", "www.imgur.com")]
            public void Should_handle_slack_style_urls(string input,string expectedDomain)
            {
                var result = _parser.Parse(input);

                Assert.Equal(expectedDomain, result.Domain);
            }

            [Theory]
            [InlineData("http://www.youtube.com/watch?v=-wtIMTCHWuI", UrlTargetType.Video)]
            [InlineData("http://www.youtube.com/v/-wtIMTCHWuI?version=3&autohide=1", UrlTargetType.Video)]
            [InlineData("http://www.youtube.com/attribution_link?a=JdfC0C9V6ZI&u=%2Fwatch%3Fv%3DEhxJLojIE_o%26feature%3Dshare", UrlTargetType.Video)]
            [InlineData("https://www.youtube.com/embed/M7lc1UVf-VE", UrlTargetType.Video)]
            [InlineData("http://youtu.be/-wtIMTCHWuI", UrlTargetType.Video)]
            [InlineData("http://www.d.com/file.gifv", UrlTargetType.Video)]
            [InlineData("https://vimeo.com/230282582", UrlTargetType.Video)]
            [InlineData("https://www.facebook.com/3dlabprintface/videos/810239659156379/", UrlTargetType.Video)]

            [InlineData("http://www.d.com/file.jpg", UrlTargetType.Image)]
            [InlineData("http://www.d.com/file.jpeg", UrlTargetType.Image)]
            [InlineData("http://www.d.com/file.gif", UrlTargetType.Image)]
            [InlineData("http://www.d.com/file.png", UrlTargetType.Image)]
            [InlineData("http://www.d.com/file.tiff", UrlTargetType.Image)]
            [InlineData("http://www.d.com/file.bmp", UrlTargetType.Image)]
            [InlineData("http://www.d.com/file.pcx", UrlTargetType.Image)]

            [InlineData("http://www.d.com/file.pdf", UrlTargetType.Document)]
            [InlineData("http://www.d.com/file.doc", UrlTargetType.Document)]
            [InlineData("http://www.d.com/file.docx", UrlTargetType.Document)]
            [InlineData("http://www.d.com/file.xls", UrlTargetType.Document)]
            [InlineData("http://www.d.com/file.xlsx", UrlTargetType.Document)]

            [InlineData("https://open.spotify.com/track/4MFKi6VzidmH1V6TX86ihS", UrlTargetType.Music)]
            [InlineData("https://open.spotify.com/user/spotify/playlist/37i9dQZF1DX4sWSpwq3LiO", UrlTargetType.Music)]

            [InlineData("http://www.nu", UrlTargetType.Other)]
            [InlineData("http://www.imgur.com", UrlTargetType.Other)]
            [InlineData("http://www.youtube.com", UrlTargetType.Other)]
            [InlineData("http://www.youtu.be", UrlTargetType.Other)]
            public void Should_parse_target_type(string url, UrlTargetType type)
            {
                var result = _parser.Parse($"<{url}>");

                Assert.Equal(type, result.TargetType);
            }
        }
        
    }
}
