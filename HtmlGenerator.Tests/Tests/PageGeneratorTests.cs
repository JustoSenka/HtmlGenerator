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
    <include src=""TestPage.html""/>
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

        [TestCase("_IgnoredInclude.html")]
        [TestCase("Directory/_IgnoredInclude.html")]
        [TestCase("_Directory/IgnoredInclude.html")]
        [TestCase("SomeOtherDir/_Directory/IgnoredInclude.html")]
        public void PageGenerator_WithIgnoredFiles_IncludesCorrectlyButDoesntCopyIgnoredFileToDestination(string ignoredFilePath)
        {
            var p1 = PageGenerator.NewPage("Include.html", $"<include src=\"{ignoredFilePath}\"/>");
            var p2 = PageGenerator.NewPage(ignoredFilePath, k_TestPage_1);

            PageGenerator.RenderToFile();

            FileAssert.Exists(p1.DestinationHtmlPath);
            FileAssert.DoesNotExist(p2.DestinationHtmlPath);

            Assert.AreEqual(k_TestPage_1, p1.RenderedHtml, "Html differ");
        }

        [TestCase("Directory/Include.html", "Directory/Include.html")]
        [TestCase("Directory/Include.html", "Directory\\Include.html")]
        [TestCase("Directory\\Include.html", "Directory/Include.HTML")]
        [TestCase("Directory\\INCLUDE.html", "Directory\\Include.html")]
        public void PageGenerator_WithPathsInDirectories_WithInconsistentDirSeparators_WorkFine(string pageID, string howItsReferencedPath)
        {
            var p1 = PageGenerator.NewPage("Main.html", $"<include src=\"{howItsReferencedPath}\"/>");
            var p2 = PageGenerator.NewPage(pageID, k_TestPage_1);

            PageGenerator.RenderToFile();

            FileAssert.Exists(p1.DestinationHtmlPath);
            FileAssert.Exists(p2.DestinationHtmlPath);

            Assert.AreEqual(k_TestPage_1, p1.RenderedHtml, "Html differ");
        }
    }
}