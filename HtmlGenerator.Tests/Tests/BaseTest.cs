using HtmlGenerator;
using HtmlGenerator.Generator;
using HtmlGenerator.Logging;
using HtmlGenerator.Tags;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tests
{
    public class BaseTest
    {
        protected PageGenerator PageGenerator;
        protected TagCollector TagCollector;

        [SetUp]
        [TearDown]
        public void CleanUp()
        {
            Logger.UsedLogger = new LoggerToConsole();

            TagCollector = new TagCollector();
            PageGenerator = new PageGenerator(TagCollector);

            PageGenerator.SourceFolder = "Resources";
            PageGenerator.DestinationFolder = "Publish";

            if (Directory.Exists(PageGenerator.DestinationFolder))
                Directory.Delete(PageGenerator.DestinationFolder, true);
        }

        protected static void CheckErrorCount(int count)
        {
            Assert.AreEqual(count, Logger.LogList.Count(log => log.LogType == LogType.Error), $"{count} errors should be reported");
        }

        protected static void CheckFirstErrorRegex(string errorMsg)
        {
            Assert.IsTrue(Regex.IsMatch(Logger.LogList.First(log => log.LogType == LogType.Error).Message, errorMsg,
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase), "Regex did not match error");
        }

        protected static void CheckWarningCount(int count)
        {
            Assert.AreEqual(count, Logger.LogList.Count(log => log.LogType == LogType.Warning), $"{count} warnings should be reported");
        }

        protected static void CheckFirstWarningRegex(string warningMsg)
        {
            Assert.IsTrue(Regex.IsMatch(Logger.LogList.First(log => log.LogType == LogType.Warning).Message, warningMsg,
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase), "Regex did not match warning");
        }
    }
}
