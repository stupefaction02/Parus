using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Naturistic.Core.Interfaces;

namespace Naturistic.Core.Services.Localization
{
	public class LocalizationService : ILocalizationService
	{
        private StreamReader stream;

        private string dictionaryFileName;
        public string DictionaryFileName 
        {
            get => dictionaryFileName;
            set
            {
                if (dictionaryFileName == value) return;

                dictionaryFileName = value;

				string filePath = Path.Combine("localization", value);

				stream = new StreamReader(filePath);
			}
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