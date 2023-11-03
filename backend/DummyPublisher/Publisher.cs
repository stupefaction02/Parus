using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Parus.Core.Extensions;
using Common.Extensions;
using Newtonsoft.Json;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Net.Sockets;
using Common.Publisher;

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

        private int[] availableBitrates = {1000};

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
            Console.WriteLine("Trying to retrieve channel key...");

            try
            {
                string requestUri = $"{mainServerApiUrl}/broadcasts/startSession?channelId={channelId}";
                string resString = client.GetStringAsync(requestUri).Result;

                if (!String.IsNullOrEmpty(resString))
                {
                    // TODO: Good naming :<
                    var raw = JsonConvert.DeserializeObject<Dictionary<string, string>>(resString);

                    if (raw == null) return;

                    channelSessionKey = raw["channelSessionKey"];

                    Task.Run(UpdateMasterManifestsAsync);

                    Console.WriteLine($"Starting session... key: {channelSessionKey}");
                }
                else
                {
                    Console.WriteLine("Error! Press any key...");
                    Console.ReadKey();
                }
            }
            catch (HttpRequestException)
            {
                Console.WriteLine(
                    $"Can't connect to the main server. It might be that the server is shutdown right now.");
            }
            catch (SocketException)
            {
                Console.WriteLine(
                    $"Can't connect to the main server. It might be that the server is shutdown right now.");
            }
        }

        private async Task UpdateMasterManifestsAsync()
        {
            // 4. Create manifest for the different bitrates
            // Since we are in a test mode, we got only 1000kbps

            // Create master manifest
            var masterManifestBuilder = new ManifestBuilder(@$"video/master-manifest-{channelId}.m3u8");

            foreach (int abitrate in availableBitrates)
            {
                var pattern = $"{channelSessionKey}manifest{abitrate}k";

                var fileNameHash = md5.CreateHash(pattern);
                var fn = $"{fileNameHash}.m3u8";

                manifestFileNameByBitrate.Add(abitrate, fn);
                
                masterManifestBuilder = masterManifestBuilder.AddExtStreamInfo($"{bhlsurl}/files?fn={fn}", abitrate,
                    "mp4a.40.2", "avc1.640028", new Rectangle {X = 640, Y = 480});
            }

            Console.WriteLine($"master-manifest-{channelId}: \n\t {masterManifestBuilder.Output}");

            await PostManifestAsync(masterManifestBuilder.Build(), "master_manifest");
        }
        
        private Dictionary<int, string> manifestFileNameByBitrate 
            = new Dictionary<int, string>();
        
        private void UpdateManifestAsync(int bitrate, string[] segments)
        {
            string fn = manifestFileNameByBitrate[bitrate];
            var manifestBuilder = new ManifestBuilder(Path.Combine(@"video/1000k", fn));
            manifestBuilder.AddVersion(3).AddTargetDuration(10).AddMediaSequence(0);
            
            foreach (var segment in segments)
            {
                manifestBuilder.AddExtInf(segment, 1.0000);
            }
            
            manifestBuilder.AddEndPlaylist();
        }
        
        private async Task PostManifestAsync(FileStream fs, string fileSalt = "")
        {
            var fileName = $"{channelSessionKey}{fileSalt}";

            // #!
            // Matter to note, that in order to get the hash identicate to http://www.md5.cz/ and
            // CryptoJS library hashes we must set ASCII encoding
            string fileNameHash = md5.CreateHash(fileName);
            fileNameHash = $"{fileNameHash}{Path.GetExtension(fs.Name)}";

            var task = PostManifestInternalAsync(fs, fileNameHash);
            await task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    fs.Dispose();
            });
        }
        
        private async Task PostManifestInternalAsync(FileStream fs, string fileNameHash)
        {
            if (fs != null)
            {
                if (!fs.CanRead)
                    throw new InvalidOperationException("FileStream aren't supposed to be unreadable.");

                string dest = $"{localHlsMServiceUrl}/hls/live/playlists";

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StreamContent(fs), "playlistFile", fileNameHash);

                    var request = new HttpRequestMessage(HttpMethod.Post, dest);

                    Console.WriteLine(
                        $"Sending file... name: {fileNameHash}, file: {Path.GetFileName(fs.Name)}, destination: {dest}");

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
        
        private async Task PostSegmentAsync(string fp, string fileSalt)
            => await PostSegmentAsync(File.Open(fp, FileMode.OpenOrCreate, FileAccess.Read), fileSalt);

        /// <summary>
        /// Post segment on a HLS service.  
        /// </summary>
        /// <param name="fs">Stream of a file</param>
        /// <param name="fileSalt">Salt added to a md5 encryption. Used in a client to decrypt filename</param>
        private async Task PostSegmentAsync(FileStream fs, string fileSalt = "")
        {
            var fileName = $"{channelSessionKey}{fileSalt}";

            // #!
            // Matter to note, that in order to get the hash identicate to http://www.md5.cz/ and
            // CryptoJS library hashes we must set ASCII encoding
            string fileNameHash = md5.CreateHash(fileName);
            fileNameHash = $"{fileNameHash}{Path.GetExtension(fs.Name)}";

            var task = PostSegmentInternalAsync(fs, fileNameHash);
            await task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    fs.Dispose();
            });
        }

        private async Task PostSegmentInternalAsync(FileStream fs, string fileNameHash)
        {
            if (fs != null)
            {
                if (!fs.CanRead)
                    throw new InvalidOperationException("FileStream aren't supposed to be unreadable.");

                string dest = $"{localHlsMServiceUrl}/hls/live/segments";

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StreamContent(fs), "segmentFile", fileNameHash);

                    var request = new HttpRequestMessage(HttpMethod.Post, dest);

                    Console.WriteLine(
                        $"Sending file... name: {fileNameHash}, file: {Path.GetFileName(fs.Name)}, destination: {dest}");

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
                // make 5 one-second files... for all bitrates(?) 
                // then update manifest
                while (isRecording)
                {
                    await PostSegmentAsync(files[fileSegmentIndex], fileSalt.ToString());

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
}