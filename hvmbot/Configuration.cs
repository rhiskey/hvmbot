using Telegram.Bot.Types;

namespace hvmbot
{

    public static class Configuration
    {
        public static string rollbarToken { get; set; }
        public static class TGAuth
        {
            private static string AccessToken;
            private static ChatId tg_chat_id;
            private static string BotToken;

            public static string GetTGAccesToken() { return AccessToken; }
            public static ChatId GetTGChatID() { return tg_chat_id; }
            public static string GetTGBotToken() { return BotToken; }

            public static void SetTGAccessToken(string token) { AccessToken = token; }
            public static void SetTGChatID(ChatId id) { tg_chat_id = id; }
            public static void SetTGBotToken(string token) { BotToken = token; }
        }
        //#if USE_PROXY

        public static class Proxy
        {
            private readonly static string Host = "127.0.0.1";
            public readonly static int Port = 9050;

            public static string GetHost() { return Host; }
            public static int GetPort() { return Port; }
        }

        //#endif
        public static class VKAuth
        {
            private static string vkLogin;
            private static string vkPassword;
            private static ulong kateMobileAppID;
            //Получается https://oauth.vk.com/authorize?client_id=7361627&scope=notify,wall,groups,messages,notifications,offline&redirect_uri=http://api.vk.com/blank.html&display=page&response_type=token                                                    
            //NOT NEEDED
            private static string vkapiToken;
            private static ulong groupID;
            private static string kateMobileToken;

            public static string GetVKLogin() { return vkLogin; }
            public static string GetVKPassword() { return vkPassword; }
            public static ulong GetVKKateMobileAppID() { return kateMobileAppID; }
            public static string GetVKKateMobileToken() { return kateMobileToken; }
            public static string GetVKApiToken() { return vkapiToken; }
            public static ulong GetVKGroupID() { return groupID; }


            public static void SetVKLogin(string login) { vkLogin = login; }
            public static void SetVKPassword(string password) { vkPassword = password; }
            public static void SetVKKateMobileAppID(ulong id) { kateMobileAppID = id; }
            public static void SetVKApiToken(string token) { vkapiToken = token; }
            public static void SetVKKateMobileToken(string token) { kateMobileToken = token; }
            public static void SetVKGroupID(ulong id) { groupID = id; }


        }

        public static class DBAuth
        {
            private static string host;
            private static int port;
            private static string database;
            private static string username;
            private static string password;

            public static string GetHost() { return host; }
            public static int GetPort() { return port; }
            public static string GetDatabaseName() { return database; }
            public static string GetUsername() { return username; }
            public static string GetPassword() { return password; }

            public static void SetHost(string Host) { host = Host; }
            public static void SetPort(int Port) { port = Port; }
            public static void SetDatabaseName(string DBName) { database = DBName; }
            public static void SetUsername(string Login) { username = Login; }
            public static void SetPassword(string Password) { password = Password; }
        }

    }
}
