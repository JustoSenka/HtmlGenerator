using System;
using System.IO;

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

        public void Run(string path)
        {
            PageGenerator.ScanFilesForHtml();
            PageGenerator.BuildWebpage();

            Changed += OnChanged;
            FileChanged += OnFileChanged;

            using (FileSystemWatcher watcher = CreateWatcher(path))
            {
                Logger.LogMessage("Starting interactive continious build.");
                Logger.LogMessage("Press 'q' to quit.");
                while (Console.ReadKey().KeyChar != 'q') ;
            }
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
            watcher.Filter = "*.html";
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
            if (e.FullPath.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase))
                FileChanged?.Invoke(e.FullPath);
        }

        private void OnRenamedInternal(object source, RenamedEventArgs e)
        {
            // Ignoring temp files from apps like visual studio
            if (e.FullPath.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase))
                Changed.Invoke();
        }
    }
}
