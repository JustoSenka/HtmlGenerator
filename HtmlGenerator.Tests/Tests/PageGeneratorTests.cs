using HtmlGenerator.Generator;
using HtmlGenerator.Tags;
using NUnit.Framework;
using System.IO;

namespace Tests
{
    public class PageGeneratorTests
    {
        public const string k_TestPage_1 = "<h1>title</h1>";
        public const string k_IncludePage_1 = @"<body>
    <include class=""TestPage.html""/>
</body>";

        public const string k_TestResult_1 = @"<body>
    <h1>title</h1>
</body>";

        private PageGenerator PageGenerator;
        private TagCollector TagCollector;

        [SetUp]
        [TearDown]
        public void CleanUp()
        {
            TagCollector = new TagCollector();
            PageGenerator = new PageGenerator(TagCollector);

            if (Directory.Exists(PageGenerator.DestinationFolder))
                Directory.Delete(PageGenerator.DestinationFolder, true);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void PageGenerator_CreatesFilesInCorrectLocations(bool useFileSystem)
        {
            if (!useFileSystem)
            {
                new HtmlPage("IncludePage.html", PageGenerator, TagCollector);
                new HtmlPage("TestPage.html", PageGenerator, TagCollector);
            }
            else
                PageGenerator.ScanFilesForHtml();

            PageGenerator.RenderToFile();

            var p1Actual = File.ReadAllText(PageGenerator.Pages["IncludePage.html"].DestinationHtmlPath);
            var p2Actual = File.ReadAllText(PageGenerator.Pages["TestPage.html"].DestinationHtmlPath);

            Assert.AreEqual(k_TestResult_1, p1Actual, "Html differ");
            Assert.AreEqual(k_TestPage_1, p2Actual, "Html differ");
        }

        [Test]
        public void DirectoriesAreCreated_ForDeepHtmlFiles()
        {
            var path = "folderA/folderB/IncludePage.html";
            var page = new HtmlPage(path, PageGenerator, TagCollector);
            page.RenderToFile();

            FileAssert.Exists(page.DestinationHtmlPath);
        }
    }
}