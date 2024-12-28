using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parus.Hsl.Services;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Parus.VideoEdge
{
    [ApiController]
	public class VideoController : Controller
	{
		private string videoStoreDir;

		public VideoController()
		{
			videoStoreDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "video_temp", "live");
		}

		[HttpGet]
		[Route("/hls/live/segments")]
		[Route("/hls/live/playlists")]
		public async Task<object> GetFile(string fn)
		{
			var headers = HttpContext.Response.Headers;
			headers.Add("Access-Control-Allow-Origin", "*");
			headers.Add("Transfer-Encoding", "chunked");
			
			// Adding this header causes download error(undefined file) on Mozilla 
			// headers.Add("Content-Encoding", "gzip");
			headers.Add("Cache-Control", "no-cache, no-store, private");
			headers.Add("Vary", "Accept-Encoding");

			Stream fs;
			var respStream = new MemoryStream();
			var fp = Path.Combine(videoStoreDir, fn);

			try
			{
				fs = System.IO.File.Open(fp, FileMode.Open, FileAccess.Read, FileShare.Read);
				await fs.CopyToAsync(respStream);

				fs.Close();
			}
			catch (FileNotFoundException)
            {
				Console.WriteLine($"file {fn} not found in live video store.");
				return NotFound();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error! {e.GetType().Name} {e.Message}");
				return StatusCode(500);
			}

			// "application/vnd.apple.mpegurl" - preferable ct, but it returns undefined file
			return new FileContentResult(respStream.ToArray(), "application/octet-stream");
		}

		/// <summary>
		/// Post file on a server, either video segment or manifest
		/// </summary>
		/// <param name="segmentFile"></param>
		/// <returns>200 code respond if file has been saved successfully, and 500 code if it hasn't</returns>
		[HttpPost]
		[Route("hls/live/segments")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> PostSegment([FromForm]IFormFile segmentFile)
		{
			if (segmentFile != null)
			{
				var fp = Path.Combine(videoStoreDir, segmentFile.FileName);
				var createdf = System.IO.File.Create(fp);
				await segmentFile.CopyToAsync(createdf);

				createdf.Close();

				Console.WriteLine($"File saved, name: {segmentFile.FileName}");

				return Ok();
			}

			return BadRequest("Provided file ...");
		}
		
		[HttpPost]
		[Route("hls/live/playlists")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> PostPlaylist([FromForm]IFormFile playlistFile)
		{
			if (playlistFile != null)
			{
				var fp = Path.Combine(videoStoreDir, playlistFile.FileName);
				var createdf = System.IO.File.Create(fp);
				await playlistFile.CopyToAsync(createdf);

				createdf.Close();

				Console.WriteLine($"File saved, name: {playlistFile.FileName}");

				return Ok();
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