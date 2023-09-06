using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Naturistic.Core.Entities;
using Naturistic.Infrastructure.DLA;

namespace Naturistic.Backend.Controllers
{
	[ApiController]
	public class TestController : Controller
	{
		#region Testing

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

        [HttpGet]
        [Route("api/test/seed1")]
        public IActionResult Seed1([FromServices] ApplicationDbContext context)
        {
			foreach (var item in context.Broadcasts)
			{
				context.Remove(item);

				Console.WriteLine(item.Username + " deleted!");
			}

			context.SaveChanges();

            string[] names = new string[6]
			{
				"athleanx_com",
				"SuperIvan",
				"TwickyEwe",
				"Milky",
				"Fredrick_Hudsen",
				"ADC_Vr"
			};

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

			var broadcasts = new List<BroadcastInfo>();

            for (int i = 1; i < 1000; i++)
			{
                int cat = (new Random()).Next(1, 5);
                int tag = (new Random()).Next(1, 4);

                int nam = (new Random()).Next(0, 5);
                int tit = (new Random()).Next(0, 5);
                int pre = (new Random()).Next(0, 5);

				string name = names[nam] + Guid.NewGuid().ToString().Substring(0, 2);
				string title = titles[tit];
				string preview = previews[pre];

                var broadcast1 = new BroadcastInfo
                {
					Preview = preview,
                    Username = name,
                    Title = title,
                    AvatarPic = "ava1.jpg",
                    Category = context.Categories.SingleOrDefault(x => x.Id == cat),
                    Ref = "athleanxdotcommer14",
                    Tags = new List<Tag> { context.Tags.SingleOrDefault(x => x.Id == tag) }
                };

				broadcasts.Add(broadcast1);

                Console.WriteLine($"Broadcast info {name} created!");
            }

			context.Broadcasts.AddRange(broadcasts);

			context.SaveChanges();

			return Ok("Seeding is done!");
        }

        #endregion
    }
}