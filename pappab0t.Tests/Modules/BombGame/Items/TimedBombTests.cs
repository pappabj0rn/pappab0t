using pappab0t.Modules.BombGame.Items;
using Xunit;

namespace pappab0t.Tests.Modules.BombGame.Items
{
    public class TimedBombTests
    {
        [Fact]
        public void TimedBomb_has_a_friendly_name()
        {
            Assert.Equal("Tidsinställd bomb", new TimedBomb().GetFriendlyTypeName());
        }

        [Fact]
        public void GetDescription_returns_string()
        {
            Assert.Equal("En tidsinställd bomb.", new TimedBomb().GetDescription());
        }
    }
}