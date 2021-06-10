using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace mv_generator
{
    public static class Class1
    {
        public static bool StartsWithUpper(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            char ch = str[0];
            return char.IsUpper(ch);
        }
        /// <summary>
        /// Genetares music video from 1 photo and 9 tracks
        /// </summary>
        /// <param name="photo"></param>
        /// <param name="musicList"></param>
        /// <returns>mp4 file</returns>
        public static string GenerateMV(this string photo, List<string> musicList, string outputWav, bool trim)
        {
            double dur = 60 / musicList.Count;
            double oneTrackDuration = Math.Floor(dur);
            List<string> trimmedPaths = new List<string>();
            //Trim audio
            foreach (var audio in musicList)
            {
                //using (Mp3FileReader reader = new Mp3FileReader(audio))
                //{
                if (trim)
                {
                    Mp3FileReader reader = new Mp3FileReader(audio);
                    TimeSpan duration = reader.TotalTime;
                    var oneThirdDuration = duration / 3;
                    //Trim 6 seconds if 9 files (60 sec video max)
                    int lastChar = audio.IndexOf(".mp3");
                    string pathWOExt = audio.Remove(lastChar, 4);
                    string trimmedFile = pathWOExt + "_trimmed.mp3";
                    TimeSpan end = oneThirdDuration + TimeSpan.FromSeconds(oneTrackDuration);

                    TrimMp3(audio, trimmedFile, oneThirdDuration, end);
                    trimmedPaths.Add(trimmedFile);
                }
                else
                    trimmedPaths.Add(audio);
                //}
            }


            // Foreach trim audio create one big audio
            Combine(trimmedPaths, outputWav);

 
            string outVideoName = StartFFMPEG(photo, outputWav, false);

            foreach (string file in trimmedPaths)
            {
                FileInfo file2 = new FileInfo(file);
                while (IsFileLocked(file2))
                    Thread.Sleep(1000);
                file2.Delete();

                //File.Delete(file);
            }
            return outVideoName;
        }

        static string StartFFMPEG(string image, string audio, bool isDebian)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg\\ffmpeg.exe");
            //if (isDebian)
            //    startInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg-linux64\\ffmpeg");
            string outMP4 = "out.mp4";
            startInfo.Arguments = "-y -loop 1 -i " + image + " -i " + audio + " -c:v libx264 -tune stillimage -c:a aac -b:a 192k -pix_fmt yuv420p -shortest " + outMP4;
            startInfo.RedirectStandardOutput = true;
            //startInfo.RedirectStandardError = true;

            Console.WriteLine(string.Format(
                "Executing \"{0}\" with arguments \"{1}\".\r\n",
                startInfo.FileName,
                startInfo.Arguments));

            try
            {
                using Process process = Process.Start(startInfo);
                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    Console.WriteLine(line);
                }

                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //Console.ReadKey();
            return outMP4;
        }
        static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
        public static void Combine(List<string> inputFiles, string wavName)
        {
            //TODO Release inputFiles

            List<AudioFileReader> audioFileReaders = new List<AudioFileReader>();
            foreach (string file in inputFiles)
            {
                //MemoryStream mp3file = new MemoryStream(file);
                var f = new AudioFileReader(file);

                audioFileReaders.Add(f);
            }
            var playlist = new ConcatenatingSampleProvider(audioFileReaders);
            //using(var writer = new WaveFileWriter())

            WaveFileWriter.CreateWaveFile16(wavName, playlist);
            //ConvertWAV2MP3(wavName, "", "playlist.mp3");

            //File.Delete(wavName);
        }

        public static void ConvertWAV2MP3(string wavFile, string outputFolder, string mp3name)
        {
            var mp3FilePath = Path.Combine(outputFolder, mp3name);
            using var reader = new WaveFileReader(wavFile);
            try
            {
                MediaFoundationEncoder.EncodeToMp3(reader, mp3FilePath);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void TrimMp3(string inputPath, string outputPath, TimeSpan? begin, TimeSpan? end)
        {
            if (begin.HasValue && end.HasValue && begin > end)
                throw new ArgumentOutOfRangeException("end", "end should be greater than begin");

            using var reader = new Mp3FileReader(inputPath);
            using var writer = File.Create(outputPath);
            Mp3Frame frame;
            while ((frame = reader.ReadNextFrame()) != null)
                if (reader.CurrentTime >= begin || !begin.HasValue)
                {
                    if (reader.CurrentTime <= end || !end.HasValue)
                        writer.Write(frame.RawData, 0, frame.RawData.Length);
                    else break;
                }
            writer.Dispose();
            reader.Dispose();
            writer.Close();
            reader.Close();
        }

    }
}
