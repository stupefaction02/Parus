using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Parus.Hsl
{
    /// <summary>
    /// 
    /// </summary>
    /// TODO: Compare storing file streams vs storing files paths in <see cref="segmentStore"/>
    public class ChannelStore
    {
        /// <summary>
        /// Rooted directory of video segments
        /// </summary>
        private string cldn;

        /// <summary>
        /// Directory contains video segments specified to live streaming 
        /// </summary>
        private string videoStoreDir;

        /// <summary>
        /// Directory contains video segments of <see cref="ChannelStore"></see> identified by its <see cref="id"/>
        /// </summary>
        private string store;

        private Queue<string> segmentStore;

		private string manifest;

        private readonly int id;

        private FileSystemWatcher fileWatcher;

        public string Manifest => manifest;

        public ChannelStore(int channelId)
        {
            segmentStore = new Queue<string>();
            this.id = channelId;

            cldn = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "video_temp");
            videoStoreDir = Path.Combine(cldn, "live");
            store = Path.Combine(videoStoreDir, channelId.ToString());

			Console.WriteLine($"Channel directory: {store}");
			manifest = Directory.GetFiles(store).First(f => Path.GetExtension(f) == ".m3u8");
			Console.WriteLine(manifest);

            FlushAlreadyExisted();

            fileWatcher = new FileSystemWatcher(videoStoreDir);
            fileWatcher.Created += FileCreated;
        }

        private void FileCreated(object source, FileSystemEventArgs e)  
            => segmentStore.Enqueue(e.FullPath); 

        private void FlushAlreadyExisted()
        {
            foreach (var file in Directory.GetFiles(store))
                segmentStore.Enqueue(file);
        }

        private byte[] FileToBytes(string fp)
        {
            var fs = File.Open(fp, FileMode.Open);
            var l = fs.Length;

            // frame bytes
            byte[] frmb = new byte[l];
            fs.Read(frmb, 0, Convert.ToInt32(l));

            return frmb;
        }

        /// <summary>
        /// Retrieve last second segment from video source
        /// </summary>
        /// <returns></returns>
        public byte[] GetVideoSegmentBytes()
        {
            if (segmentStore.Any())
                return FileToBytes(segmentStore.Dequeue());

            while (!segmentStore.Any())
            {
                if (segmentStore.Any())
                    return FileToBytes(segmentStore.Dequeue());

                Thread.Sleep(50);
            }

            return null;
        }

        /// <summary>
        /// Retrieve last second segment from video source
        /// </summary>
        /// <returns></returns>
        public Stream GetVideoSegmentFile()
        {
            if (segmentStore.Any())
            {
                var stream = new MemoryStream();
                var file = segmentStore.Dequeue();

                var fs = File.Open(file, FileMode.Open);//.CopyTo(stream);
            //    File.Delete(file);

				Console.WriteLine($"Retreiving file: {file}");

                return fs;
            }

            while (!segmentStore.Any() && false)
            {
                if (segmentStore.Any())
                    return File.Open(segmentStore.Dequeue(), FileMode.Open);

                Thread.Sleep(50);
            }

            FlushAlreadyExisted();

            return GetVideoSegmentFile();
        }

		private Stream GetPlaylistFile() => File.Open(manifest, FileMode.Open);

        public async Task<byte[]> GetVideoSegmentBytesAsync()
                => await Task.Run(() => GetVideoSegmentBytes());

        public async Task<Stream> GetVideoSegmentFilesAsync()
                => await Task.Run(() => GetVideoSegmentFile());
				
	    public async Task<Stream> GetPlaylistFileStreamAsync() 
				=> await Task.Run(() => GetPlaylistFile()); 
    }
}