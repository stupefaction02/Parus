﻿using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using Xabe.FFmpeg;
using static Program;

public abstract class Obs
{
    public abstract Task RunAsync();
}

public class DeshOBS : Obs, IDisposable
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
    private ManifestBuilder manifestBuilder = new ManifestBuilder();

    public DeshOBS(User host)
    {
        this.host = host;

        broadcastClient.HslBasePath = DeshOBS.ApiPath;

        videoFileName = Path.Combine(videoDir, videos[(new Random()).Next(0, videos.Length)]);
        //videoFileName = Path.Combine(videoDir, videos[ 1 ]);

        Log($"Booting OBS with video = {videoFileName} ...");

        string hostid = hostId.Replace("-", "");

        segmentsDir = Path.Combine("segments", hostid);

        FFmpeg.SetExecutablesPath("C:\\Program Files (x86)\\ffmpeg\\bin");

        Directory.CreateDirectory(Path.Combine(videoDir, hostIdFormatted));

        playlistBuilder.AddQuality("180");
        playlistBuilder.AddQuality("360");
        playlistBuilder.AddQuality("720");
    }

    public override async Task RunAsync()
    {
        Task retrieveSegmentsTask = Task.Run(StartRetreivingSegments);

        Task updatingPlaylistTask = Task.Run(StartSendingSegments);

        await Task.WhenAny(retrieveSegmentsTask, updatingPlaylistTask);
    }

    private string[] qualiyOptions = new string[3]
    {
            "180",
            "360",
            "720"
    };

    private void UpdateManifest()
    {
        string url1 = $"https://localhost:2020/live/{hostIdFormatted}/{qualiyOptions[0]}/playlist.m3u8";
        string url2 = $"https://localhost:2020/live/{hostIdFormatted}/{qualiyOptions[1]}/playlist.m3u8";
        string url3 = $"https://localhost:2020/live/{hostIdFormatted}/{qualiyOptions[2]}/playlist.m3u8";
        manifestBuilder.AddPlaylist(url1);
        manifestBuilder.AddPlaylist(url2);
        manifestBuilder.AddPlaylist(url3);

        Log($"user={host.username}. Sending the master manifest...");
        broadcastClient.PostMasterPlaylist(manifestBuilder.BuildString(), hostId);
    }

    bool repeat = true;
    bool needUpdateManifest = true;

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

            Thread.Sleep(100);
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
                    //Log($"user={host.username}. Sending {segment.FileName}. Already sent={segmentsSent} ...");
                    broadcastClient.PostSegmentAsync(segment, hostid).GetAwaiter().GetResult();

                    playlistBuilder.AddSegment(segment.Duration, path: segment.FileName);

                    if (segmentsSent % 5 == 0)
                    {
                        if (needUpdateManifest)
                        {
                            UpdateManifest();
                        }
                    }

                    Log($"user={host.username}. Updating thumbnail...");
                    Task.Run(() => UpdateThumbnail(segment.Path));
                }

                //Thread.Sleep(100);
            }

            Thread.Sleep(200);
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

            FFmpeg.Conversions.New()
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
        }
    }

    private void Log(string msg)
    {
        Console.WriteLine(msg);
    }

    int l = 0;
    /// <summary>
    /// Create and send preview from 0 second of the segment
    /// </summary>
    /// <param name="segmentFn">Segment's .ts video file path</param>
    private async Task UpdateThumbnail(string segmentFn)
    {
        string hostid = hostId.Replace("-", "");

        string output = Path.Combine(thumbnailsDir, hostid + $"____{l}.png");
        string input = segmentFn;

        if (!File.Exists(input))
        {
            throw new IOException($"Could find video sample. Add {videoFileName} to video folder in bin folder.");
        }

        //IConversion conversion = await
        //    FFmpeg.Conversions.FromSnippet.Snapshot(input, output, TimeSpan.FromSeconds(4));

        //IConversionResult result = await conversion.Start("-y");

        //if (result != null)
        //{
        //    await broadcastClient.PostThumbnail(thumbnailPath: output);
        //}

        StartGettingPreviewProcess(input, output, offset: 10);
        l += 5;
        await broadcastClient.PostThumbnail(thumbnailPath: output);
    }
    //ffmpeg -ss 15 -i ./1.mp4 -frames:v 1 -q:v 2 output2.jpg
    public bool StartGettingPreviewProcess(string input, string output, int offset)
    {
        Process process = new Process();

        process.StartInfo.WorkingDirectory = "C:\\Users\\Ivan\\Desktop\\sensorium\\NET Projects\\ASPNET\\NatureForYou\\backend\\DummyUser\\bin\\Debug\\net6.0\\";
        process.StartInfo.FileName = "C:\\Program Files (x86)\\ffmpeg\\bin\\ffmpeg.exe";
        process.StartInfo.Arguments = $"-ss -y {offset} -i {input} -frames:v 1 -q:v 2 {output}.jpg";
        process.StartInfo.CreateNoWindow = true;
        return process.Start();
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