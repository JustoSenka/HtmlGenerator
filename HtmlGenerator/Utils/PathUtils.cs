using System;
using System.IO;
using System.Text.RegularExpressions;

namespace HtmlGenerator.Utils
{
    public static class PathUtils
    {
        public static string GetRelativePath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return fullPath;

            var fullAbsolutePath = (File.Exists(fullPath)) ? new FileInfo(fullPath).FullName : new DirectoryInfo(fullPath).FullName;
            var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory).FullName;

            if (fullAbsolutePath.StartsWith(currentDirectory))
            {
                // The +1 is to avoid the directory separator
                return fullAbsolutePath.Substring(currentDirectory.Length + 1).NormalizePath();
            }
            else
            {
                Console.WriteLine("Unable to make relative path from: " + fullAbsolutePath +
                    " where current environment path is: " + currentDirectory);
            }

            return "";
        }

        public static string TrimNewLinesAndDirSeparators(this string path) => path.Trim('\n', '\r', ' ', '\\', '/');

        public static string NormalizePath(this string path)
        {
            return path.Replace("\\", "/").TrimNewLinesAndDirSeparators().FixLineEndings();
        }

        public static string FixLineEndings(this string str)
        {
            return Regex.Replace(str, @"\r\n|\n\r|\n|\r", Environment.NewLine);
        }

        public static void CopyDirectory(string sourceDirectory, string targetDirectory, Func<string, bool> ignoreFileFunc = default)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            if (ignoreFileFunc == default)
                ignoreFileFunc = _ => false;

            CopyDirectory(diSource, diTarget, ignoreFileFunc);
        }

        private static void CopyDirectory(DirectoryInfo source, DirectoryInfo target, Func<string, bool> ignoreFileFunc)
        {
            try
            {
                Directory.CreateDirectory(target.FullName);
            }
            catch (Exception e)
            {
                Logger.LogError($"Cannot create directory '{target.FullName}': {e.Message}");
                return;
            }

            // Copy each file into the new directory.
            foreach (var fileInfo in source.GetFiles())
            {
                if (ignoreFileFunc(fileInfo.FullName))
                    continue;

                try
                {
                    fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), true);
                }
                catch (Exception e)
                {
                    Logger.LogError($"Cannot copy file to destination '{target.FullName}': {e.Message}");
                }
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                if (ignoreFileFunc(diSourceSubDir.Name))
                    continue;

                try
                {
                    var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                    CopyDirectory(diSourceSubDir, nextTargetSubDir, ignoreFileFunc);
                }
                catch (Exception e)
                {
                    Logger.LogError($"Cannot create subdirectory '{Path.Combine(target.FullName, diSourceSubDir.Name)}': {e.Message}");
                }
            }

            // Delete empty directories at target afterwards
            if (target.GetFiles().Length == 0 && target.GetDirectories().Length == 0)
            {
                try
                {
                    target.Delete();
                }
                catch (Exception e)
                {
                    Logger.LogError($"Cannot delete empty directory '{target.FullName}': {e.Message}");
                }
            }
        }
    }
}
