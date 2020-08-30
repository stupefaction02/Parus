using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using Microsoft.AspNet.Identity.EntityFramework;

using Naturistic.Infrastructure.Identity;
using Naturistic.Infrastructure.DLA;
using Naturistic.Core;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces;
using Naturistic.Core.Interfaces.Repositories;
using System.Security.Cryptography;

namespace Naturistic.Backend.Controllers
{
	[ApiController] 
	public class BroadcastController : Controller
	{
		private readonly IBroadcastRepository broadcasts;
	
		private struct BroadcastSessionInfo
        {
			public string SessionKey;

			public DateTime StreamingStart;
        }

		/// <summary>
		/// Yes.
		/// </summary>
		/// TODO: Move it to a repository
		private static Dictionary<int, BroadcastSessionInfo> broadcastsSessionStore 
			= new Dictionary<int, BroadcastSessionInfo>();

		public BroadcastController(IBroadcastRepository broadcasts)
		{
			this.broadcasts = broadcasts;
		}

        [HttpGet]
        [Route("api/broadcasts")]
        public async Task<object> GetBroadcastsInfo(int limit) => await broadcasts.GetAllAsync();

        [HttpGet]
		[Route("api/broadcasts/startSession")]
		public IActionResult StartChannelSession(int channelId)
        {  
			string chsk = generateRandomHash();
			var info = new BroadcastSessionInfo { SessionKey = chsk, StreamingStart = DateTime.Now };
			if (broadcastsSessionStore.ContainsKey(channelId))
				broadcastsSessionStore[channelId] = info;
			else
				broadcastsSessionStore.Add(channelId, info);

            #region Logging

            Console.WriteLine($"Start broadcast on a channel with id = {channelId}");
			Console.WriteLine("Current session store:");

			foreach (var ch in broadcastsSessionStore)
				Console.WriteLine($"Channel id: {ch.Key} session key: {ch.Value.SessionKey}");

			Console.WriteLine("/*_____________----_____________*/");

            #endregion
			
            return Ok(new { channelSessionKey = chsk });
        }

		[HttpGet]
		[Route("api/broadcasts/endSession")]
		public IActionResult EndChannelSession(int channelId)
        {

			return Ok();
        }

		[HttpGet]
		[Route("api/broadcasts/sessions")]
		public IActionResult GetSession(int channelId)
        {
			if (channelId > 0)
			{
				BroadcastSessionInfo info;
				if (!broadcastsSessionStore.TryGetValue(channelId, out info))
					return NotFound();

				#region Logging

				Console.WriteLine($"Touched one channel with name = {channelId} at {DateTime.Now}");
				Console.WriteLine("Current session store:");

				foreach (var ch in broadcastsSessionStore)
					Console.Write($"Channel id: {ch.Key} session key: {ch.Value} ");

				Console.WriteLine("/---------------------------------/");

				#endregion

				return Ok(new { channelSessionKey = info.SessionKey });
			}

			return BadRequest();
        }

		private string generateRandomHash()
        {
			var bytes = new byte[8];
			using (var rng = new RNGCryptoServiceProvider())
				rng.GetBytes(bytes);

			string hash2;
			return hash2 = BitConverter.ToString(bytes).Replace("-", "").ToLower();
		}
	}
}