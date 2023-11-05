using System;
using System.Collections.Generic;
using System.Text;

namespace Parus.Core.Interfaces
{
	public interface ILocalizationService
	{
		string RetrievePhrase(string key);
		void SetLocale(string localeFn);
	}
}
