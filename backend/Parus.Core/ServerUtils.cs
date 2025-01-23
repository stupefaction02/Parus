using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Parus.Common
{
    public static class ServerUtils
    {
		public static Stream ToStream(this string str)
		{
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(str);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}

		public static async Task<T> ConvertResponseStream<T>(HttpResponseMessage response)
        {
			string jsonString;
			using (var inputStream = new StreamReader(await response.Content.ReadAsStreamAsync()))
			{
				jsonString = await inputStream.ReadToEndAsync();
			}

			return JsonSerializer.Deserialize<T>(jsonString);
		}

		public static T ConvertJsonResult<T>(object registerUser)
		{
			return default;
		}
	}
}
