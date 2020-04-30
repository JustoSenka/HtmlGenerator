using HtmlGenerator;
using HtmlGenerator.Generator;
using HtmlGenerator.Logging;
using HtmlGenerator.Tags;
using NUnit.Framework;
using System.IO;

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
    }
}
