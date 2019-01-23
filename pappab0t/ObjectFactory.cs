using System;
using System.Threading;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.MessageHandler;
using pappab0t.Models;
using pappab0t.Modules.Inventory;
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

                x.For<ISlackDMApi>().Use<SlackDMApi>();

                x.Scan(y =>
                {
                    y.AddAllTypesOf<IResponder>();
                    y.AddAllTypesOf<IMessageHandler>();
                    y.AddAllTypesOf<ScheduledTask>();
                    y.AddAllTypesOf<IUrlParser>();
                    y.AddAllTypesOf<IPhrasebook>();
                    y.AssemblyContainingType(typeof(IExposedCapability));
                });
            });
        }
    }
}