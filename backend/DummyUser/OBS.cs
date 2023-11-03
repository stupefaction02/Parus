using System.Net;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using Xabe.FFmpeg;
using static Program;
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

        private PlaylistBuilder playlistBuilder = new PlaylistBuilder();

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

            playlistBuilder.AddQuality("180");
            playlistBuilder.AddQuality("360");
            playlistBuilder.AddQuality("720");
        }

        public async Task RunAsync()
        {
            Task updatingThumbnailTask = Task.Run(StartUpdatingThumbnail);

            Task retrieveSegmentsTask = Task.Run(StartRetreivingSegments);

            Task updatingPlaylistTask = Task.Run(StartSendingSegments);

            await Task.WhenAny(updatingThumbnailTask, retrieveSegmentsTask, updatingPlaylistTask);
        }

        private async Task SendManifest()
        {
            Log($"user={host.username}. Sending the master manifest...");
            await broadcastClient.PostMasterPlaylist(hostId);
        }

        bool repeat = true;

        private async Task StartRetreivingSegments()
        {
            TimeSpan startTime = new TimeSpan(0, 0, 0);

            Log($"user={host.username}. Filling up the segments bank... Thread id={Thread.CurrentThread.ManagedThreadId}");

            while (true)
            {
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

                Thread.Sleep(2000);
            }
        }

        private object locker = new object();

        private void StartSendingSegments()
        {
            int segmentsSent = 0;

            string hostid = hostId.Replace("-", "");

            Log($"user={host.username}. Start sending pending segments to HSL server... Thread id={Thread.CurrentThread.ManagedThreadId}");

            while (true)
            {
                lock (locker)
                {
                    while (segmentsBank.Any())
                    {
                        Segment segment = segmentsBank.Dequeue();

                        segmentsSent++;
                        Log($"user={host.username}. Sending {segment.FileName}. Already sent={segmentsSent} ...");
                        broadcastClient.PostSegmentAsync(segment, hostid).GetAwaiter().GetResult();

                        playlistBuilder.AddSegment(segment.Duration, path: segment.FileName);

                        if (segmentsSent % 5 == 0)
                        {
                            SendPlaylist();
                        }
                    }

                    Thread.Sleep(500);
                }

                Thread.Sleep(500);
            }
        }

        private void SendPlaylist1()
        {
            Log($"user={host.username}. Sending playlists...");
            using (Stream stream720 = playlistBuilder.BuildStream())
            {
                (string, Stream)[] data = new (string, Stream)[3]
                {
                    ("playlist.m3u8", stream720),
                    ("playlist.m3u8", stream720),
                    ("playlist.m3u8", stream720)
                };

                broadcastClient.PostPlaylists1(data, hostIdFormatted);
            }
        }

        private void SendPlaylist()
        {
            Log($"user={host.username}. Sending playlists...");

            broadcastClient.PostPlaylists(playlistBuilder.BuildStringAll(), hostIdFormatted);
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

        string b;

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

                lock (locker)
                {
                    segmentsBank.Enqueue(
                        new Segment
                        {
                            Duration = segmentDuration,
                            FileName = sfn,
                            Path = output,
                            _debugInfo = $"duration={segmentDuration} sec, offset={position} sec"
                        }
                    );
                }

                segmentIndex++;
            }

            return new FragmentsProccedResult { TakenTime = new TimeSpan(0, 0, position), EndOfFile = endOfFile };
        }

        private async Task StartUpdatingThumbnail()
        {
            while (true)
            {
                Log($"user={host.username}. Updating the thumbnail broadcast... Thread id={Thread.CurrentThread.ManagedThreadId}");
                await UpdateThumbnail();

                Thread.Sleep(120000);
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

        public class Segment
        {
            public double Duration { get; set; }
            public string FileName { get; set; }

            public string Path { get; set; }

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

    public class PlaylistBuilder
    {
        private List<string> lines = new List<string>();

        private readonly int segmentsStartOffset;
        private int segmentsIndex;

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

            segmentsIndex += 2;
        }

        public string BuildStringAll() 
        {
            string ret = "";

            foreach (var header in headersStore)
            {
                List<string> headerLines = header.Value;

                headerLines.AddRange(lines);

                headerLines.Add("#EXT-X-ENDLIST");

                ret += string.Join(Environment.NewLine, headerLines);

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