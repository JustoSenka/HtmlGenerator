﻿using HtmlGenerator.Generator;
using HtmlGenerator.Utils;
using System.Linq;
using System.Text.RegularExpressions;

namespace HtmlGenerator.Tags
{
    public class IncludeTag : ITag
    {
        public string TagID => "Include";

        private readonly Regex k_IncludeClassTag = new Regex(@"<include class=""(.*)""/>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        public string Modify(PageGenerator PageGenerator, string html)
        {
            var includeTags = k_IncludeClassTag.Matches(html);
            foreach (var tag in includeTags.OrderByDescending(t => t.Index))
            {
                var pageID = tag.Groups[1].Value.NormalizePath();

                if (PageGenerator.Pages.ContainsKey(pageID))
                {
                    var replacement = PageGenerator.Pages[pageID].RenderedHtml;
                    html = html.Replace(tag.Index, tag.Length, replacement);
                }
                else
                {
                    Logger.LogError($"Class not found: {pageID}. Cannot Perform Include");
                }
            }

            return html.FixLineEndings();
        }
    }
}