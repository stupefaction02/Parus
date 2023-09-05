using System.IO;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naturistic.Core.Interfaces;

namespace Naturistic.WebUI.HtmlHelpers
{
    public static class PaginatorHelper
    {
        private const int pageWindow = 9;
        private const int pagesMax = 11;

        public static HtmlString RegularPagination(int page, int pageCount, string href)
        {
            TagBuilder pagination = new TagBuilder("div");
            
            if (pageCount <= pagesMax)
            {
                for (int i = 1; i <= pageCount; i++)
                {
                    TagBuilder a = new TagBuilder("a");
                    a.InnerHtml.Append(i.ToString());

                    if (i == page)
                    {
                        a.AddCssClass("active");
                    }

                    a.Attributes["href"] = "?page=" + i;

                    pagination.InnerHtml.AppendHtml(a);
                }
            }
            else
            {
                int leftVerge = page - (pageWindow / 2);
                int rightVerge = page + (pageWindow / 2);

                int start;
                if (leftVerge > 1)
                {
                    start = leftVerge;
                }
                else
                {
                    start = 1;
                }

                int end;
                if (rightVerge < pageCount)
                {
                    end = rightVerge;
                }
                else
                {
                    end = rightVerge + (pagesMax - rightVerge);
                }

                for (int i = start; i <= end; i++)
                {
                    TagBuilder a = new TagBuilder("a");
                    a.InnerHtml.Append(i.ToString());

                    if (i == page)
                    {
                        a.AddCssClass("active");
                    }

                    a.Attributes["href"] = "?page=" + i;

                    pagination.InnerHtml.AppendHtml(a);
                }
            }

            System.IO.StringWriter writer = new System.IO.StringWriter();

            pagination.WriteTo(writer, HtmlEncoder.Default);

            return new HtmlString(writer.ToString());
        }
    }
}