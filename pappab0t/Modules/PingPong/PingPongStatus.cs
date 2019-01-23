using System;

namespace pappab0t.Modules.PingPong
{
    public class PingPongStatus
    {
        public static DateTime PingSent { get; set; }
        public static bool WaitingForPong { get; set; }
    }
}