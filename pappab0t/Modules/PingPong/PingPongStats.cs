using System;

namespace pappab0t.Modules.PingPong
{
    public class PingPongStats
    {
        public static DateTime LastFailedPing { get; set; }
        public static DateTime LastPingTime { get; set; }
        public static DateTime LastPongTime { get; set; }
        public static string LastResponse { get; set; }
        public static int PingsSent { get; set; }
    }
}