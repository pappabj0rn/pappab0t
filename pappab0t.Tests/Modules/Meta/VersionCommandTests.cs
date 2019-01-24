using System;
using pappab0t.Modules.Meta;
using pappab0t.Modules.PingPong;
using Xunit;

namespace pappab0t.Tests.Modules.PingPong
{
    public abstract class VersionCommandTests
    {
        private readonly VersionCommand _cmd;

        protected VersionCommandTests()
        {
            _cmd = new VersionCommand();
        }

        public class GetResponse : VersionCommandTests
        {
            [Fact]
            public void Should_return_pappab0t_assembly_version_info()
            {
                var version = _cmd.GetType().Assembly.GetName().Version;

                var buildDate = new DateTime(2000, 1, 1)
                    .AddDays(version.Build)
                    .AddSeconds(version.Revision * 2);
                
                var response = _cmd.GetResponse();

                Assert.Equal($"pappab0t {version} ({buildDate:G})", response.Text);
            }
        }

        public class RespondsTo : VersionCommandTests
        {
            [Theory]
            [InlineData("version")]
            public void Should_respond_to_listad_commans(string cmd)
            {
                var respondTo = _cmd.RespondsTo(cmd);

                Assert.True(respondTo);
            }
        }
    }
}
