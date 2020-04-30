using HtmlGenerator.Generator;
using HtmlGenerator.Tags;
using NUnit.Framework;
using System.IO;

namespace Tests
{
    public class PageGeneratorTests : BaseTest
    {
        public const string k_TestPage_1 = "<h1>title</h1>";
        public const string k_IncludePage_1 = @"<body>
    <include class=""TestPage.html""/>
</body>";

        public const string k_TestResult_1 = @"<body>
    <h1>title</h1>
</body>";

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

        [Test]
        public void PageGenerator_WithIgnoredFiles_IncludesCorrectlyButDoesntCopyIgnoredFileToDestination()
        {
            var p1 = PageGenerator.NewPage("Include.html", "<include class=\"_IgnoredInclude.html\"/>");
            var p2 = PageGenerator.NewPage("_IgnoredInclude.html", k_TestPage_1);

            PageGenerator.RenderToFile();

            FileAssert.Exists(p1.DestinationHtmlPath);
            FileAssert.DoesNotExist(p2.DestinationHtmlPath);

            Assert.AreEqual(k_TestPage_1, p1.RenderedHtml, "Html differ");
        }
    }
}