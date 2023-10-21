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
        private string hostIdFormatted => host.id.Replace("-", "");

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

        private Queue<Segment> segmentsBank = new Queue<Segment>();

        public OBS(User host)
        {
            this.host = host;

            hslClient.HslBasePath = OBS.ApiPath;

            videoFileName = Path.Combine(videoDir, videos[ (new Random()).Next(0, videos.Length) ]);

            string hostid = hostId.Replace("-", "");

            segmentsDir = Path.Combine("segments", hostid);

            FFmpeg.SetExecutablesPath("C:\\Program Files (x86)\\ffmpeg\\bin");

            Directory.CreateDirectory( Path.Combine(videoDir, hostIdFormatted) );
        }

        public async Task RunAsync()
        {
            Log($"user={host.username}. Sending the master manifest...");
            await hslClient.PostMasterPlaylist(host.id);

            Task updatingThumbnailTask = Task.Run(StartUpdatingThumbnail);

            Task retrieveSegmentsTask = Task.Run(StartRetreivingSegments);

            Task updatingPlaylistTask = Task.Run(StartUpdatingPlaylist);

            await Task.WhenAny(updatingThumbnailTask, retrieveSegmentsTask, updatingPlaylistTask);
        }

        TimeSpan startTime = new TimeSpan(0, 0, 0);

        private async Task StartRetreivingSegments()
        {
            while (true)
            {
                Log($"user={host.username}. Filling up the segments bank...");

                await ProccesNextSegments(start: startTime);

                Thread.Sleep(15000);
            }
        }

        private async Task StartUpdatingPlaylist()
        {
            while (true)
            {
                Thread.Sleep(15000);
            }
        }

        private string segmentsDir;

        private static int chunkSize = 5;

        private int segmentDuration = 5;

        /// <summary>
        /// Retrieve the next 5 segments and adds it to the bank
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        private async Task ProccesNextSegments(TimeSpan start)
        {
            int segmentIndex = 0;

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(videoFileName);

            IStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                                    ?.SetCodec(VideoCodec.h264);

            int duration = mediaInfo.Duration.Seconds;

            int position = start.Seconds;

            int segmentDuration;

            for (int i = 0; i < chunkSize; i++)
            {
                if (duration < 0) break;

                string sfn = "segment" + (segmentIndex + 1) + ".ts";
                string output = Path.Combine(segmentsDir, sfn);

                position += this.segmentDuration;

                IConversionResult result = FFmpeg.Conversions.New()
                    .AddStream<IStream>(videoStream)
                    .AddParameter($"-ss {TimeSpan.FromSeconds(position)} -t {TimeSpan.FromSeconds(this.segmentDuration)}")
                    .SetOutput(output)
                    .Start().GetAwaiter().GetResult();

                duration -= this.segmentDuration;

                if (duration <= 0)
                {
                    segmentDuration = mediaInfo.Duration.Seconds - (position - this.segmentDuration);
                }
                else
                {
                    segmentDuration = this.segmentDuration;
                }

                segmentsBank.Enqueue(
                    new Segment { Duration = segmentDuration, FileName = sfn }
                );

                segmentIndex++;
            }
        }

        private async Task StartUpdatingThumbnail()
        {
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
            string input = videoFileName;

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

        public class Segment
        {
            public double Duration { get; set; }
            public string FileName { get; set; }
        }

        public void Dispose()
        {
            hslClient.Dispose();
        }
    }
}