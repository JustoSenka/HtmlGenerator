using HtmlGenerator.Generator;
using HtmlGenerator.Tags;
using NUnit.Framework;
using System.IO;

namespace Tests
{
    public class HtmlPageTests
    {
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

        public const string k_TestPage_1 = "<h1>title</h1>";
        public const string k_IncludePage_1 = @"<body>
    <include class=""TestPage.html""/>
</body>";

        public const string k_TestResult_1 = @"<body>
    <h1>title</h1>
</body>";


        [Test]
        public void IncludeSimpleText_Works()
        {
            var page = new HtmlPage("IncludePage.html", PageGenerator, TagCollector, k_IncludePage_1);
            new HtmlPage("TestPage.html", PageGenerator, TagCollector, k_TestPage_1);

            Assert.AreEqual(k_TestResult_1, page.RenderedHtml, "Html differ");
        }

        public const string k_TestPage_2 = "<h1>title{0}</h1>";
        public const string k_IncludePage_2 = @"<body>
    <include class=""TestPage0.html""/>
    <include class=""TestPage1.html""/>
    <dic>
        <include class=""TestPage2.html""/>
        <include class=""TestPage3.html""/>
    </div>
</body>";

        public const string k_TestResult_2 = @"<body>
    <h1>title0</h1>
    <h1>title1
SomeRandomTextInNewLine</h1>
    <dic>
        <h1>title2</h1>
        <h1>title3</h1>
    </div>
</body>";


        [Test]
        public void MultipleIncludesInSameFile_AreAdded_Correctly()
        {
            new HtmlPage("TestPage0.html", PageGenerator, TagCollector, string.Format(k_TestPage_2, "0"));
            new HtmlPage("TestPage1.html", PageGenerator, TagCollector, string.Format(k_TestPage_2, "1\nSomeRandomTextInNewLine"));
            new HtmlPage("TestPage2.html", PageGenerator, TagCollector, string.Format(k_TestPage_2, "2"));
            new HtmlPage("TestPage3.html", PageGenerator, TagCollector, string.Format(k_TestPage_2, "3"));

            var page = new HtmlPage("IncludePage.html", PageGenerator, TagCollector, k_IncludePage_2);

            Assert.AreEqual(k_TestResult_2, page.RenderedHtml, "Html are not the same");
        }

        public const string k_TestPage_3 = @"
<surround-begin class=""Surround.html""/>
    <h1>title</h1>
<surround-end class=""Surround.html""/>";

        public const string k_SurroundPage_3 = @"<body>
    <surround-content/>
</body>";

        public const string k_TestResult_3 = @"
<body>
    
    <h1>title</h1>

</body>";


        [Test]
        public void SurroundSimpleScenario_Works()
        {
            new HtmlPage("Surround.html", PageGenerator, TagCollector, k_SurroundPage_3);
            var page = new HtmlPage("TestPage.html", PageGenerator, TagCollector, k_TestPage_3);

            Assert.AreEqual(k_TestResult_3, page.RenderedHtml, "Html differ");
        }
    }
}