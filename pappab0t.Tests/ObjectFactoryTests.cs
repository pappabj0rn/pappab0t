using System;
using MargieBot;
using Moq;
using pappab0t.Abstractions;
using pappab0t.MessageHandler;
using pappab0t.Models;
using pappab0t.Modules.Highscore;
using pappab0t.Modules.Inventory;
using pappab0t.Responders;
using pappab0t.SlackApis;
using pappab0t.Tests.Responders;
using Raven.Client;
using Xunit;

namespace pappab0t.Tests
{
    public class ObjectFactoryTests : ResponderTestsContext
    {
        public ObjectFactoryTests()
        {
            ConfigureRavenDB();

            ObjectFactory.Container.Configure(
                x =>
                {
                    x.For<IDocumentStore>()
                        .Use(Store);
                    
                    x.For<IBot>().Use(() => new BotWrapper(null));
                });
        }

        [Fact]
        public void Should_have_configuration_for_IPhrasebook()
        {
            var instance = ObjectFactory.Container.GetInstance<IPhrasebook>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void Should_have_configuration_for_IHighscoreManager()
        {
            var instance = ObjectFactory.Container.GetInstance<IHighScoreManager>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void Should_have_configuration_for_IInventoryManager()
        {
            var instance = ObjectFactory.Container.GetInstance<IInventoryManager>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void Should_have_configuration_for_ICommandDataParser()
        {
            var instance = ObjectFactory.Container.GetInstance<ICommandDataParser>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void Should_have_configuration_for_IUrlParser()
        {
            var instance = ObjectFactory.Container.GetInstance<IUrlParser>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void Should_have_configuration_for_ISlackDMApi()
        {
            var instance = ObjectFactory.Container.GetInstance<ISlackDMApi>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void Should_have_configuration_for_Random()
        {
            var instance = ObjectFactory.Container.GetInstance<Random>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void Scan_for_IEventHandler()
        {
            var instances = ObjectFactory.Container.GetAllInstances<IEventHandler>();

            Assert.NotEmpty(instances);
        }

        [Fact]
        public void Scan_for_IResponder()
        {
            var instances = ObjectFactory.Container.GetAllInstances<IResponder>();

            Assert.NotEmpty(instances);
        }

        [Fact]
        public void Scan_for_IMessageHandler()
        {
            var instances = ObjectFactory.Container.GetAllInstances<IMessageHandler>();

            Assert.NotEmpty(instances);
        }

        [Fact]
        public void Scan_for_Command()
        {
            var instances = ObjectFactory.Container.GetAllInstances<Command>();

            Assert.NotEmpty(instances);
        }

        [Fact]
        public void Scan_for_ScheduledTask()
        {
            var instances = ObjectFactory.Container.GetAllInstances<ScheduledTask>();

            Assert.NotEmpty(instances);
        }
    }
}
