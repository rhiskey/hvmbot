﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace hvmbot.TelegramPart
{
    class TelegramMusic
    {
        private static string tg_acces_token = Configuration.TGAuth.GetTGBotToken(); //BotFather

        private static ChatId tg_chat_id = Configuration.TGAuth.GetTGChatID();         ///https://habr.com/ru/post/306222/

        private static TelegramBotClient Bot;
        //private static TelegramBotClient botClient = new TelegramBotClient(tg_acces_token, new HttpToSocks5Proxy(Configuration.Proxy.Host, Configuration.Proxy.Port));
        private static TelegramBotClient botClient = new TelegramBotClient(tg_acces_token);
        private static int waitUntillUpload = 30000; //milliseconds

        //Каждый раз при вызове класса, происходит авторизация бота
        public TelegramMusic()
        {

        }
        public static TelegramBotClient Auth()
        {
            ///TorBrowser
            //botClient = new TelegramBotClient(tg_acces_token, new HttpToSocks5Proxy(Configuration.Proxy.Host, Configuration.Proxy.Port));

            var botClient = new TelegramBotClient(tg_acces_token);
            Bot = botClient;
            var me = botClient.GetMeAsync().Result;

            Console.WriteLine("Telegram Auth!");
            return botClient;
        }

        //}
        public static async System.Threading.Tasks.Task SendPhotoAsync(string photoLink, string photoCaption)
        {
            botClient = Auth();
            Message message = await botClient.SendPhotoAsync(chatId: tg_chat_id, photo: photoLink, caption: photoCaption, parseMode: ParseMode.Html);
        }
        public static async System.Threading.Tasks.Task SendPhotoFromFileAsync(string photoFilePath, string photoCaption)
        {
            botClient = Auth();
            Message msg = null;
            using (FileStream fstream = System.IO.File.OpenRead(photoFilePath))
            {
                InputOnlineFile iof = new InputOnlineFile(fstream);
                iof.FileName = photoFilePath;
                msg = await botClient.SendPhotoAsync(tg_chat_id, iof, photoCaption);
                //msg.

            }
            do
            {
                Thread.Sleep(100);
            } while (msg == null);
            //if (msg!=null)
            System.IO.File.Delete(photoFilePath);
        }

        public static async System.Threading.Tasks.Task SendAudioAsync(string mp3Link)
        {
            botClient = Auth();
            //Не может открыть адрес вк, т.к. привязан к IP - поэтому нужно загрузить- затем отправить

            Message message = await botClient.SendAudioAsync(tg_chat_id, mp3Link); //or e.Message.Chat
        }

        public static async System.Threading.Tasks.Task SendAudioFromFileAsync(string localFilePath, string message, string author, string title, Uri thumbnailUrl = null /*, InputMedia thumbinal*/)
        {
            botClient = Auth();
            Message msg = null;
            //Добавить в replyMarkup клавиатуру зелен галочка emoji и крестик (либо лайк и дизлайк)
            //byte lovely = "\xF0\x9F\x98\x8D";
            //var inlineKeyboard = new InlineKeyboardMarkup(new[]
            //{
            //        // first row
            //        new []
            //        {
            //            InlineKeyboardButton.WithCallbackData("+ Кайф", "likePressed"),
            //            InlineKeyboardButton.WithCallbackData("- Шлак", "dislikePressed"),
            //        }
            //        //// second row
            //        //new []
            //        //{
            //        //    InlineKeyboardButton.WithCallbackData("2.1", "21"),
            //        //    InlineKeyboardButton.WithCallbackData("2.2", "22"),
            //        //}
            //});
            //Доделать отображение на кнопках счетчика нажатий 

            //string thumbUrl = thumbnailUrl.AbsoluteUri.ToString(), thumbName = "thumb.jpg" , thumbFilePath = Path.GetFileName(thumbName) ;

            //Не может открыть адрес вк, т.к. привязан к IP - поэтому нужно загрузить- затем отправить
            try
            {
                using (FileStream fstream = System.IO.File.OpenRead(localFilePath))
                {
                    InputOnlineFile iof = new InputOnlineFile(fstream);


                    #region UploadAudio_with_thumbnail
                    //await Bot.SendChatActionAsync(tg_chat_id, ChatAction.UploadPhoto); //Typing Status Not Needed

                    // Отправлять в качестве превью уже загруженную картинку поста (ВЫШЕ)
                    //InputMedia thumb = new InputMedia(thumbUrl); 

#if WANT_TO_DONWLOAD_THUMB
                    //Download Thumb
                    try
                    {

                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFile(thumbUrl, thumbFilePath);
                            double two_hundred_kilobytes = 200000;
                            long size = new System.IO.FileInfo(thumbName).Length;//in bytes 
                            if (size < two_hundred_kilobytes)
                            {
                                using var thumbFileStream = new FileStream(thumbFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                                var newThumbFileName = thumbFilePath.Split(Path.DirectorySeparatorChar).Last();
                                thumb = new InputMedia(thumbFileStream, newThumbFileName);

                            }
                            else thumb = null;

                        }
                    }
                    catch (Exception ex) { Logging.ErrorLogging(ex); Logging.ReadError(); thumb = new InputMedia(thumbUrl); }
#endif
                    /*Thumbnail of the file sent; can be ignored if thumbnail generation for the file is supported server-side.
                     * The thumbnail should be in JPEG format and less than 200 kB in size. 
                     * A thumbnail's width and height should not exceed 320. Ignored if the file is not uploaded using multipart/form-data.
                     * Thumbnails can't be reused and can be only uploaded as a new file, so you can pass “attach://<file_attach_name>” if the
                     * thumbnail was uploaded using multipart/form-data under <file_attach_name>.
                     * 
                     */
                    #endregion

                    iof.FileName = localFilePath;
                    if (message != null)
                    {

#if THUMB_PROPERLY_STREAMED_OR_LOADED
                    CheckThumb:
                        if (thumb != null) //GOVNO
                        {
                            try
                            {
                                msg = await botClient.SendAudioAsync(tg_chat_id, iof, message, ParseMode.Default, duration: 0, performer: author, title, false, 0, null, default(CancellationToken), thumb); //Передавать JPEG <200 кб превью (фотку поста)
                            }
                            catch (Exception ex) { Logging.ErrorLogging(ex); Logging.ReadError(); thumb = null; goto CheckThumb; }
                        }
                        else
                        {
#endif
                        msg = await botClient.SendAudioAsync(tg_chat_id, iof, message, ParseMode.Default, null, duration: 0, performer: author, title /*, false, 0, inlineKeyboard*/ /*,false, 0, null, new CancellationTokenSource(waitUntillUpload).Token*/); //Передавать JPEG <200 кб превью (фотку поста)
                                                                                                                                                                                                                                                          //                      }
                    }
                    else msg = await botClient.SendAudioAsync(tg_chat_id, iof);
                }
                do
                {
                    Thread.Sleep(100);
                } while (msg.MessageId == 0);

                System.IO.File.Delete(localFilePath);

                //                System.IO.File.Delete(thumbFilePath);

            }
            catch (Exception ioe) { Logging.ErrorLogging(ioe); Logging.ReadError(); /*Request timed out*/ }
        }
        public static async System.Threading.Tasks.Task SendAudioFromFileAsync(string localFilePath)
        {
            botClient = Auth();
            Message msg = null;
            //Не может открыть адрес вк, т.к. привязан к IP - поэтому нужно загрузить- затем отправить
            using (FileStream fstream = System.IO.File.OpenRead(localFilePath))
            {
                InputOnlineFile iof = new InputOnlineFile(fstream);
                iof.FileName = "music.mp3";
                msg = await botClient.SendAudioAsync(tg_chat_id, iof);
            }
            do
            {
                Thread.Sleep(100);
            } while (msg == null);
            System.IO.File.Delete(localFilePath);
        }
        public static async Task SendPoll(string question, IEnumerable<string> pollAnswers)
        {

            //poll
            botClient = Auth();
            await botClient.SendPollAsync(tg_chat_id, question, pollAnswers);
        }

        private static async void ProcessInlineQuiery()
        {
            botClient.OnCallbackQuery += async (object sc, Telegram.Bot.Args.CallbackQueryEventArgs ev) =>
            {
                var message = ev.CallbackQuery.Message;
                if (ev.CallbackQuery.Data == "likePressed")
                {
                    // Отобразить на кнопке число нажатий
                    //likeCounter++;
                    // сюда то что тебе нужно сделать при нажатии на первую кнопку 
                }
                else
                if (ev.CallbackQuery.Data == "dislikePressed")
                {
                    //dislikeCouner++;
                    // сюда то что нужно сделать при нажатии на вторую кнопку
                }
            };
        }

    }


}
