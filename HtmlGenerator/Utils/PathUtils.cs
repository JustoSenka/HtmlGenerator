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

        public static string NormalizePath(this string path)
        {
            return path.Trim('\n', '\r', ' ', '\\', '/').FixLineEndings();
        }

        public static string FixLineEndings(this string str)
        {
            return Regex.Replace(str, @"\r\n|\n\r|\n|\r", Environment.NewLine);
        }
    }
}
