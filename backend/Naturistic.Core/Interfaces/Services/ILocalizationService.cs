using System;
using System.Collections.Generic;
using System.Text;

namespace Naturistic.Core.Interfaces
{
	public interface ILocalizationService
	{
		string DictionaryFileName { get; set; }

		string RetrievePhrase(string key);
	}
}
