using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Parus.Common.Utils
{
    public static class JsonUtils
    {
        static Regex regex = new Regex("\"_source\": (.*)", RegexOptions.IgnoreCase);
        /// <summary>
        /// Format JSON 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static string FormatElasticSearchResultJson(string json)
        {
            var d = regex.Matches(json);
            if (d != null)
            {
                
            }

            return json;
        }
    }
}
