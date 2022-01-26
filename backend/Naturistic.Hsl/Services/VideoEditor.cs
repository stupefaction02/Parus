using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Naturistic.Hsl.Services
{
    public class VideoService
    {
        private string ffmpegFn;

        private ProcessStartInfo defaultStartInfo = new ProcessStartInfo
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        public VideoService()
        {
            var ffmpegFn = new FileInfo(@"ffmpeg/ffmpeg.exe").FullName;

            // add tryc
            defaultStartInfo.FileName = ffmpegFn;
        }

        public FileInfo CutVideo(string input, TimeSpan from, TimeSpan length)
        {
            var startt = from.ToString("hh:mm:ss");
            var lengtht = length.ToString("hh:mm:ss");
            var inputfn = new FileInfo(input).FullName;
            var outputfn = "cut";

            defaultStartInfo.Arguments = $"-i {inputfn} -ss {startt} -t {lengtht} -async 1 {outputfn}.mp4";

            using (var ffmpegProcess = new Process() { StartInfo = defaultStartInfo })
            {

            }
         
            return null;
        }
    }
}
