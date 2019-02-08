using System;
using System.Threading;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.MessageHandler;
using pappab0t.Models;
using pappab0t.Modules.Highscore;
using pappab0t.Modules.Inventory;
using pappab0t.Responders;
using pappab0t.SlackApis;
using StructureMap;

namespace pappab0t
{
    public static class ObjectFactory
    {
        private static readonly Lazy<Container> ContainerBuilder =
            new Lazy<Container>(DefaultContainer, LazyThreadSafetyMode.ExecutionAndPublication);

        public static IContainer Container => ContainerBuilder.Value;

        private static Container DefaultContainer()
        {
            return new Container(x =>
            {
                x.For<IInventoryManager>().Use<InventoryManager>();
                x.For<ICommandDataParser>().Use<CommandDataParser>();
                x.For<IPhrasebook>().Use<Phrasebook>();
                x.For<IUrlParser>().Use<UrlParser>();
                x.For<IHighScoreManager>().Use<HighScoreManager>();

                x.For<ISlackDMApi>().Use<SlackDMApi>();
                x.For<Random>().Use(() => new Random());

                x.Scan(y =>
                {
                    y.AddAllTypesOf<IResponder>();
                    y.AddAllTypesOf<IMessageHandler>();
                    y.AddAllTypesOf<IEventHandler>();
                    y.AddAllTypesOf<Command>();
                    y.AddAllTypesOf<ScheduledTask>();
                    y.AssemblyContainingType(typeof(IExposedCapability));
                });
            });
        }
    }
}