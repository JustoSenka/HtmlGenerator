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

        public PageGenerator()
        {
            Pages = new Dictionary<string, HtmlPage>(new PathEqualityComparer());
        }

        public virtual void ScanFilesForHtml()
        {
            Pages = Directory.GetFileSystemEntries(SourceFolder, "*.html", SearchOption.AllDirectories)
                .Select(PathUtils.GetRelativePath)
                .Select(p => p.Replace(SourceFolder, "", StringComparison.InvariantCultureIgnoreCase).NormalizePath())
                .ToDictionary<string, string, HtmlPage>(p => p, p => new HtmlPage(p, this), new PathEqualityComparer());
        }

        public virtual void Build()
        {
            foreach (var (path, page) in Pages)
                page.RenderToFile();
        }
    }
}
