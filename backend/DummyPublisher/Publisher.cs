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

namespace DummyPublisher
{
    public class Publisher : IDisposable
    {
        /// <summary>
        /// Channel session key. Retrieved a new one from the server everytime publisher starts broadcasting.
        /// Is used in crypting segment file name with hash function
        /// </summary>
        private string chSessionKey;

        private bool isRecording = false;

        /// <summary>
        /// URL of the local HSL microservice
        /// </summary>
        private readonly string localHlsMServiceUrl = "https://localhost:2020";

        private readonly string mainServerApiUrl = "https://localhost:3939/api";

        private readonly System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

        private HttpClient client = new HttpClient();

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
            int channelId = 1;
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
                        chSessionKey = raw["channelSessionKey"];

                        // 4. Create manifest for the different bitrates
                        // Since we are in a test mode, we got only 1000kbps

                        // Create master manifest
                        var mmbuilder = new ManifestBuilder(@$"video/master-manifest-{channelId}.m3u8");
                        mmbuilder = mmbuilder.AddExtStreamInfo("https://localhost:5001/1000K/playlist-1000K.m3u8", 1000,
                            "mp4a.40.2", "avc1.640028", new Rectangle { X = 640, Y = 480 });

                        var lmt = Task.Run(async () => await PostFileAsync(mmbuilder.Build(), 1));

                        lmt.Wait();

                        Console.WriteLine($"Starting session... key: {chSessionKey}");
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

        public void Start() => Task.Run(async () => await StartAsync());

        public async Task StartAsync()
        {
            isRecording = true;

            int fileIndex = 1;
            int _i = 0; // :)

            Console.WriteLine("type: HttpClient");

            try
            {
                while (isRecording)
                {
                    await PostFileAsync(files[_i], fileIndex);

                    fileIndex += 1;
                    _i++;

                    if (_i == files.Length)
                        _i = 0;

                    Thread.Sleep(3000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType().Name}: {e.Message}");
            }
        }

        private async Task PostFileAsync(FileStream fs, int fileIndex)
        {
            if (fs != null)
            {
                try
                {
                    var fileName = $"{chSessionKey}{fileIndex}";

                    byte[] retVal = md5.ComputeHash(Encoding.Unicode.GetBytes(fileName));
                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < retVal.Length; i++)
                        sb.Append(retVal[i].ToString("x2"));

                    string dest = $"{localHlsMServiceUrl}/hls/live/files";

                    using (var content = new MultipartFormDataContent())
                    {
                        // TODO: Add disposal support for fs
                        string nfn = $"{sb}{Path.GetExtension(fs.Name)}";

                        content.Add(new StreamContent(fs), "SegmentFile", nfn);

                        var request = new HttpRequestMessage(HttpMethod.Post, dest);

                        Console.WriteLine($"Sending file... name: {nfn}, file: {Path.GetFileName(fs.Name)}, destination: {dest}");

                    //    request.Headers.Add("Cache-Control", "no-cache");
                        request.Headers.Add("Accept", "*/*");
                        request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                        request.Content = content;

                        // we don't care about answer 
                        await client.SendAsync(request);
                    }
                }
                catch (HttpRequestException hrex)
                {
                    Console.WriteLine($"Can't connect to the HLS service. It might be that the server is shutdown right now.");
                    return;
                }
                catch (NotSupportedException nsex)
                {
                    var y = nsex.Message;
                }
            }
        }

        private async Task PostFileAsync(string fp, int fileIndex)
        {
            await PostFileAsync(File.Open(fp, FileMode.OpenOrCreate, FileAccess.Read), fileIndex);
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

        public FileStream Build()
        {
            // Shitcoding here we see
            // The problem is: how we can cast StreamWriter to FileStream, but
            // with FileAccess options. Options are avaiable only in FileStream's construct, so
            // Maybe there is another way to do it
            // TODO: Try to set FileAccess in StremaWriter at the very begin
            manifestWriter.Flush();
            var fs = manifestWriter.BaseStream as FileStream;
            fs.Close();

            return new FileStream(fs.Name, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public void Dispose()
        {
            if (manifestWriter != null)
                manifestWriter.Dispose();
        }
    }
}
