using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Parus.Core.Interfaces;
using Newtonsoft.Json.Linq;

namespace Parus.Core.Services.Localization
{
	public class LocalizationService : ILocalizationService
	{
        private StreamReader stream;

		public void SetLocale(string locale)
        {
			string localeFn = default;
			switch (locale)
			{
				default:
				case "ru":
					localeFn = "ru.txt";
					break;
				case "en":
					localeFn = "en.txt";
					break;
			}

			string filePath = Path.Combine("localization", localeFn);

			stream = new StreamReader(filePath);
		}

		public string RetrievePhrase(string key)
        {
            string value = "";
			string? line;
			while ((line = stream.ReadLine()) != null)
			{
                int separatorIndex;
				string key1 = getKey(line, out separatorIndex);
				
                if (key == key1)
                {
                    value = line.Substring(separatorIndex);

					break;
                }
			}
			
			//var t = stream.ReadLine();
			stream.BaseStream.Position = 0;

			return value;
		}

		private string getKey(string line, out int separatorIndex)
		{
            StringBuilder keyChars = new StringBuilder();
            char c = default;
            int i = 0;
            while (c != '=')
            {
                c = line[i];
                i++;

                if (c != '=')
                {
                    keyChars.Append(c);
                }
			}

            separatorIndex = i++;

			return keyChars.ToString();
		}

		public void Dispose()
        {
            stream.Dispose();
            stream = null;
		}
    }
}