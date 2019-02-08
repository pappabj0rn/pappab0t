using System;
using System.Linq;
using pappab0t.Abstractions;
using pappab0t.Modules.Inventory;
using pappab0t.Modules.Inventory.Items.Modifiers;

namespace pappab0t.Modules.BombGame
{
    public class TimedBombEventHandler : IEventHandler, IDisposable
    {
        private bool _initialized;
        public void Initialize()
        {
            if(_initialized)
                return;
            
            MessageBus<ItemMovingMessage>.Instance.MessageRecieved += Instance_MessageRecieved;
            _initialized = true;
        }

        private void Instance_MessageRecieved(object sender, MessageBusEventArgs<ItemMovingMessage> e)
        {
            var log = e.Message.Item.Modifiers.OfType<IHandlerLog>().FirstOrDefault();

            log?.Add(e.Message.TargetInventory.UserId);
        }

        public void Dispose()
        {
            MessageBus<ItemMovingMessage>.Instance.MessageRecieved -= Instance_MessageRecieved;
        }
    }
}