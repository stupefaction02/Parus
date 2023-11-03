using System.Text;

internal partial class Program
{

    public class ManifestBuilder
    {
        private List<string> lines = new List<string>();

        public ManifestBuilder()
        {
            lines.Add("#EXTM3U");
        }

        public void AddPlaylist(/* TODO: params */ string url)
        {
            lines.Add("#EXT-X-STREAM-INF:PROGRAM-ID=1,BANDWIDTH=6221600,CODECS=\"mp4a.40.2,avc1.640028\",RESOLUTION=1920x1080,NAME=\"1080\"");
            lines.Add(url);
        }

        public string BuildString()
        {
            return String.Join(Environment.NewLine, lines);
        }
    }

    public class PlaylistBuilder
    {
        private List<string> lines = new List<string>();

        private Dictionary<string, List<string>> headersStore = new Dictionary<string, List<string>>();

        public void AddQuality(string name)
        {
            if (headersStore.ContainsKey(name))
            {
                headersStore.Remove(name);
            }

            List<string> headerLines = new List<string>
            {
                "#EXTM3U",
                "#EXT-X-TARGETDURATION:11",
                "#EXT-X-VERSION:3",
                "#EXT-X-PLAYLIST-TYPE:VOD"
            };

            headersStore.Add(name, headerLines);
        }


        public void AddSegment(double duration, string path)
        {
            lines.Add($"#EXTINF:{duration}");
            lines.Add(path);
        }

        public string BuildStringAll() 
        {
            string ret = "";

            foreach (var header in headersStore)
            {
                List<string> headerLines = header.Value;

                headerLines.AddRange(lines);

                headerLines.Add("#EXT-X-ENDLIST");

                ret += String.Join(Environment.NewLine, headerLines);

                ret += Environment.NewLine;
            }

            return ret;
        }

        public Stream BuildStream()
        {
            lines.Add("#EXT-X-ENDLIST");

            string s = string.Join(Environment.NewLine, lines);

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
            FileStream fs = File.Create("playlists/playlist" + Guid.NewGuid().ToString() + ".m3u8");

            stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);
            stream.Flush();
            stream.CopyTo(fs);

            return fs;
        }
    }
}