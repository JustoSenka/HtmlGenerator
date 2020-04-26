using HtmlGenerator.Generator;
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

        [SetUp]
        [TearDown]
        public void CleanUp()
        {
            PageGenerator = new PageGenerator();
            if (Directory.Exists(PageGenerator.DestinationFolder))
                Directory.Delete(PageGenerator.DestinationFolder, true);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IncludeSimpleText_Works(bool useFileSystem)
        {
            if (!useFileSystem)
            {
                new HtmlPage("IncludePage.html", PageGenerator);
                new HtmlPage("TestPage.html", PageGenerator);
            }
            else
                PageGenerator.ScanFilesForHtml();

            PageGenerator.Build();

            var p1Actual = File.ReadAllText(PageGenerator.Pages["IncludePage.html"].DestinationHtmlPath);
            var p2Actual = File.ReadAllText(PageGenerator.Pages["TestPage.html"].DestinationHtmlPath);

            Assert.AreEqual(k_TestResult_1, p1Actual, "Html differ");
            Assert.AreEqual(k_TestPage_1, p2Actual, "Html differ");
        }

        [Test]
        public void DirectoriesAreCreated_ForDeepHtmlFiles()
        {
            var path = "folderA/folderB/IncludePage.html";
            var page = new HtmlPage(path, PageGenerator);
            page.RenderToFile();

            FileAssert.Exists(page.DestinationHtmlPath);
        }
    }
}