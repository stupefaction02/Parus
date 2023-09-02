using System.Collections.Generic;
using System.Linq;
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

		#endregion
	}
}