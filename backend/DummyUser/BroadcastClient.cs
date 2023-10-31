internal partial class Program
{
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

            string uri = HslBasePath + $"/uploadManifest?usrDirectory={hostid}";

            await sw.FlushAsync();
            ms.Seek(0, SeekOrigin.Begin);

            await ms.CopyToAsync(fs);

            sw.Close();

            await PostFile(fs, uri);
        }

        private async Task PostFile(FileStream fs, string uri)
        {
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                fs.Seek(0, SeekOrigin.Begin);

                StreamContent fileContent = new StreamContent(fs);

                content.Add(fileContent, "file", Path.GetFileName(fs.Name));

                HttpResponseMessage response = await webClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
        }

        public async Task PostSegmentAsync(OBS.Segment segment, string hostid)
        {
            using FileStream fs = File.OpenRead(segment.Path);

            string uri = HslBasePath + $"/uploadSegment?usrDirectory={hostid}";

            await PostFile(fs, uri);
        }

        public async Task PostPlaylists((string, Stream)[] streams, string hostid)
        {
            string uri = HslBasePath + $"/uploadPlaylists?usrDirectory={hostid}";

            await PostFiles(streams, uri);
        }

        private async Task PostFiles((string, Stream)[] streams, string uri)
        {
            using (MultipartFormDataContent content = new MultipartFormDataContent())
            {
                int index = 0;
                foreach (var tuple in streams)
                {
                    string name = tuple.Item1;
                    Stream stream = tuple.Item2;

                    stream.Seek(0, SeekOrigin.Begin);

                    StreamContent fileContent = new StreamContent(stream);

                    index++;
                    content.Add(fileContent, "file" + index, name);
                }

                HttpResponseMessage response = await webClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
        }
    }
}