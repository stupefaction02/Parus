using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Xabe.FFmpeg;

internal partial class Program
{
    public class OBS : IDisposable
    {
        private static string ApiPath => "https://localhost:2020";

        private readonly User host;
        private string hostId => host.id;

        private HttpClient webClient = new HttpClient();

        private int second = 1;

        private string videoDir => "video";
        private string thumbnailsDir => "preview";

        private readonly string videoFileName;

        private string[] videos => new string[3]
        {
            "1.mp4",
            "2.mp4",
            "3.mov"
        };

        private BroadcastClient hslClient = new BroadcastClient();

        public OBS(User host)
        {
            this.host = host;

            hslClient.HslBasePath = OBS.ApiPath;

            videoFileName = videos[ (new Random()).Next(0, videos.Length) ];

            FFmpeg.SetExecutablesPath("C:\\Program Files (x86)\\ffmpeg\\bin");
        }

        public async Task RunAsync()
        {
            Log($"user={host.username}. Sending the master manifest...");
            await hslClient.PostMasterPlaylist(host.id);

            while (true)
            {
                Log($"user={host.username}. Updating the thumbnail broadcast...");
                await UpdateThumbnail();

                Thread.Sleep(60000);
            }
        }

        private void Log(string msg)
        {
            Console.WriteLine(msg);
        }

        private async Task UpdateThumbnail()
        {
            string hostid = hostId.Replace("-", "");

            string output = Path.Combine(thumbnailsDir, hostid + ".png");
            string input = Path.Combine(videoDir, videoFileName);

            if (!File.Exists(input))
            {
                throw new IOException($"Could find video sample. Add {videoFileName} to video folder in bin folder.");
            }

            IConversion conversion = await
                FFmpeg.Conversions.FromSnippet.Snapshot(input, output, TimeSpan.FromSeconds(second));

            IConversionResult result = await conversion.Start();

            if (result != null) 
            {
                await hslClient.PostThumbnail(thumbnailPath: output);
            }
        }

        private async Task SendMasterPlaylist()
        {
            string hostid = host.id.Replace("-", "");
            string fn = "manifest/" + "manifest_" + hostid.Substring(0, 12) + ".m3u8";

            using FileStream fs = File.Create(fn);
            using StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine("#EXTM3U");
            sw.WriteLine("#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=6221600,CODECS=\"mp4a.40.2,avc1.640028\",RESOLUTION=1920x1080,NAME=\"1080\"");
            sw.WriteLine($"{OBS.ApiPath}/live/{hostid}/1000K/playlist-1000K.m3u8");

            Console.WriteLine(host.username + ". Created manifest:");

            string uri = ApiPath + $"/uploadManifest?usrDirectory={hostid}";
            Console.WriteLine(host.username + ". Sending manifest to " + uri);

            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                StreamContent fileContent = new StreamContent(fs);

                //content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                content.Add(fileContent, "file", Path.GetFileName(fn));

                HttpResponseMessage response = await webClient.PostAsync(uri, content);

                if (!response.IsSuccessStatusCode)
                {
                    //// log
                    //fileContent.Dispose();
                    //content.Dispose();
                    //response.Dispose();
                    //Thread.Sleep(5000);
                    //goto SEND;
                }
                else
                {
                    sw.Close();
                    return;
                }
            }
        }

        private string CreatePlaylist()
        {
            StringBuilder sb = new StringBuilder();

            string hostid = host.id.Replace("-", "");

            sb.Append("#EXTM3U");
            sb.Append("#EXT-X-TARGETDURATION:11");
            sb.Append("#EXT-X-VERSION:3");
            sb.Append("#EXT-X-PLAYLIST-TYPE:VOD");
            


            Console.WriteLine(host.username + ". Created manifest:");
            Console.WriteLine(sb.ToString());

            return sb.ToString();
        }

        public void Dispose()
        {
            hslClient.Dispose();
        }
    }

    public class BroadcastClient : IDisposable
    {
        private HttpClient webClient = new HttpClient();

        public string HslBasePath { get; set; }

        public string ImagesBasePath { get; set; }

        public void Dispose()
        {
            webClient.Dispose();
        }

        public async Task PostThumbnail(string thumbnailPath)
        {
            using FileStream fs = File.OpenRead(thumbnailPath);

            string uri = "https://localhost:5004/upload";

            await PostFile(fs, uri);
        }

        public async Task PostMasterPlaylist(string hostId, string manifestPath = null)
        {
            string hostid = hostId.Replace("-", "");
            string fn = "manifest/" + "manifest_" + hostid.Substring(0, 12) + ".m3u8";

            using FileStream fs = File.Create(fn);
            using MemoryStream ms = new MemoryStream();
            using StreamWriter sw = new StreamWriter(ms);

            sw.WriteLine("#EXTM3U");
            sw.WriteLine("#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=6221600,CODECS=\"mp4a.40.2,avc1.640028\",RESOLUTION=1920x1080,NAME=\"1080\"");
            sw.WriteLine($"{HslBasePath}/live/{hostid}/1000K/playlist-1000K.m3u8");

            //Console.WriteLine(host.username + ". Created manifest:");

            string uri = HslBasePath + $"/uploadManifest?usrDirectory={hostid}";
            //Console.WriteLine(host.username + ". Sending manifest to " + uri);

            await sw.BaseStream.CopyToAsync(fs);

            sw.Close();

            await PostFile(fs, uri);
        }

        private async Task PostFile(FileStream fs, string uri)
        {
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                StreamContent fileContent = new StreamContent(fs);

                content.Add(fileContent, "file", Path.GetFileName(fs.Name));

                HttpResponseMessage response = await webClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
        }
    }
}