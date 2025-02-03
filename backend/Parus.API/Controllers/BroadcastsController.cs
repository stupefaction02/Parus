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

using Parus.Infrastructure.Identity;
using Parus.Infrastructure.DataLayer;
using Parus.Core;
using Parus.Core.Entities;
using Parus.Core.Interfaces;
using Parus.Core.Interfaces.Repositories;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Parus.API.Services;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace Parus.API.Controllers
{
	[ApiController] 
	public class BroadcastController : Controller
	{
		private ClaimsPrincipal user;

        public new ClaimsPrincipal User  
		{ 
			get 
			{ 
				return HttpContext != null ? HttpContext.User : user; 
			} 
			set 
			{ 
				user = value; 
			} 
		}

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

		public BroadcastController()
		{
		}

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

			return BitConverter.ToString(bytes).Replace("-", "").ToLower();
		}

		[HttpPost]
        [Route("api/broadcasts/start")]
		[AllowAnonymous]
        public async Task<IActionResult> StartBroadcast(
			[FromQuery] string title,
            [FromQuery] int catId,
            [FromQuery] int[] tags, 
			[FromServices] ParusDbContext context,
			[FromServices] BroadcastControl broadcastControl)
		{
            System.Security.Principal.IIdentity identity = User.Identity;

			if (!identity.IsAuthenticated)
			{
				return Unauthorized();
			}

            ParusUser user = context.Users
                .AsEnumerable().SingleOrDefault(x => x.GetUsername() == identity.Name);

            if (user == null || !user.EmailConfirmed)
			{
                return Unauthorized();
            }

            Console.WriteLine($"{user.UserName} is starting broadcast...");

            await broadcastControl.StartBroadcastAsync(catId, tags, title, user, context);

            return Json(new { broadcastKey = "" });
        }

        [Route("api/broadcasts/stop")]
        [HttpDelete]
        public async Task<IActionResult> StopBroadcast(
			[FromQuery] string broadcastKey,
			[FromServices] IBroadcastInfoRepository context,
            [FromServices] ParusDbContext identityDbContext,
            [FromServices] BroadcastControl broadcastControl)
		{
            System.Security.Principal.IIdentity identity = User.Identity;

            if (!identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            ParusUser user = identityDbContext.Users
                .AsEnumerable().SingleOrDefault(x => x.GetUsername() == identity.Name);

            if (user == null || !user.EmailConfirmed)
            {
                return Unauthorized();
            }

            Console.WriteLine($"{user.UserName} is stoping broadcast...");

            await broadcastControl.StopBroadcastAsync(user, context);

            // user must to have only one broadcast at the time

			return Ok();
        }

        [Benchmark(Description = "BroadcastController.SearchBroadcasts")]
        [Route("api/broadcasts/search")]
		[HttpGet]
		public async Task<IActionResult> SearchBroadcasts(string q,
		   [FromServices] IBroadcastInfoRepository context)
		{
			return Json( context.Search(query: q) );
		}
    }
}