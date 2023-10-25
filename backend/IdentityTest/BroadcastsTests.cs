
using System.Text.Json;
using System.Text;
using Cassandra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Naturistic.Backend.Controllers;
using Naturistic.Core;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Core.Interfaces.Services;
using Naturistic.Infrastructure.DLA.Repositories;
using Naturistic.Infrastructure.Identity;
using Newtonsoft.Json.Linq;
using Xunit;

namespace IdentityTest
{
	public class BroadcastsTests
	{
		readonly IServiceProvider services =
			Naturistic.Backend.Program.CreateHostBuilder(new string[] { }).Build().Services;

		private T GetService<T>()
		{
			return services.GetRequiredService<T>();
		}

		[Fact]
		public async Task Search_Broadcast_Simple()
		{
			IBroadcastInfoRepository br = GetService<IBroadcastInfoRepository>();
            BroadcastController bc = BroadcastController;

			string input = "Aboba";

			JsonResult r = (JsonResult)await bc.SearchBroadcasts(input, br);

            SearchResponse jsonResult;
            using (MemoryStream ms = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(ms, r.Value, typeof(object),
                    new JsonSerializerOptions() { });
                var jsonResulltString = Encoding.UTF8.GetString(ms.ToArray());
                jsonResult = JsonSerializer.Deserialize<SearchResponse>(jsonResulltString);
            }

            Assert.True(jsonResult != null);
            //Assert.True(jsonResult.Success == "Y");
        }

		public class SearchResponse
		{
            public string MyProperty { get; set; }
        }

		BroadcastController bc;
		private BroadcastController BroadcastController
		{
			get
			{
				return bc == null ? bc = new BroadcastController() : bc;
			}
		}
	}
}