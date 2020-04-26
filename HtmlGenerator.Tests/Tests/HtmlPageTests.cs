using HtmlGenerator.Generator;
using NUnit.Framework;
using System.IO;

namespace Tests
{
    public class HtmlPageTests
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

        [Test]
        public void IncludeSimpleText_Works()
        {
            var gen = new PageGenerator();
            new HtmlPage("IncludePage.html", gen).OverrideHtml(k_IncludePage_1);
            new HtmlPage("TestPage.html", gen).OverrideHtml(k_TestPage_1);

            var actualResult = gen.Pages["IncludePage.html"].RenderedHtml;
            Assert.AreEqual(k_TestResult_1, actualResult, "Html differ");
        }
    }
}