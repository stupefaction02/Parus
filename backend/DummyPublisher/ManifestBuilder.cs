using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Common.Publisher
{
    internal class ManifestBuilder : IDisposable
    { 
        private StreamWriter manifestWriter;

        public string Output => manifestWriter.ToString();
        
        /// <summary>
        /// Create new .m3u8 file and give access to writing to it
        /// </summary>
        /// <param name="filepath"></param>
        public ManifestBuilder(string filepath)
        {
            if (File.Exists(filepath))
            {
                manifestWriter = new StreamWriter(filepath);
            }

            manifestWriter.WriteLine("#EXTM3U");
            manifestWriter.WriteLine("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="type">Type of playlist. Can be either VOD or EVENT</param>
        public ManifestBuilder(string filepath, string type) : this(filepath)
        {
            manifestWriter.WriteLine($"#EXT-X-PLAYLIST-TYPE:{type}");
        }

        public ManifestBuilder AddExtStreamInfo(string url, int bandwidth, string vcodec, string acodec, Rectangle resolution)
        {
            //# EXTM3U
            //# EXT-X-STREAM-INF:BANDWIDTH=1000,CODECS="mp4a.40.2, avc1.640028",RESOLUTION=640x480
            //https://localhost:5001/1000K/playlist-1000K.m3u8

            var sb = new StringBuilder();

            // TODO: Add variaty
            sb.Append("#EXT-X-STREAM-INF:");
            sb.Append($"BANDWIDTH={bandwidth},");
            sb.Append($"CODECS=\"{vcodec}, {acodec}\",");
            sb.Append($"RESOLUTION={resolution.X}x{resolution.Y},");

            var str = sb.ToString().TrimEnd(',');
            manifestWriter.WriteLine(str);
            manifestWriter.WriteLine(url);
            manifestWriter.WriteLine();

            return this;
        }

        public ManifestBuilder AddExtInf(string info, double duration)
        {
            manifestWriter.WriteLine($"#EXTINF:{duration}");
            manifestWriter.WriteLine(info);

            return this;
        }

        public ManifestBuilder AddEndPlaylist()
        {
            manifestWriter.WriteLine($"#EXT-X-ENDLIST");
            return this;
        }

        public ManifestBuilder AddMediaSequence(int sequence)
        {
            manifestWriter.WriteLine($"#EXT-X-MEDIA-SEQUENCE:{sequence}");
            return this;
        }

        public ManifestBuilder AddTargetDuration(int duration)
        {
            manifestWriter.WriteLine($"#EXT-X-TARGETDURATION:{duration}");
            return this;
        }

        public ManifestBuilder AddVersion(int version)
        {
            manifestWriter.WriteLine($"EXT-X-VERSION:{version}");
            return this;
        }

        public FileStream Build()
        {
            // Shitcoding here we see
            // The problem is: how we can cast StreamWriter to FileStream, but
            // with FileAccess options. Options are avaiable only in FileStream's construct, so
            // Maybe there is another way to do it
            // TODO: Try to set FileAccess in StremaWriter at the very begin
            manifestWriter.Flush();
            var fs = manifestWriter.BaseStream as FileStream;
            fs.Dispose();
            fs.Close();

            return new FileStream(fs.Name, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        #region Dispose
        public void Dispose()
        {
            if (manifestWriter != null)
                manifestWriter.Dispose();
        }

        #endregion
    }
}