using System;
using System.Collections.Generic;
using System.Text;

namespace Naturistic.Core.Interfaces
{
	public interface ILocalizationService
	{
		string RetrievePhrase(string key);
		void SetLocale(string localeFn);
	}
}
