using HtmlGenerator.Utils;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HtmlGenerator.Generator
{
    public class HtmlPage
    {
        public string Path { get; private set; }
        public string Html { get; private set; }

        public bool IsHtmlOverriden { get; private set; }

        public string SourceHtmlPath => System.IO.Path.Combine(PageGenerator.SourceFolder, Path);
        public string DestinationHtmlPath => System.IO.Path.Combine(PageGenerator.DestinationFolder, Path);

        protected string m_RenderedHtml;
        public string RenderedHtml => string.IsNullOrEmpty(m_RenderedHtml) ? m_RenderedHtml = Render() : m_RenderedHtml;

        // ----

        protected readonly Regex k_IncludeClassTag = new Regex(@"<include class=""(.*)""/>");
        protected readonly Regex k_SurroundBeginTag = new Regex(@"<surround class=""(.*)"">");
        protected readonly Regex k_SurroundEndTag = new Regex(@"</surround>");
        protected readonly Regex k_RenderSectionTag = new Regex(@"<rendersection/>");

        private readonly PageGenerator PageGenerator;

        public HtmlPage(string path, PageGenerator PageGenerator)
        {
            this.Path = PathUtils.NormalizePath(path);
            this.PageGenerator = PageGenerator;

            PageGenerator.Pages[this.Path] = this;
        }

        public virtual void OverrideHtml(string html)
        {
            Html = html;
            IsHtmlOverriden = true;
        }

        public virtual string Render()
        {
            if (!IsHtmlOverriden)
                TryReadHtmlFromFile();

            if (string.IsNullOrEmpty(Html))
            {
                Console.WriteLine($"[ERROR] {Html}: does not contain any HTML or failed to read.");
                return "";
            }

            var newHtml = Html;
            newHtml = HandleInclude(newHtml);
            return m_RenderedHtml = newHtml;
        }

        public virtual void RenderToFile()
        {
            Directory.CreateDirectory(new FileInfo(DestinationHtmlPath).DirectoryName);
            File.WriteAllText(DestinationHtmlPath, Render());
        }

        private string HandleInclude(string newHtml)
        {
            var includeTags = k_IncludeClassTag.Matches(newHtml);
            foreach (Match tag in includeTags.OrderByDescending(t => t.Index))
            {
                var pageID = tag.Groups[1].Value.NormalizePath();

                var replacement = PageGenerator.Pages.ContainsKey(pageID) ?
                    PageGenerator.Pages[pageID].RenderedHtml :
                    $"\n<!-- Error. Class not found: {pageID}. Cannot Perform Include -->";

                newHtml = newHtml.Remove(tag.Index, tag.Length).Insert(tag.Index, replacement).Normalize();
            }

            return newHtml;
        }

        private void TryReadHtmlFromFile()
        {
            try
            {
                Html = File.ReadAllText(SourceHtmlPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] {SourceHtmlPath}: failed to read file: {e.Message}");
            }
        }
    }
}
