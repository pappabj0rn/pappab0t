using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pappab0t.Modules.Inventory.Items.Modifiers;
using pappab0t.Modules.Inventory.Items.Tokens;
using pappab0t.Tests.Responders;
using Xunit;

namespace pappab0t.Tests.Modules.Inventory.Items
{
    public abstract class ItemStorabilityTests : ResponderTestsContext
    {
        protected ItemStorabilityTests()
        {
            ConfigureRavenDB();
        }

        public class HandlerLogModifier : ItemStorabilityTests
        {
            [Fact]
            public void Should_be_storable_and_retreivable()
            {
                var item = new Note();
                var log = new HandlerLog();
                log.Add("testUserId");
                item.Modifiers.Add(log);

                using (var session = Store.OpenSession())
                {
                    session.Store(item);
                    session.SaveChanges();
                }

                using (var session = Store.OpenSession())
                {
                    var notes = session
                        .Query<Note>()
                        .Customize(x => x.WaitForNonStaleResults())
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        .ToList();

                    Assert.NotEmpty(notes);
                }
            }
        }

        public class ExpiresModifier : ItemStorabilityTests
        {
            [Fact]
            public void Should_be_storable_and_retreivable()
            {
                var item = new Note();
                var exp = new Expires{DateTime = new DateTime(2019,2,8,1,52,17)};;
                item.Modifiers.Add(exp);

                using (var session = Store.OpenSession())
                {
                    session.Store(item);
                    session.SaveChanges();
                }

                using (var session = Store.OpenSession())
                {

                    var notes = session
                        .Query<Note>()
                        .Customize(x => x.WaitForNonStaleResults())
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        .ToList();

                    Assert.NotEmpty(notes);
                    Assert.Equal(
                        exp.DateTime,
                        notes.First().Modifiers.OfType<Expires>().First().DateTime);
                }
            }
        }
    }
}
