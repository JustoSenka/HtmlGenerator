using HtmlGenerator.Generator;
using HtmlGenerator.Utils;
using System.Linq;
using System.Text.RegularExpressions;

namespace HtmlGenerator.Tags.Content
{
    public class SurroundTag : BaseTag, ITag
    {
        public const string k_TagID = "Surround";
        public string TagID => k_TagID;

        private readonly Regex k_SurroundBeginTag = new(RegexForTagAndClass("surround-begin"), PreferredRegexOptions);
        private readonly Regex k_SurroundEndTag = new(RegexForTagAndClass("surround-end"), PreferredRegexOptions);

        protected const string k_RenderSectionTag = @"<surround-content[ ]?/?>";

        private const string ClassNotFoundError = "'{0}': Class not found: {1}. Cannot Perform Surround";
        private const string SurroundNotFoundError = "'{0}': Tried to surround with '{1}' but couldn't find '" + k_RenderSectionTag + "' tag inside it.";

        public string Modify(PageGenerator PageGenerator, string mainPageID, string html)
        {
            var beginTags = k_SurroundBeginTag.Matches(html).OrderByDescending(t => t.Index);
            foreach (var tag in beginTags)
                html = ReplaceHtml(PageGenerator, tag, mainPageID, html, true);

            var endTags = k_SurroundEndTag.Matches(html).OrderByDescending(t => t.Index);
            foreach (var tag in endTags)
                html = ReplaceHtml(PageGenerator, tag, mainPageID, html, false);

            if (beginTags.Count() != endTags.Count())
                Logger.LogError($"'{mainPageID}': Html had unequal begin and end surround tag count. surround-begin count: {beginTags.Count()}, surround-end count: {endTags.Count()}");

            return html.FixLineEndings();
        }

        private static string ReplaceHtml(PageGenerator PageGenerator, Match tag, string mainPageID, string html, bool isBeginTag)
        {
            var pageID = tag.Groups[1].Value.NormalizePath();

            if (!PageGenerator.Pages.ContainsKey(pageID))
            {
                Logger.LogError(string.Format(ClassNotFoundError, mainPageID, pageID));
                return html;
            }

            var replacement = PageGenerator.Pages[pageID].RenderedHtml;
            var match = Regex.Match(replacement, k_RenderSectionTag, RegexOptions.IgnoreCase);
            if (match == null || match.Captures.Count == 0 || match.Captures[0].Index == -1)
            {
                Logger.LogError(string.Format(SurroundNotFoundError, mainPageID, pageID));
                return html;
            }

            var index = match.Captures[0].Index;
            if (isBeginTag)
            {
                replacement = replacement.Substring(0, index); // insert part from beginning to content tag
            }
            else
            {
                index += match.Captures[0].Length; // insert part from content tag till the end
                replacement = replacement.Substring(index, replacement.Length - index);
            }

            return html.Replace(tag.Index, tag.Length, replacement);
        }
    }
}
