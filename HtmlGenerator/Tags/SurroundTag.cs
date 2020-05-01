using HtmlGenerator.Generator;
using HtmlGenerator.Utils;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace HtmlGenerator.Tags
{
    public class SurroundTag : BaseTag, ITag
    {
        protected readonly Regex k_SurroundBeginTag = new Regex(@"<surround-begin class=""(.*)""[ ]?/?>", PreferredRegexOptions);
        protected readonly Regex k_SurroundEndTag = new Regex(@"<surround-end class=""(.*)""[ ]?/?>", PreferredRegexOptions);
        protected const string k_RenderSectionTag = @"<surround-content[ ]?/?>";
        
        public override string TagID => "Surround";

        private const string ClassNotFoundError = "Class not found: {0}. Cannot Perform Surround";
        private const string SurroundNotFoundError = "Tried to surround html with '{0}' but couldn't find '" + k_RenderSectionTag + "' tag inside it.";

        public override string Modify(PageGenerator PageGenerator, string html)
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
            var match = Regex.Match(replacement, k_RenderSectionTag, RegexOptions.IgnoreCase);
            if (match == null || match.Captures.Count == 0 || match.Captures[0].Index == -1)
            {
                Logger.LogError(string.Format(SurroundNotFoundError, pageID));
                return html;
            }

            var index = match.Captures[0].Index;
            if (isBeginTag)
            {
                replacement = replacement.Substring(0, index); // insert part from beginning to content tag
            }
            else
            {
                index = index + match.Captures[0].Length; // insert part from content tag till the end
                replacement = replacement.Substring(index, replacement.Length - index);
            }

            return html.Replace(tag.Index, tag.Length, replacement);
        }
    }
}
