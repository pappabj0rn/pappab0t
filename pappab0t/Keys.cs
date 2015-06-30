namespace pappab0t
{
    public static class Keys
    {
        public static class AppSettings
        {
            public const string SlackKey = "slackKey";
            public const string RavenUrl = "ravenUrl";
            public const string RavenDbName = "ravenDbName";
            public const string BotAliases = "botAliases";
        }

        public static class RavenDB
        {
            public static class Metadata
            {
                public const string Created = "Created";
                public const string TimeStamp = "TimeStamp";
            }
        }

        public static class Slack
        {
            public static class MessageJson
            {
                public const string Username = "username";
                public const string TimeStamp = "ts";
                public const string Type = "type";
                public const string Text = "text";
                public const string User = "user";
                public const string Channel = "channel";
            }

            public static class UserListJson
            {
                public const string Members = "members";
            }
        }
    }
}
