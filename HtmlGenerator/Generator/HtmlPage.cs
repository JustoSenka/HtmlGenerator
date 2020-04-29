using HtmlGenerator.Tags;
using HtmlGenerator.Utils;
using System;
using System.IO;

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

        private readonly PageGenerator PageGenerator;
        private readonly TagCollector TagCollector;
        public HtmlPage(string path, PageGenerator PageGenerator, TagCollector TagCollector, string overrideHtml = "")
        {
            this.Path = PathUtils.NormalizePath(path);
            this.PageGenerator = PageGenerator;
            this.TagCollector = TagCollector;

            PageGenerator.Pages[this.Path] = this;

            if (!string.IsNullOrEmpty(overrideHtml))
                OverrideHtml(overrideHtml);
        }

        public virtual void OverrideHtml(string html)
        {
            Html = html.FixLineEndings();
            IsHtmlOverriden = true;
        }

        public virtual string Render()
        {
            if (!IsHtmlOverriden)
                TryReadHtmlFromFile();

            if (string.IsNullOrEmpty(Html))
            {
                Logger.LogError($"{Html}: does not contain any HTML or failed to read file.");
                return "";
            }

            var newHtml = Html;
            foreach (var tag in TagCollector.IterateTagsConstantOrder())
                newHtml = tag.Modify(PageGenerator, newHtml);

            return m_RenderedHtml = newHtml.FixLineEndings();
        }

        public virtual void RenderToFile()
        {
            Directory.CreateDirectory(new FileInfo(DestinationHtmlPath).DirectoryName);
            File.WriteAllText(DestinationHtmlPath, Render());

            Logger.LogMessage($"Successfully created: {DestinationHtmlPath}");
        }

        private void TryReadHtmlFromFile()
        {
            try
            {
                Html = File.ReadAllText(SourceHtmlPath).FixLineEndings();
            }
            catch (Exception e)
            {
                Logger.LogError($"{SourceHtmlPath}: failed to read file: {e.Message}");
            }
        }
    }
}
