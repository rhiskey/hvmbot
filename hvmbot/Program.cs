using hvmbot.TelegramPart;
using hvmbot.VKPart;
using System;
using System.Threading;

namespace hvmbot
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load Config
            DotNetEnv.Env.Load();

            var TG_ACCESS_TOKEN = System.Environment.GetEnvironmentVariable("TG_ACCESS_TOKEN");
            var TG_CHAT_ID = System.Environment.GetEnvironmentVariable("TG_CHAT_ID");
            var TG_BOT_TOKEN = System.Environment.GetEnvironmentVariable("TG_BOT_TOKEN");
            var VK_API_TOKEN = System.Environment.GetEnvironmentVariable("VK_API_TOKEN");
            ulong VK_GROUP_ID = (ulong)Convert.ToInt64(System.Environment.GetEnvironmentVariable("VK_GROUP_ID"));
            var VK_LOGIN = System.Environment.GetEnvironmentVariable("VK_LOGIN");
            var VK_PASS = System.Environment.GetEnvironmentVariable("VK_PASS");
            ulong VK_KATE_MOBILE_APP_ID = (ulong)Convert.ToInt64(DotNetEnv.Env.GetInt("VK_KATE_MOBILE_APP_ID"));
            //var KATE_MOBILE_TOKEN = System.Environment.GetEnvironmentVariable("VK_KATE_MOBILE_TOKEN");

            string rollbarToken = System.Environment.GetEnvironmentVariable("ROLLBAR_TOKEN");


            Configuration.VKAuth.SetVKApiToken(VK_API_TOKEN); //Обновления группы 
            Configuration.VKAuth.SetVKGroupID(VK_GROUP_ID);
            Configuration.VKAuth.SetVKLogin(VK_LOGIN);
            Configuration.VKAuth.SetVKPassword(VK_PASS);
            Configuration.VKAuth.SetVKKateMobileAppID(VK_KATE_MOBILE_APP_ID); //Скачивание музыки и фото
            //Configuration.VKAuth.SetVKKateMobileToken(KATE_MOBILE_TOKEN);
            Configuration.TGAuth.SetTGAccessToken(TG_ACCESS_TOKEN);
            Configuration.TGAuth.SetTGChatID(TG_CHAT_ID);
            Configuration.TGAuth.SetTGBotToken(TG_BOT_TOKEN);
            
            Configuration.rollbarToken = rollbarToken;


            //---------------START-------------------
            Thread vkLPL = new Thread(LongPollHandler.LongPollListener);
            vkLPL.Start();
            Console.WriteLine("Bot is running...");
            //Thread runTG = new Thread(RunTg);
            //runTG.Start();
            //---------------START-------------------

            ////Docker can't Poll WHY?
            //Thread echoBot = new Thread(RunEchoBot);
            //echoBot.Start();
        }

        static async void RunEchoBot()
        {
            await EchoBot.Main();
        }
        static async void RunTg()
        {
            //await TelegramMusic.SendTextAsync("TestText");
            await TelegramMusic.SendAudioFromFileAsync("music.mp3");
            //await TelegramMusic.SendPhotoAsync( "https://.jpg", "");
            //await TelegramMusic.SendAudioAsync("https://github.com/TelegramBots/book/raw/master/src/docs/audio-guitar.mp3");
        }

    }
}
