public class BroadcastClient : IDisposable
{
    private HttpClient webClient = new HttpClient(new HttpMessageHandler1(new HttpClientHandler()));

    public string HslBasePath { get; set; }

    public string ImagesBasePath { get; set; }

    public BroadcastClient()
    {
        //webClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip, deflate, br"));

        webClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
    }

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

    public void PostMasterPlaylist(string data, string hostId)
    {
        string uri = HslBasePath + $"/uploadManifest?usrDirectory={hostId}";

        PostRaw(data, uri);
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

    public async Task PostSegmentAsync(DeshOBS.Segment segment, string hostid)
    {
        using FileStream fs = File.OpenRead(segment.Path);

        string uri = HslBasePath + $"/uploadSegment?usrDirectory={hostid}";

        await PostFile(fs, uri);
    }

    public void PostPlaylists(string data, string hostid)
    {
        string uri = HslBasePath + $"/uploadPlaylists?usrDirectory={hostid}";

        PostRaw(data, uri);
    }

    private void PostRaw(string data, string uri)
    {
        using (MultipartFormDataContent content = new MultipartFormDataContent())
        {
            StringContent stringContent = new StringContent(data);

            content.Add(stringContent, "playlist");

            HttpResponseMessage response = webClient.PostAsync(uri, content).GetAwaiter().GetResult();
            Console.WriteLine($"Request url={uri}, ok={response.IsSuccessStatusCode}");
        }
    }

    public class HttpMessageHandler1 : DelegatingHandler
    {
        public HttpMessageHandler1(HttpMessageHandler innerHandler)
        : base(innerHandler)
        { }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

        Send:
            try
            {
                response = await base.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                goto Send;
            }

            return response;
        }
    }
}