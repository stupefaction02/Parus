using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Parus.Backend.Services;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using Parus.Infrastructure.DLA;
using Parus.Infrastructure.Identity;

namespace Parus.Backend.Controllers
{
	[ApiController]
	public class TestController : Controller
	{
        #region Testing

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

			var broadcast1 = new BroadcastInfo
			{
				Username = "athleanxdotcommer14",
				Title = "Gym, Connecticut",
				AvatarPic = "/athleanxdotcommer14.png",
				Category = context.Categories.SingleOrDefault(x => x.Id == 4),
				Ref = "/athleanxdotcommer14",
				Tags = new List<Tag> { context.Tags.SingleOrDefault(x => x.Id == 3) }
			};

			var broadcast2 = new BroadcastInfo
			{
				Username = "broadcaster2",
				Title = "Those loevely birds!",
				AvatarPic = "/broadcaster2.png",
				Category = context.Categories.SingleOrDefault(x => x.Id == 1),
				Ref = "/broadcaster2",
				Tags = new List<Tag> { context.Tags.SingleOrDefault(x => x.Id == 2) }
			};

			var broadcast3 = new BroadcastInfo
			{
				Username = "ivan25",
				Title = "Winter!",
				AvatarPic = "/ivan25.png",
				Category = context.Categories.SingleOrDefault(x => x.Id == 2),
				Ref = "/ivan25",
				Tags = new List<Tag> { context.Tags.SingleOrDefault(x => x.Id == 1) }
			};

			BroadcastInfo[] broadcast = new BroadcastInfo[3]
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
        public IActionResult Purge([FromServices] ApplicationDbContext context)
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
        [Route("api/hello")]
        public object Hello()
        {
            return "Hello World!";
        }

        [HttpGet]
        [Route("api/test/seed1")]
        public IActionResult Seed1([FromServices] ApplicationDbContext context)
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

			var broadcasts = new List<BroadcastInfo>();
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

				var tag1 = context.Tags.SingleOrDefault(x => x.Id == 1);
				var tag2 = context.Tags.SingleOrDefault(x => x.Id == tag);
				var cat1 = context.Categories.SingleOrDefault(x => x.Id == cat);

                var broadcast1 = new BroadcastInfo
				{
					Preview = preview,
					Username = name,
					Title = title,
					AvatarPic = $"ava{a}.png",
					Category = cat1,
					Ref = "athleanxdotcommer14",
					Tags = new List<Tag> { tag1, tag2 }
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

        private void DeleteAllBroadcasts(ApplicationDbContext context)
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
        public async Task<IActionResult> PurgeBroadcasts([FromServices] IUserRepository users, [FromServices] UserManager<ApplicationUser> um, [FromServices] ApplicationDbContext dbContext)
		{
            ApplicationUser usr = users.One(x => x.GetUsername() == User.Identity.Name) as ApplicationUser;

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
        public IActionResult GrantAdminRoles(string username, string role, [FromServices] IUserRepository users, [FromServices] UserManager<ApplicationUser> um)
        {
			ApplicationUser usr = users.One(x => x.GetUsername() == username) as ApplicationUser;

			if (usr != null)
			{
				um.AddToRoleAsync(usr, role);
			}

            return null;    
        }

        [HttpGet]
        [Route("api/test/createbroadcastes")]
        public IActionResult Seed2([FromServices] ApplicationDbContext dbContext, 
			[FromServices] ApplicationIdentityDbContext identityDbContext,
			[FromServices] BroadcastControl broadcastControl)
		{
			DeleteAllBroadcasts(dbContext);
			
            string[] titles = new string[6]
            {
                "Collab? 💖 BOYFU VIBES AND HIP SWAY 💖   !gg !bodypillow",
                "🦐FIRST TIME INSCRYPTION!!! LEGGOOO! ❤️BLOWHOLE BLAST RESTOCKED -> !GG / !Merch 《VTuber》!socials",
                "Weekly Dev Stream! | !merch",
                "SPOOKTEMBER DAY 4: I'M BACK! DRAMA + SPOOKS + GAMING | !TTS !advgg | !figure |",
                "Yay a new update in Genshin!!",
                "✨ ☄️ ⋆ Cozy Monday Zatsudan! ⋆ ☄️ ✨ !merch !discord !socials !box !mousepad"
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

			foreach (ApplicationUser user in identityDbContext.Users)
			{
				int cat = (new Random()).Next(1, 5);
				int tag = (new Random()).Next(2, 4);

				int tit = (new Random()).Next(0, 5);
				int pre = (new Random()).Next(0, 5);

				string title = titles[tit];

				broadcastControl.StartBroadcastAsync(cat, new int[] { 1, tag }, title, user, dbContext);

                Console.WriteLine($"{user.UserName} has started a new broadcast!");
            }

			dbContext.SaveChanges();

            return Ok();
        }

        #endregion
    }
}