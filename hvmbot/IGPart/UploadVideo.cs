using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace hvmbot.IGPart
{
    class UploadVideo
    {
        private static IInstaApi InstaApi;
        private static string videoPath, thumbPath, caption;


        public UploadVideo(IInstaApi instaApi, string video, string prev, string text)
        {
            InstaApi = instaApi;
            videoPath = video;
            thumbPath = prev;
            caption = text;
        }
        public static async Task DoShow()
        {
            var video = new InstaVideoUpload
            {
                // leave zero, if you don't know how height and width is it.
                Video = new InstaVideo(videoPath, 0, 0),
                VideoThumbnail = new InstaImage(thumbPath, 0, 0)
            };
            //// Add user tag (tag people)
            //video.UserTags.Add(new InstaUserTagVideoUpload
            //{
            //    Username = "rmt4006"
            //});
            var result = await InstaApi.MediaProcessor.UploadVideoAsync(video, caption);
            Console.WriteLine(result.Succeeded
                ? $"Media created: {result.Value.Pk}, {result.Value.Caption}"
                : $"Unable to upload video: {result.Info.Message}");
        }
    }
}
