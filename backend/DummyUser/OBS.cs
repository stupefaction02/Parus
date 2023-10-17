using System.Text;

internal partial class Program
{
    public class OBS
    {
        private static string ApiUri = "https://localhost:2020/live";

        private readonly User host;

        public OBS(User host)
        {
            this.host = host;
        }

        public async Task RunAsync()
        {
            string masterManifest = CreateMasterManifest();

            string 
        }

        private string CreateMasterManifest()
        {
            StringBuilder sb = new StringBuilder();

            string hostid = host.id.Replace("-", "");

            sb.Append("#EXTM3U");
            sb.Append("#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=6221600,CODECS=\"mp4a.40.2,avc1.640028\",RESOLUTION=1920x1080,NAME=\"1080\"");
            sb.Append($"{OBS.ApiUri}/{hostid}/1000K/playlist-1000K.m3u8");

            Console.WriteLine(host.username + ". Created manifest:");
            Console.WriteLine(sb.ToString());

            return sb.ToString();
        }
    }
}