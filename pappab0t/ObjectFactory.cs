using System;
using System.Threading;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.MessageHandler;
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
                x.Scan(y =>
                {
                    y.AddAllTypesOf<IResponder>();
                    y.AddAllTypesOf<IMessageHandler>();
                    y.AssemblyContainingType(typeof(IExposedCapability));
                });
            });
        }
    }
}