using System.Net;
using System.Text;

internal partial class Program
{
    public class OBS
    {
        private static string ApiUri => "https://localhost:2020/live";

        private readonly User host;

        private WebClient webClient = new WebClient();

        public OBS(User host)
        {
            this.host = host;
        }

        public async Task RunAsync()
        {
            await SendMasterPlaylist();

            while (true)
            {

                Thread.Sleep(5000);
            }

            //CreatePlaylist();
        }

        private async Task SendMasterPlaylist()
        {
            string hostid = host.id.Replace("-", "");
            string fn = "manifest/" + "manifest_" + hostid.Substring(0, 12) + ".m3u8";

            using FileStream fs = File.Create(fn);
            using StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine("#EXTM3U");
            sw.WriteLine("#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=6221600,CODECS=\"mp4a.40.2,avc1.640028\",RESOLUTION=1920x1080,NAME=\"1080\"");
            sw.WriteLine($"{OBS.ApiUri}/{hostid}/1000K/playlist-1000K.m3u8");

            Console.WriteLine(host.username + ". Created manifest:");

            string uri = ApiUri + $"/{hostid}/master_playing.m3u8";
            Console.WriteLine(host.username + ". Sending manifest to " + uri);
            webClient.UploadFileAsync(new Uri(uri), fn);
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