using HtmlGenerator.Generator;
using HtmlGenerator.Utils;
using System.Linq;
using System.Text.RegularExpressions;

namespace HtmlGenerator.Tags
{
    public class IncludeTag : BaseTag, ITag
    {
        public const string k_TagID = "Include";
        public string TagID => k_TagID;

        private readonly Regex k_IncludeClassTag = new Regex(RegexForTagAndClass(k_TagID), PreferredRegexOptions);
        public string Modify(PageGenerator PageGenerator, string mainPageID, string html)
        {
            var includeTags = k_IncludeClassTag.Matches(html);
            foreach (var tag in includeTags.OrderByDescending(t => t.Index))
            {
                var pageID = tag.Groups[1].Value.NormalizePath();

                if (!PageGenerator.Pages.ContainsKey(pageID))
                {
                    Logger.LogError($"'{mainPageID}': Class not found: {pageID}. Cannot Perform Include");
                    continue;
                }

                var replacement = PageGenerator.Pages[pageID].RenderedHtml;
                html = html.Replace(tag.Index, tag.Length, replacement);
            }

            return html.FixLineEndings();
        }
    }
}
