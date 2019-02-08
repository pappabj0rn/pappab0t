using System;
using System.Collections;
using System.Collections.Generic;

namespace pappab0t.Modules.Inventory.Items.Modifiers
{
    public class HandlerLog : Modifier, IHandlerLog
    {
        public List<HandlerLogEntry> LogEntries { get; } = new List<HandlerLogEntry>();

        public void Add(string userId)
        {
            LogEntries.Add(new HandlerLogEntry
            {
                UserId = userId,
                LogDateTime = SystemTime.Now()
            });
        }
    }

    public class HandlerLogEntry
    {
        public string UserId { get; set; }
        public DateTime LogDateTime { get; set; }
    }

    public interface IHandlerLog
    {
        void Add(string userId);
        List<HandlerLogEntry> LogEntries { get; }
    }
}