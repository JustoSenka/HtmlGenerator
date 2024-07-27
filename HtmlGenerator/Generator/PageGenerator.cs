using HtmlGenerator.Logging;
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

        public string SourceFolder { get; set; }
        public string DestinationFolder { get; set; }
        public string LibrariesFolder { get; set; }

        private readonly TagCollector TagCollector;
        public PageGenerator(TagCollector TagCollector)
        {
            this.TagCollector = TagCollector;
            Pages = new Dictionary<string, HtmlPage>(new PathEqualityComparer());
        }

        public virtual void ScanFilesForHtml()
        {
            try
            {
                Pages = Directory.GetFileSystemEntries(SourceFolder, "*.html", SearchOption.AllDirectories)
                    .Select(PathUtils.GetRelativePath)
                    .Select(p => p.Replace(SourceFolder, "", StringComparison.InvariantCultureIgnoreCase).NormalizePath())
                    .ToDictionary<string, string, HtmlPage>(p => p, p => new HtmlPage(p, this, TagCollector), new PathEqualityComparer());

                Logger.LogMessage($"{Pages.Count} HTML Source files found in: {SourceFolder}");
            }
            catch (DirectoryNotFoundException e)
            {
                Logger.LogError($"Could not scan directory '{SourceFolder}' for HTML files: {e.Message}");
            }
        }

        public void CleanBuild()
        {
            if (Directory.Exists(DestinationFolder))
            {
                Directory.Delete(DestinationFolder, true);
                Logger.LogMessage($"Successfully deleted: {DestinationFolder}");
            }
        }

        public virtual void BuildWebpage()
        {
            if (Directory.Exists(LibrariesFolder))
            {
                PathUtils.CopyDirectory(LibrariesFolder, DestinationFolder);
                Logger.LogMessage($"Successfully copied contents from '{LibrariesFolder}' to '{DestinationFolder}'");
            }
            else if (string.IsNullOrEmpty(LibrariesFolder))
            {
                Logger.LogMessage($"Skipping libraries since path is not provided:");
            }
            else
            {
                Logger.LogWarning($"Libraries directory not found: {LibrariesFolder}");
            }

            RenderToFile();
        }

        public void RenderToFile()
        {
            foreach (var (path, page) in Pages)
            {
                if (!path.StartsWith("_") && !path.Contains("/_") && !path.Contains("\\_"))
                    page.RenderToFile();
            }
        }

        public void Render()
        {
            foreach (var (_, page) in Pages)
                page.Render();
        }

        public HtmlPage NewPage(string path, string overrideHtml = "")
        {
            return new HtmlPage(path, this, TagCollector, overrideHtml);
        }

        public void ReportErrors()
        {
            Console.WriteLine();
            var errorCount = Logger.LogList.Count(log => log.LogType == LogType.Error);
            var warningCount = Logger.LogList.Count(log => log.LogType == LogType.Warning);

            if (errorCount == 0 && warningCount == 0)
                Logger.LogMessage("[Success] Build finished without any error(s)");
            else if (errorCount != 0 && warningCount == 0)
                Logger.LogMessage($"[Failure] Build finished with {errorCount} error(s)");
            else if (errorCount == 0 && warningCount != 0)
                Logger.LogMessage($"[Success] Build finished but with {warningCount} warning(s)");
            else
                Logger.LogMessage($"[Failure] Build finished with {errorCount} error(s) and {warningCount} warning(s)");

            Logger.LogList.Clear();
        }
    }
}
