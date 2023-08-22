using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Naturistic.Core.Interfaces;

namespace Naturistic.WebUI.HtmlHelpers
{
    public static class LocalizationHelper
    {
        private readonly static string span = "<span class=\"placeholder\"></span>";
        private readonly static string pattern = @"\{([^)]*)\}";
        public static string PlaceholderToHtml(string text)
        {
            int matches = Regex.Count(text, pattern);

            if (matches > 0)
            {
                return Regex.Replace(text, pattern, span);
            }

            return string.Empty;
        }

        public static HtmlString RetreiveErrorParagraph(this IHtmlHelper html, string key,
            [FromServices] ILocalizationService localization)
        {
            string prhase = localization.RetrievePhrase(key);

            string str = "<p class=\"error_text\">" + prhase + "</p>";

            return new HtmlString(str);
        }

        public static HtmlString RetreiveRawText(this IHtmlHelper html, string key,
            [FromServices] ILocalizationService localization)
        {
            string prhase = localization.RetrievePhrase(key);

            string tagReplaced = PlaceholderToHtml(prhase);

            return string.IsNullOrEmpty(tagReplaced) ? new HtmlString(prhase) : new HtmlString(tagReplaced);
        }
    }
}