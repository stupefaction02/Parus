using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Naturistic.Core.Interfaces;

namespace Naturistic.WebUI.HtmlHelpers
{
    public static class LocalizationHelper
    {
        public static HtmlString RetreiveErrorParagraph(this IHtmlHelper html, string key,
            [FromServices] ILocalizationService localization)
        {
            string word = localization.RetrievePhrase(key);

            string str = "<p class=\"error_text\">" + word + "</p>";

            return new HtmlString(str);
        }
    }
}