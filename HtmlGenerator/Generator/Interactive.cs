using System;
using System.IO;
using System.Linq;

namespace HtmlGenerator.Generator
{
    public class Interactive
    {
        public event Action Changed;
        public event Action<string> FileChanged;

        private readonly PageGenerator PageGenerator;
        public Interactive(PageGenerator PageGenerator)
        {
            this.PageGenerator = PageGenerator;
        }

        public void Run(params string[] paths)
        {
            PageGenerator.ScanFilesForHtml();
            PageGenerator.BuildWebpage();

            Changed += OnChanged;
            FileChanged += OnFileChanged;

            var watchers = paths.Select(p => CreateWatcher(p)).ToArray();

            Logger.LogMessage("Starting interactive continious build.");
            Logger.LogMessage("Press 'q' to quit.");
            while (Console.ReadKey().KeyChar != 'q') ;

            foreach (var w in watchers)
                w.Dispose();
        }

        private void OnFileChanged(string obj)
        {
            RebuildEverything();
        }

        private void OnChanged()
        {
            RebuildEverything();
        }

        private void RebuildEverything()
        {
            Console.WriteLine();
            PageGenerator.ScanFilesForHtml();
            PageGenerator.BuildWebpage();
            PageGenerator.ReportErrors();
        }

        private FileSystemWatcher CreateWatcher(string path)
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*";
            watcher.IncludeSubdirectories = true;

            watcher.Changed += OnChangedInternal;
            watcher.Created += OnChangedInternal;
            watcher.Deleted += OnChangedInternal;
            watcher.Renamed += OnRenamedInternal;

            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        private void OnChangedInternal(object source, FileSystemEventArgs e)
        {
            // Ignoring temp files from apps like visual studio
            if (!ShouldIgnoreFile(e.FullPath))
                FileChanged?.Invoke(e.FullPath);
        }

        private void OnRenamedInternal(object source, RenamedEventArgs e)
        {
            // Ignoring temp files from apps like visual studio
            if (!ShouldIgnoreFile(e.FullPath))
                Changed.Invoke();
        }

        /// <summary>
        /// Ignores temporary or hidden files
        /// </summary>
        private bool ShouldIgnoreFile(string path)
        {
            var f = new FileInfo(path);
            return f.Attributes.HasFlag(FileAttributes.Temporary) ||
                f.Attributes.HasFlag(FileAttributes.Hidden) ||
                path.EndsWith("~") ||
                path.EndsWith(".TMP", StringComparison.InvariantCultureIgnoreCase) ||
                string.IsNullOrEmpty(f.Extension) ||
                f.Extension.Contains("~");
        }
    }
}
