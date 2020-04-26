using HtmlGenerator.Generator;
using HtmlGenerator.Utils;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace HtmlGenerator.Tags
{
    public class SurroundTag : ITag
    {
        protected readonly Regex k_SurroundBeginTag = new Regex(@"<surround-begin class=""(.*)""/>");
        protected readonly Regex k_SurroundEndTag = new Regex(@"<surround-end class=""(.*)""/>");
        protected readonly string k_RenderSectionTag = @"<surround-content/>";

        public string TagID => "Surround";

        private const string ClassNotFoundError = "Class not found: {0}. Cannot Perform Surround";

        public string Modify(PageGenerator PageGenerator, string html)
        {
            // Begin
            foreach (var tag in k_SurroundBeginTag.Matches(html).OrderByDescending(t => t.Index))
            {
                var pageID = tag.Groups[1].Value.NormalizePath();

                if (PageGenerator.Pages.ContainsKey(pageID))
                {
                    var replacement = PageGenerator.Pages[pageID].RenderedHtml;

                    var index = replacement.IndexOf(k_RenderSectionTag, StringComparison.InvariantCultureIgnoreCase);
                    replacement = replacement.Substring(0, index);

                    html = html.Replace(tag.Index, tag.Length, replacement);
                }
                else
                {
                    Logger.LogError(string.Format(ClassNotFoundError, pageID));
                }
            }

            // End
            foreach (var tag in k_SurroundEndTag.Matches(html).OrderByDescending(t => t.Index))
            {
                var pageID = tag.Groups[1].Value.NormalizePath();

                if (PageGenerator.Pages.ContainsKey(pageID))
                {
                    var replacement = PageGenerator.Pages[pageID].RenderedHtml;

                    var index = replacement.IndexOf(k_RenderSectionTag, StringComparison.InvariantCultureIgnoreCase) + k_RenderSectionTag.Length;
                    replacement = replacement.Substring(index, replacement.Length - index);

                    html = html.Replace(tag.Index, tag.Length, replacement);
                }
                else
                {
                    Logger.LogError(string.Format(ClassNotFoundError, pageID));
                }
            }
            return html.FixLineEndings();
        }
    }
}
