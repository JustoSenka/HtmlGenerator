using HtmlGenerator.Tags;
using HtmlGenerator.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HtmlGenerator.Generator
{
    public class PageGenerator
    {
        public virtual IDictionary<string, HtmlPage> Pages { get; protected set; }

        public string SourceFolder { get; set; } = "Resources";
        public string DestinationFolder { get; set; } = "Publish";

        private readonly TagCollector TagCollector;
        public PageGenerator(TagCollector TagCollector)
        {
            this.TagCollector = TagCollector;
            Pages = new Dictionary<string, HtmlPage>(new PathEqualityComparer());
        }

        public virtual void ScanFilesForHtml()
        {
            Pages = Directory.GetFileSystemEntries(SourceFolder, "*.html", SearchOption.AllDirectories)
                .Select(PathUtils.GetRelativePath)
                .Select(p => p.Replace(SourceFolder, "", StringComparison.InvariantCultureIgnoreCase).NormalizePath())
                .ToDictionary<string, string, HtmlPage>(p => p, p => new HtmlPage(p, this, TagCollector), new PathEqualityComparer());
        }

        public virtual void RenderToFile()
        {
            foreach (var (path, page) in Pages)
                page.RenderToFile();
        }

        public virtual void Render()
        {
            foreach (var (path, page) in Pages)
                page.Render();
        }

        public HtmlPage NewPage(string path, string overrideHtml = "")
        {
            return new HtmlPage(path, this, TagCollector, overrideHtml);
        }
    }
}
