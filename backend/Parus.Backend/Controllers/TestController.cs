using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BenchmarkDotNet.Loggers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Parus.Backend.Services;
using Parus.Core.Entities;
using Parus.Core.Identity;
using Parus.Core.Interfaces.Repositories;
using Parus.Infrastructure.DLA;
using Parus.Infrastructure.Extensions;
using Parus.Infrastructure.Identity;
using static Parus.Backend.Controllers.IdentityController;

namespace Parus.Backend.Controllers
{
	[ApiController]
	public class TestController : ParusController
	{
        #region Testing

        [HttpGet]
        [Route("api/hello")]
        public string HelloWorld()
        {
            return "Hello World!";
        }

        [HttpGet]
		[Authorize]
        [Route("api/test/auth")]
        public string Test()
		{
			return "Authorized!";
		}

        [HttpGet]
		[Route("api/test/seed")]
		public IActionResult Seed([FromServices] ApplicationDbContext context)
		{
			//Tag[] tags = new Tag[3]
			//{
			//    new Tag { Name = "Russia" },
			//    new Tag { Name = "Norway" },
			//    new Tag {  Name = "USA" }
			//};

			//Tags.AddRange(tags);

			//BroadcastCategory[] categories = new BroadcastCategory[4]
			//{
			//    new BroadcastCategory { Name = "Bird Watching" },
			//    new BroadcastCategory { Name = "Scenery" },
			//    new BroadcastCategory { Name = "Animals" },
			//    new BroadcastCategory { Name = "City" }
			//};

			//Categories.AddRange(categories);

			//context.SaveChanges();

			var broadcast1 = new Broadcast
			{
				Username = "athleanxdotcommer14",
				Title = "Gym, Connecticut",
				AvatarPic = "/athleanxdotcommer14.png",
				Category = context.Categories.SingleOrDefault(x => x.Id == 4),
				Ref = "/athleanxdotcommer14",
				Tags = new List<BroadcastTag> { context.Tags.SingleOrDefault(x => x.BroadcastTagId == 3) }
			};

			var broadcast2 = new Broadcast
			{
				Username = "broadcaster2",
				Title = "Those loevely birds!",
				AvatarPic = "/broadcaster2.png",
				Category = context.Categories.SingleOrDefault(x => x.Id == 1),
				Ref = "/broadcaster2",
				Tags = new List<BroadcastTag> { context.Tags.SingleOrDefault(x => x.BroadcastTagId == 2) }
			};

			var broadcast3 = new Broadcast
			{
				Username = "ivan25",
				Title = "Winter!",
				AvatarPic = "/ivan25.png",
				Category = context.Categories.SingleOrDefault(x => x.Id == 2),
				Ref = "/ivan25",
				Tags = new List<BroadcastTag> { context.Tags.SingleOrDefault(x => x.BroadcastTagId == 1) }
			};

			Broadcast[] broadcast = new Broadcast[3]
			{
				broadcast1,
				broadcast2,
				broadcast3,
			};

			context.Broadcasts.AddRange(broadcast);

			context.SaveChanges();

			return Ok("Seeding is done!");
		}

        [HttpDelete]
        [Route("api/test/purgebroadcasts")]
        public IActionResult Purge([FromServices] ParusDbContext context)
		{
            DeleteAllBroadcasts(context);

			return Ok();
        }

        [HttpGet]
        [Route("api/test/test1")]
		public object Test1()
		{
			return "Test";
		}

        [HttpGet]
        [Route("api/hellojson")]
        public async Task<object> HelloJson()
        {
            return Json(new { success = "true", message = "Hello World!" });
        }

        [HttpGet]
        [Route("api/test/hellojson")]
        public async Task<object> HelloJson1([FromServices] ILogger<TestController> logger)
        {
			logger.Log(LogLevel.Information, "Boobies!");
            return Json(new { success = "true", message = "Hello World!" });
        }

        [HttpGet]
        [Route("api/test/seed1")]
        public IActionResult Seed1([FromServices] ParusDbContext context)
        {
			DeleteAllBroadcasts(context);

			context.SaveChanges();

            string[] names = new string[12]
			{
				"athleanx_com",
				"SuperIvan",
				"TwickyEwe",
				"Milky",
				"Fredrick_Hudsen",
				"ADC_Vr",
				"Oleg Kashin",
				"Michael Svetoff",
				"Putin",
				"Lomonosoff",
				"Van Darkholme",
				"Gay Website"
			};

            string[] titles = new string[20]
            {
                "Watching Birds",
                "Birds, Birds, Birds",
                "Cats and Dogs",
                "Good morning, I'm back",
                "Hello",
                "OMG THIS IS INSANE",
				"Look at those creatures",
				"///0_0/////",
				"AAAAAAAAAAAAAAAAaahhhh",
				"Watching animals",
				"Oh my god",
				"-_-",
				"ayyyy lmaoo",
				"Bratishkaaa",
				"Golubaya luna",
				"Goluuuubaya",
				"5:5",
				"Watching 0_0",
				"Batynya batyna batyna cumbutt",
				"P-P-P-P-P Z-z-z-z-z-z-z"
            };

			string[] previews = new string[6]
			{
				"preview1.png",
                "preview2.png",
                "preview3.png",
                "preview4.png",
                "preview5.png",
                "preview6.png"
            };

			var broadcasts = new List<Broadcast>();
			var keywords = new List<BroadcastInfoKeyword>();

			for (int i = 1; i < 15000; i++)
			{
				int cat = (new Random()).Next(1, 5);
				int tag = (new Random()).Next(2, 4);

				int nam = (new Random()).Next(0, 12);
				int tit = (new Random()).Next(0, 20);
				int pre = (new Random()).Next(0, 6);
				int a = (new Random()).Next(1, 3);

				string name = names[nam] + Guid.NewGuid().ToString().Substring(0, 2);
				string title = titles[tit];
				string preview = previews[pre];

				var tag1 = context.Tags.SingleOrDefault(x => x.BroadcastTagId == 1);
				var tag2 = context.Tags.SingleOrDefault(x => x.BroadcastTagId == tag);
				var cat1 = context.Categories.SingleOrDefault(x => x.Id == cat);

                var broadcast1 = new Broadcast
				{
					Preview = preview,
					Username = name,
					Title = title,
					AvatarPic = $"ava{a}.png",
					Category = cat1,
					Ref = "athleanxdotcommer14",
					Tags = new List<BroadcastTag> { tag1, tag2 }
				};

				broadcasts.Add(broadcast1);

                Console.WriteLine($"Broadcast info {name} created!");
            }

            context.BroadcastsKeywords.AddRange(keywords);

			if (broadcasts != null)
			{
				context.Broadcasts.AddRange(broadcasts);
			}

			context.SaveChanges();

			return Ok("Seeding is done!");
        }

        private void DeleteAllBroadcasts(ParusDbContext context)
        {
            foreach (var item in context.Broadcasts)
            {
                context.Remove(item);

                Console.WriteLine(item.Username + " deleted!");
            }

			context.SaveChanges();
        }

		[AllowAnonymous]
        [HttpDelete]
        [Route("api/test/purgeallbroadcasts")]
        public async Task<IActionResult> PurgeBroadcasts([FromServices] IUserRepository users, [FromServices] UserManager<ParusUser> um, [FromServices] ParusDbContext dbContext)
		{
            ParusUser usr = users.One(x => x.GetUsername() == User.Identity.Name) as ParusUser;

			if (usr == null)
			{
				Console.WriteLine("Authtoken is not valid!");
				return Unauthorized();
			}

            if (await um.IsInRoleAsync(usr, "admin"))
			{
				DeleteAllBroadcasts(dbContext);

				return Ok();
			}

			return Unauthorized();
		}

        [HttpGet]
        [Route("api/test/grantrole")]
        public object GrantAdminRoles(string username, string role, [FromServices] IUserRepository users, [FromServices] UserManager<ParusUser> um)
        {
			ParusUser usr = users.One(x => x.GetUsername() == username) as ParusUser;

			if (usr != null)
			{
				um.AddToRoleAsync(usr, role);

				return new { success = true };
			}

            return new { success = false };
        }

        [HttpGet]
        [Route("api/test/createbroadcastes")]
        public async Task<IActionResult> Seed2([FromServices] ParusDbContext dbContext, 
			[FromServices] BroadcastControl broadcastControl)
		{
			DeleteAllBroadcasts(dbContext);

            //dbContext.Database.CloseConnection();

            //return default;
            string[] titles = new string[10]
            {
                "Collab? 💖 BOYFU VIBES AND HIP SWAY 💖   !gg !bodypillow",
                "🦐FIRST TIME INSCRYPTION!!! LEGGOOO! ❤️BLOWHOLE BLAST RESTOCKED -> !GG / !Merch 《VTuber》!socials",
                "Weekly Dev Stream! | !merch",
                "SPOOKTEMBER DAY 4: I'M BACK! DRAMA + SPOOKS + GAMING | !TTS !advgg | !figure |",
                "Yay a new update in Genshin!!",
                "✨ ☄️ ⋆ Cozy Monday Zatsudan! ⋆ ☄️ ✨ !merch !discord !socials !box !mousepad", 
				"Title #1",
                "Title #2",
                "My stream",
				"Cool stream"
            };

            string[] previews = new string[6]
            {
                "preview1.jpg",
                "preview2.jpg",
                "preview3.jpg",
                "preview4.jpg",
                "preview5.jpg",
                "preview6.jpg"
            };

			var users = dbContext.Users.ToList();

            foreach (ParusUser user in users)
			{
				int cat = (new Random()).Next(1, 5);
				int tag = (new Random()).Next(2, 4);

				int tit = (new Random()).Next(0, 5);
				int pre = (new Random()).Next(0, 5);

				string title = titles[tit];

				await broadcastControl.StartBroadcastAsync(1, new int[] { 1 }, title, user, dbContext);

                Console.WriteLine($"{user.UserName} has started a new broadcast!");
            }

			dbContext.SaveChanges();

            return Ok();
        }

        #endregion

        [HttpGet]
        [Route("api/test/generaterefreshtoken")]
        public async Task<object> GenerateRefreshToken(string username, string fingerPrint, [FromServices] ParusDbContext identityDbContext)
		{
            ParusUser user = await identityDbContext.Users
                .FirstOrDefaultAsync(x => x.UserName == username);

            string fp = String.IsNullOrEmpty(fingerPrint) ? HttpContext.Request.Headers.UserAgent : fingerPrint;
            RefreshSession refreshSession = RefreshSession.CreateDefault(fp, user);

            await identityDbContext.RefreshSessions.AddAsync(refreshSession);

			await identityDbContext.SaveChangesAsync();

            return new
            {
                success = true,
                refresh_token = new { token = refreshSession.Token, expires = refreshSession.ExpiresAt }
            };
        }

        [HttpGet]
        [Route("api/test/addsample01")]
        public async Task<object> AddSample01([FromServices] BroadcastControl broadcastControl, 
			[FromServices] ParusDbContext parusDbIndex)
		{
            ParusUser user = await parusDbIndex.Users
				.FirstOrDefaultAsync(x => x.UserName == "broadcasteroff");

            await broadcastControl.StartBroadcastAsync(1, new int[] { 1 }, "Broadcasteroff is alive!", user, parusDbIndex);

            return new
            {
                success = true
            };
        }

		[HttpGet]
		[Route("api/test/registerdeveloperuser")]
		public async Task<object> RegisterDevelopers([FromServices] ParusUserIdentityService identityService,
			[FromServices] ParusDbContext users)
		{
            var random = new Random();
            var username = "deveoper_" + random.Next(0, 10000) + "_" + Guid.NewGuid().ToString().Substring(0, 3);
            var usr = new ParusUserRegistrationJsonDTO
            {
                Username = username,
                Email = $"{username}@mail.ru",
                Password = "zx1",
                Gender = Gender.Male
            };

            var result = await identityService.RegisterAsync(usr, new string[1] { "Developer" });

			foreach (var user in result.RegisteredUsers)
			{
				user.EmailConfirmed = true;
				users.Update(user);
            }

			await users.SaveChangesAsync();

            return Json(result.JsonResponse);
        }

        [HttpGet]
        [Route("api/test/registeruser")]
        public async Task<object> RegisterUser(string username, 
			[FromServices] ParusUserIdentityService identityService,
            [FromServices] ParusDbContext users)
        {
            var usr = new ParusUserRegistrationJsonDTO
            {
                Username = username,
                Email = $"{username}@mail.ru",
                Password = "zx1",
                Gender = Gender.Male
            };

            var result = await identityService.RegisterAsync(usr);

			//if (resultюЫ)

            foreach (var user in result.RegisteredUsers)
            {
                user.EmailConfirmed = true;
                users.Update(user);
            }

            await users.SaveChangesAsync();

            return Json(result.JsonResponse);
        }

        /// <summary>
        /// Get special users used on pluto server, e.g. imitate highload on chat module
        /// </summary>
        /// <param name="userRepository"></param>
        /// <returns></returns>
        [HttpGet]
		[Route("api/test/seedplutousers")]
		public async Task<object> SeedPlutoUsers([FromServices] ParusUserIdentityService identityService)
		{
			var random = new Random();
			int count = 0;
			for (int i = 0; i < 10; i++)
			{
				var username = "PlutoUser_" + random.Next(0, 10000) + "_" + Guid.NewGuid().ToString().Substring(0, 3);

                var usr = new ParusUserRegistrationJsonDTO
				{
					Username = username,
					Email = $"{username}@mail.ru",
					Password = "zx1",
					Gender = Gender.Male
                };

                var result = await identityService.RegisterAsync(usr, new string[1] { "TestUsers.Pluto" });

				if (result.StatusCode == (int)HttpStatusCode.OK && result.RegisteredUsers != null)
				{
                    count++;

                    Console.WriteLine($"SeedPlutoUsers: registered user {result.RegisteredUsers.ToArray()[0].UserName}");
				}
            }

            return new { success = "true", users = count };
		}

		/// <summary>
		/// Get special users used on pluto server, e.g. imitate highload on chat module
		/// </summary>
		/// <param name="userRepository"></param>
		/// <returns></returns>
        [HttpGet]
        [Route("api/test/plutousers")]
        public async Task<object> PlutoUsers(
			[FromServices] IOptions<JwtAuthOptions> authOptions,
            [FromServices] UserManager<ParusUser> userManager)
        {
			var plutoUsers = await userManager.GetUsersInRoleAsync("TestUsers.Pluto");

			var plutoUsersRet = plutoUsers.Select(x => {
                ParusUser usr = (ParusUser)x;

                return new
                {
                    id = usr.Id,
                    username = usr.UserName,
                    email = usr.Email,
                    emailConfirmed = usr.EmailConfirmed,
                    jwt = usr.JwtTokenFromUser(authOptions.Value)
                };
            });

            return new { success = "true", count = plutoUsers.Count, users = plutoUsersRet };
        }
    }
}