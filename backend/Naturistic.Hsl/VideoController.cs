using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Naturistic.Hsl.Services;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Naturistic.Hsl
{
    [ApiController]
	public class VideoController : Controller
	{
		private string videoStoreDir;

		public VideoController()
		{
			videoStoreDir = Path.Combine("video_temp", "live");
		}

		[HttpGet]
		[Route("/hls/live/segment")]
		// TODO: Replace method below to a middleware that intercepts any file access requests
		public async Task<object> GetVideoSegment(string segmentFileName)
		{
			var headers = HttpContext.Response.Headers;
			headers.Add("Accept-Ranges", "bytes");
			headers.Add("Access-Control-Allow-Origin", "*");
			// Since we use .ts or mp4 extension let's use this mime-type
			// But if we would use mpeg type, we can set to video/mpeg 
			headers.Add("Transfer-Encoding", "chunked");

			Stream file;
			var fp = Path.Combine(videoStoreDir, segmentFileName);

			try
			{
				file = System.IO.File.Open(fp, FileMode.Open);
			}
			catch (Exception)
            {
				return NotFound();
            }

			// TODO: use Application.Octet
			return new FileStreamResult(file, "application/octet-stream");
		}

		[HttpGet]
		[Route("/hls/live/playlist")]
		public async Task<object> GetManifest(string manifestFile)
		{
			var headers = HttpContext.Response.Headers;
			headers.Add("Access-Control-Allow-Origin", "*");
			headers.Add("Transfer-Encoding", "chunked");

			// Adding this header causes download error(undefined file) on Mozilla 
			// headers.Add("Content-Encoding", "gzip");
			headers.Add("Cache-Control", "no-cache, no-store, private");
			headers.Add("Vary", "Accept-Encoding");

			Stream file;
			var respStream = new MemoryStream();
			var fp = Path.Combine(videoStoreDir, manifestFile);

			try
			{
				file = System.IO.File.Open(fp, FileMode.Open);
				await file.CopyToAsync(respStream);

				System.IO.File.Delete(fp);
			}
			catch (Exception)
			{
				return NotFound();
			}

			// "application/vnd.apple.mpegurl" - preferable ct, but it returns undefined file
			return new FileStreamResult(file, "application/octet-stream");
		}

		/// <summary>
		/// Post file on a server, either video segment or manifest
		/// </summary>
		/// <param name="file"></param>
		/// <returns>200 code respond if file has been saved successfully, and 500 code if it hasn't</returns>
		[HttpPost]
		[Route("hls/live/files")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> PostFile([FromForm]IFormFile file)
		{
			if (file != null)
			{
				var fp = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", videoStoreDir, file.FileName);
				var createdf = System.IO.File.Create(fp);
				await file.CopyToAsync(createdf);

				Console.WriteLine($"File saved, name: {file.FileName}");
			}

			return BadRequest("Provided file ...");
		}

		private void LogHeaders()
        {
            foreach (var t in Request.Headers)
				Console.WriteLine($"{t.Key}: {t.Value}");
		}
	} 
}