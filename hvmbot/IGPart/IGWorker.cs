using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hvmbot.IGPart
{
    class IGWorker
    {
        private static IInstaApi InstaApi;
        public static async Task<bool> UploadVideoToIGAsync(string videoFile, string prevImgFile, string caption)
        {
            try
            {
                // create user session data and provide login details
                var userSession = new UserSessionData
                {
                    UserName = Program.IG_USERNAME,
                    Password = Program.IG_PASS
                };
                // if you want to set custom device (user-agent) please check this:
                // https://github.com/ramtinak/InstagramApiSharp/wiki/Set-custom-device(user-agent)

                var delay = RequestDelay.FromSeconds(2, 2);
                // create new InstaApi instance using Builder
                InstaApi = InstaApiBuilder.CreateBuilder()
                    .SetUser(userSession)
                    .UseLogger(new DebugLogger(LogLevel.All)) // use logger for requests and debug messages
                    .SetRequestDelay(delay)
                    .Build();
                // create account
                // to create new account please check this:
                // https://github.com/ramtinak/InstagramApiSharp/wiki/Create-new-account
                const string stateFile = "state.bin";
                try
                {
                    if (File.Exists(stateFile))
                    {
                        Console.WriteLine("Loading state from file");
                        using (var fs = File.OpenRead(stateFile))
                        {
                            InstaApi.LoadStateDataFromStream(fs);
                            // in .net core or uwp apps don't use LoadStateDataFromStream
                            // use this one:
                            // _instaApi.LoadStateDataFromString(new StreamReader(fs).ReadToEnd());
                            // you should pass json string as parameter to this function.
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }


                if (!InstaApi.IsUserAuthenticated)
                {
                    // login
                    Console.WriteLine($"Logging in as {userSession.UserName}");
                    delay.Disable();
                    var logInResult = await InstaApi.LoginAsync();
                    delay.Enable();
                    if (!logInResult.Succeeded)
                    {
                        Console.WriteLine($"Unable to login: {logInResult.Info.Message}");
                        return false;
                    }
                }
                var state = InstaApi.GetStateDataAsStream();
                // in .net core or uwp apps don't use GetStateDataAsStream.
                // use this one:
                // var state = _instaApi.GetStateDataAsString();
                // this returns you session as json string.
                using (var fileStream = File.Create(stateFile))
                {
                    state.Seek(0, SeekOrigin.Begin);
                    state.CopyTo(fileStream);
                }

                var up = new IGPart.UploadVideo(InstaApi, videoFile, prevImgFile, caption);
                await IGPart.UploadVideo.DoShow();
                Thread.Sleep(8000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                // perform that if user needs to logged out
                // var logoutResult = Task.Run(() => _instaApi.LogoutAsync()).GetAwaiter().GetResult();
                // if (logoutResult.Succeeded) Console.WriteLine("Logout succeed");
            }
            return false;
        }
    }
}
