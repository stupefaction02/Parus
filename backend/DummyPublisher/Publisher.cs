using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

using Naturistic.Core.Extensions;
using Newtonsoft.Json;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;

namespace DummyPublisher
{
    public class Publisher : IDisposable
    {
        /// <summary>
        /// Channel session key. Retrieved a new one from the server everytime publisher starts broadcasting.
        /// Is used in crypting segment file name with hash function
        /// </summary>
        private string channelSessionKey;

        private bool isRecording = false;

        private int channelId;

        private int[] availableBitrates = new int[1] { 1000 };

        /// <summary>
        /// URL of the local HSL microservice
        /// </summary>
        private readonly string localHlsMServiceUrl = "https://localhost:2020";

        private readonly string mainServerApiUrl = "https://localhost:3939/api";

        private readonly MD5 md5 = MD5.Create();

        private HttpClient client = new HttpClient();

        /// <summary>
        /// Base HLS url
        /// </summary>
        private readonly string bhlsurl = "https://localhost:2020/hls/live";

        // test
        private string[] files = new string[5]
        {
            @"video/segment1-1000K.ts",
            @"video/segment2-1000K.ts",
            @"video/segment3-1000K.ts",
            @"video/segment4-1000K.ts",
            @"video/segment5-1000K.ts"
        };

        public Publisher()
        {
            // 1. Registering channel, but right now we don't really need it

            // 2. We must be sure that the channel exists on server

            // 3. Start session and and store it o a server, then get channel key, that represts that info
            // TODO: Get id from local store
            channelId = 1;
            Console.WriteLine("Trying to retrive channel key...");

            try
            {
                var resString = client.GetStringAsync($"{mainServerApiUrl}/broadcasts/startSession?channelId={channelId}").Result;

                if (!String.IsNullOrEmpty(resString))
                {
                    // TODO: Good naming :<
                    var raw = JsonConvert.DeserializeObject<Dictionary<string, string>>(resString);

                    if (raw != null)
                    {
                        channelSessionKey = raw["channelSessionKey"];

                        Task.Run(() => InitManifestsAsync());

                        Console.WriteLine($"Starting session... key: {channelSessionKey}");
                    }
                }
                else
                {
                    Console.WriteLine("Error! Press any key...");
                    Console.ReadKey();

                    return;
                }
            }
            catch (HttpRequestException)
            {
                Console.WriteLine($"Can't connect to the main server. It might be that the server is shutdown right now.");
            }
        }

        private async Task InitManifestsAsync()
        {
            // 4. Create manifest for the different bitrates
            // Since we are in a test mode, we got only 1000kbps

            // Create master manifest
            var masterManifestBuilder = new ManifestBuilder(@$"video/master-manifest-{channelId}.m3u8");

            foreach (var abitrate in availableBitrates)
            {
                var pattern = $"{channelSessionKey}manifest{abitrate}k";

                var fileNameHash = md5.CreateHash(pattern);
                var fn = $"{fileNameHash}.m3u8";

                masterManifestBuilder = masterManifestBuilder.AddExtStreamInfo($"{bhlsurl}/files?fn={fn}", abitrate,
                    "mp4a.40.2", "avc1.640028", new Rectangle { X = 640, Y = 480 });

                var manifestBuilder = new ManifestBuilder(Path.Combine(@"video/1000k", fn));
                manifestBuilder.AddVersion(3).AddTargetDuration(10).AddMediaSequence(0);

                foreach (var segment in this.files)
                {
                    manifestBuilder.AddExtInf(segment, 1.0000);
                }

                manifestBuilder.AddEndPlaylist();

                var pfit = PostFileInternalAsync(manifestBuilder.Build(), fn);
                await pfit.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        masterManifestBuilder.Dispose();
                });
            }

            var pft = PostFileAsync(masterManifestBuilder.Build(), "master_manifest");
            await pft.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    masterManifestBuilder.Dispose();
            });
        }

        private async Task PostFileAsync(string fp, string fileSalt)
            => await PostFileAsync(File.Open(fp, FileMode.OpenOrCreate, FileAccess.Read), fileSalt);

        /// <summary>
        /// Post file on a HLS service.  
        /// </summary>
        /// <param name="fs">Stream of a file</param>
        /// <param name="fileSalt">Salt added to a md5 encryption. Used in a client to decrypt filename</param>
        private async Task PostFileAsync(FileStream fs, string fileSalt = "")
        {
            var fileName = $"{channelSessionKey}{fileSalt}";

            // #!
            // Matter to note, that in order to get the hash identicate to http://www.md5.cz/ and
            // CryptoJS library hashes we must set ASCII encoding
            string fileNameHash = md5.CreateHash(fileName);
            fileNameHash = $"{fileNameHash}{Path.GetExtension(fs.Name)}";

            var task = PostFileInternalAsync(fs, fileNameHash);
            await task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    fs.Dispose();
            });
        }

        /// <summary>
        /// Posts file
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="fileNameHash">Hashed filename with extension</param>
        /// <returns></returns>
        private async Task PostFileInternalAsync(FileStream fs, string fileNameHash)
        {
            if (fs != null)
            {
                if (!fs.CanRead)
                    throw new InvalidOperationException("FileStream aren't supposed to be unreadable.");

                string dest = $"{localHlsMServiceUrl}/hls/live/files";

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StreamContent(fs), "segmentFile", fileNameHash);

                    var request = new HttpRequestMessage(HttpMethod.Post, dest);

                    Console.WriteLine($"Sending file... name: {fileNameHash}, file: {Path.GetFileName(fs.Name)}, destination: {dest}");

                    request.Headers.Add("Cache-Control", "no-cache");
                    request.Headers.Add("Accept", "*/*");
                    request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                    request.Content = content;

                    // we don't care about answer 
                    await client.SendAsync(request);
                }

                // TODO:
                // You know, I can't come up an elegant solution that relates the fact that if sendAsync throws an exception
                // the file stream will left undisposed

                await fs.DisposeAsync();
            }
        }

        public void Start() => Task.Run(async () => await StartAsync());

        public async Task StartAsync()
        {
            isRecording = true;

            int fileSalt = 1;
            int fileSegmentIndex = 0;

            Console.WriteLine("type: HttpClient");

            try
            {
                while (isRecording)
                {
                    await PostFileAsync(files[fileSegmentIndex], fileSalt.ToString());

                    fileSalt += 1;
                    fileSegmentIndex++;

                    if (fileSegmentIndex == files.Length)
                        fileSegmentIndex = 0;

                    Thread.Sleep(500);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType().Name}: {e.Message}");
            }
        }

        public void Dispose()
        {
            if (md5 != null)
                md5.Dispose();

            if (client != null)
                client.Dispose();

            var t = Directory.GetFiles("video").Where(f => Path.GetExtension(f) == "m3u8");
            foreach (var o_r in t)
                File.Delete(o_r);
        }
    }

    internal class ManifestBuilder : IDisposable
    {
        private StreamWriter manifestWriter;

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

    public static class CryptoExtensions
    {
        /// <summary>
        /// Uses ASCII encoding
        /// </summary>
        /// <param name="md5"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CreateHash(this MD5 md5, string str)
        {
            var bytes = Encoding.ASCII.GetBytes(str);
            var retVal = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < retVal.Length; i++)
                sb.Append(retVal[i].ToString("x2"));

            return sb.ToString();
        }
    }
}
