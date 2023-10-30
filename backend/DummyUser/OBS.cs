using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using Xabe.FFmpeg;
using static Program.OBS;

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

        private string _hosiId;

        public string HostId
        {
            get { return _hosiId; }
            set { _hosiId = value; }
        }


        private string videoDir => "video";
        private string thumbnailsDir => "preview";
        private string playlistsDir => "playlists";

        private readonly string videoFileName;

        private string[] videos => new string[3]
        {
            "1.mp4",
            "2.mp4",
            "3.mov"
        };

        private BroadcastClient broadcastClient = new BroadcastClient();

        private Queue<Segment> segmentsBank = new Queue<Segment>();

        public OBS(User host)
        {
            this.host = host;

            broadcastClient.HslBasePath = OBS.ApiPath;

//            videoFileName = Path.Combine(videoDir, videos[ (new Random()).Next(0, videos.Length) ]);
            videoFileName = Path.Combine(videoDir, videos[ 1 ]);

            Log($"Booting OBS with video = {videoFileName} ...");

            string hostid = hostId.Replace("-", "");

            segmentsDir = Path.Combine("segments", hostid);

            FFmpeg.SetExecutablesPath("C:\\Program Files (x86)\\ffmpeg\\bin");

            Directory.CreateDirectory( Path.Combine(videoDir, hostIdFormatted) );
        }

        public async Task RunAsync()
        {
            Log($"user={host.username}. Sending the master manifest...");
            await broadcastClient.PostMasterPlaylist(host.id);

            Task updatingThumbnailTask = Task.Run(StartUpdatingThumbnail);

            Task retrieveSegmentsTask = Task.Run(StartRetreivingSegments);

            Task updatingPlaylistTask = Task.Run(StartUpdatingPlaylist);

            await Task.WhenAny(updatingThumbnailTask, retrieveSegmentsTask, updatingPlaylistTask);
        }

        bool repeat = true;

        private async Task StartRetreivingSegments()
        {
            TimeSpan startTime = new TimeSpan(0, 0, 0);

            //var result = await ProccesNextSegments(start: startTime);
            //return;
            while (true)
            {
                Log($"user={host.username}. Filling up the segments bank...");

                FragmentsProccedResult result = await ProccesNextSegments(start: startTime);

                if (result.EndOfFile)
                {
                    if (repeat)
                    {
                        startTime = new TimeSpan(0, 0, 0);
                    }
                    else
                    {
                        // halt
                    }
                }
                else
                {
                    startTime = result.TakenTime;
                }

                //Log("Current segments bank: + \n");
                //foreach (var item in segmentsBank)
                //{
                //    Log("\t" + item.ToString());
                //}

                Thread.Sleep(2000);
            }
        }

        private async Task StartUpdatingPlaylist()
        {
            string hostid = hostId.Replace("-", "");
            
            while (true)
            {
                Log($"user={host.username}. Start sending pending segments to HSL server...");

                while (segmentsBank.Any())
                {
                    Segment segment = segmentsBank.Dequeue();

                    Log($"Sending {segment.FileName} ...");
                    await broadcastClient.PostSegmentAsync(segment, hostid);
                }

                //// send the new playlist with 5 fragments
                //string output = Path.Combine(playlistsDir, hostid + ".m3u8");
                //broadcastClient.PostPlaylist(output);

                Thread.Sleep(500);
            }
        }

        private string segmentsDir;

        private static int chunkSize = 5;

        private int segmentDuration = 5;

        private struct FragmentsProccedResult
        {
            public bool EndOfFile { get; set; }

            public TimeSpan TakenTime { get; set; }

            public FragmentsProccedResult(TimeSpan takenTime, bool endOfFile)
            {
                TakenTime = takenTime;
                EndOfFile = endOfFile;
            }
        }

        UInt64 segmentIndex = 1;

        /// <summary>
        /// Retrieve the next 5 segments and adds it to the bank
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        private async Task<FragmentsProccedResult> ProccesNextSegments(TimeSpan start)
        {
            bool endOfFile = false;

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(videoFileName);

            IStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                                    ?.SetCodec(VideoCodec.h264);

            int duration = mediaInfo.Duration.Seconds;

            Log($"Start procceding video={videoFileName}, duration={duration}");

            // seconds
            int position = start.Seconds;

            int segmentDuration;

            for (int i = 0; i < chunkSize; i++)
            {
                if (duration < 0) break;

                string sfn = $"segment{(segmentIndex)}.ts";
                string output = Path.Combine(segmentsDir, sfn);

                Log($"Procceding {sfn}. offset={position} sec.");

                IConversionResult result = FFmpeg.Conversions.New()
                    .AddStream<IStream>(videoStream)
                    .AddParameter($"-ss {TimeSpan.FromSeconds(position)} -t {TimeSpan.FromSeconds(this.segmentDuration)}")
                    .SetOutput(output)
                    .Start().GetAwaiter().GetResult();

                position += this.segmentDuration;
                duration -= this.segmentDuration;

                if (duration <= 0)
                {
                    endOfFile = true;
                    segmentDuration = mediaInfo.Duration.Seconds - (position - this.segmentDuration);
                }
                else
                {
                    segmentDuration = this.segmentDuration;
                }

                segmentsBank.Enqueue(
                    new Segment { Duration = segmentDuration, FileName = sfn, _debugInfo = $"duration={segmentDuration} sec, offset={position} sec" }
                );

                segmentIndex++;
            }

            return new FragmentsProccedResult { TakenTime = new TimeSpan(0, 0, position), EndOfFile = endOfFile };
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
                await broadcastClient.PostThumbnail(thumbnailPath: output);
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

            public string _debugInfo;

            public override string ToString()
            {
                return $"{FileName} {_debugInfo}";
            }
        }

        public void Dispose()
        {
            broadcastClient.Dispose();
        }
    }
}