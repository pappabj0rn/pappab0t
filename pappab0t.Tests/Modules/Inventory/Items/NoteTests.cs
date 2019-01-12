using pappab0t.Modules.Inventory.Items.Tokens;
using Xunit;

namespace pappab0t.Tests.Modules.Inventory.Items
{
    public abstract class NoteTests
    {
        public class DescribableTests : NoteTests
        {
            private readonly Note _note;

            public DescribableTests()
            {
                _note = new Note
                {
                    Name = "Pipe",
                    Text = "Ceci n'est pas une pipe"
                };
            }

            [Fact]
            public void Description_should_return_text_of_note()
            {
                Assert.Equal(_note.Text, _note.GetDescription());
            }

            [Fact]
            public void GetFriendlyTypeName_should_return_swedish_name()
            {
                Assert.Equal("Lapp", _note.GetFriendlyTypeName());
            }
        }
    }
}
