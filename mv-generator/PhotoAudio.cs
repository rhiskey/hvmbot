using System;
using System.Collections.Generic;
using System.Text;

namespace mv_generator
{
    public class PhotoAudio
    {
        string photoPath { get; set; }
        List<string> audioPaths { get; set; }

        public PhotoAudio(string photo, List<string> audio)
        {
            photoPath = photo;
            audioPaths = audio;
        }

        public string GetPhoto()
        {
            return photoPath;
        }
        public List<string> GetAudioList()
        {
            return audioPaths;
        }
    }
}
