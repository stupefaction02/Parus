using System.Net;
using System.Net.Http.Headers;
using System.Text;
using FFmpeg;

internal partial class Program
{
    public class OBS
    {
        private static string ApiUri => "https://localhost:2020";

        private readonly User host;

        private HttpClient webClient = new HttpClient();

        private int frameIndex = 0;

        private string videoDir => "video/";

        public OBS(User host)
        {
            this.host = host;
        }

        public async Task RunAsync()
        {
            await SendMasterPlaylist();

            while (true)
            {
                await UpdatePreview();

                Thread.Sleep(5000);
            }

            //CreatePlaylist();
        }

        private async Task UpdatePreview()
        {
            string videoPath = Path.Combine(videoDir, "1.mp4");


        }

        private async Task SendMasterPlaylist()
        {
            string hostid = host.id.Replace("-", "");
            string fn = "manifest/" + "manifest_" + hostid.Substring(0, 12) + ".m3u8";

            using FileStream fs = File.Create(fn);
            using StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine("#EXTM3U");
            sw.WriteLine("#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=6221600,CODECS=\"mp4a.40.2,avc1.640028\",RESOLUTION=1920x1080,NAME=\"1080\"");
            sw.WriteLine($"{OBS.ApiUri}/live/{hostid}/1000K/playlist-1000K.m3u8");

            Console.WriteLine(host.username + ". Created manifest:");

            string uri = ApiUri + $"/uploadManifest?usrDirectory={hostid}";
            Console.WriteLine(host.username + ". Sending manifest to " + uri);

        SEND:
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
    }
}