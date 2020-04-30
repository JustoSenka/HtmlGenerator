using HtmlGenerator.Generator;
using HtmlGenerator.Utils;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace HtmlGenerator.Tags
{
    public class SurroundTag : ITag
    {
        protected readonly Regex k_SurroundBeginTag = new Regex(@"<surround-begin class=""(.*)""/+>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        protected readonly Regex k_SurroundEndTag = new Regex(@"<surround-end class=""(.*)""/+>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        protected const string k_RenderSectionTag = @"<surround-content/>";

        public string TagID => "Surround";

        private const string ClassNotFoundError = "Class not found: {0}. Cannot Perform Surround";
        private const string SurroundNotFoundError = "Tried to surround html with '{0}' but couldn't find '" + k_RenderSectionTag + "' tag inside it.";

        public string Modify(PageGenerator PageGenerator, string html)
        {
            var beginTags = k_SurroundBeginTag.Matches(html).OrderByDescending(t => t.Index);
            foreach (var tag in beginTags)
                html = ReplaceHtml(PageGenerator, tag, html, true);

            var endTags = k_SurroundEndTag.Matches(html).OrderByDescending(t => t.Index);
            foreach (var tag in endTags)
                html = ReplaceHtml(PageGenerator, tag, html, false);

            if (beginTags.Count() != endTags.Count())
                Logger.LogError($"Html had unequal begin and end surround tag count. surround-begin count: {beginTags.Count()}, surround-end count: {endTags.Count()}");

            return html.FixLineEndings();
        }

        private static string ReplaceHtml(PageGenerator PageGenerator, Match tag, string html, bool isBeginTag)
        {
            var pageID = tag.Groups[1].Value.NormalizePath();

            if (!PageGenerator.Pages.ContainsKey(pageID))
            {
                Logger.LogError(string.Format(ClassNotFoundError, pageID));
                return html;
            }

            var replacement = PageGenerator.Pages[pageID].RenderedHtml;
            var index = replacement.IndexOf(k_RenderSectionTag, StringComparison.InvariantCultureIgnoreCase);
            if (index == -1)
            {
                Logger.LogError(string.Format(SurroundNotFoundError, pageID));
                return html;
            }

            if (isBeginTag)
            {
                replacement = replacement.Substring(0, index); // insert part from beginning to content tag
            }
            else
            {
                index = index + k_RenderSectionTag.Length; // insert part from content tag till the end
                replacement = replacement.Substring(index, replacement.Length - index);
            }

            return html.Replace(tag.Index, tag.Length, replacement);
        }
    }
}
