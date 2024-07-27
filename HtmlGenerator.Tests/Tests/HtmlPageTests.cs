using HtmlGenerator.Generator;
using HtmlGenerator.Tags;
using NUnit.Framework;
using System.IO;

namespace Tests
{
    public class HtmlPageTests : BaseTest
    {
        public const string k_TestPage_1 = "<h1>title</h1>";
        public const string k_IncludePage_1 = @"<body>
    <include src=""TestPage.html""/>
</body>";

        public const string k_TestResult_1 = @"<body>
    <h1>title</h1>
</body>";

        [Test]
        public void IncludeSimpleText_Works()
        {
            var page = new HtmlPage("IncludePage.html", PageGenerator, TagCollector, k_IncludePage_1);
            new HtmlPage("TestPage.html", PageGenerator, TagCollector, k_TestPage_1);

            Assert.That(k_TestResult_1 == page.RenderedHtml, "Html differ");
        }

        public const string k_TestPage_2 = "<h1>title{0}</h1>";
        public const string k_IncludePage_2 = @"<body>
    <include src=""TestPage0.html""/>
    <include src=""TestPage1.html""/>
    <dic>
        <include src=""TestPage2.html""/>
        <include src=""TestPage3.html""/>
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

            Assert.That(k_TestResult_2 == page.RenderedHtml, "Html are not the same");
        }

        public const string k_TestPage_3 = @"
<surround-begin src=""Surround.html""/>
    <h1>title</h1>
<surround-end src=""Surround.html""/>";

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

            Assert.That(k_TestResult_3 == page.RenderedHtml, "Html differ");
        }

        public const string k_SurroundBodyPage_4 = @"<body>
    <surround-content/>
</body>";

        public const string k_SurroundDivPage_4 = @"<div>
    <include src=""TestPage.html""/>
    <surround-content/>
</div>";

        public const string k_IncludePage_4 = @"<button>
    <include src=""TestPage.html""/>
</button>";

        public const string k_TestPage_4 = "<h1>title</h1>";

        public const string k_MainPage_4 = @"
<surround-begin src=""SurroundBody.html""/>
    <include src=""IncludePage.html""/>
    <surround-begin src=""SurroundDiv.html""/>
        <h2>subtitle</h2>
    <surround-end src=""SurroundDiv.html""/>
<surround-end src=""SurroundBody.html""/>";

        public const string k_TestResult_4 = @"
<body>
    
    <button>
    <h1>title</h1>
</button>
    <div>
    <h1>title</h1>
    
        <h2>subtitle</h2>
    
</div>

</body>";


        [Test]
        public void MixOfSurround_AndInclude_MultipleTimes_ProduceCorrectResult()
        {
            new HtmlPage("SurroundBody.html", PageGenerator, TagCollector, k_SurroundBodyPage_4);
            new HtmlPage("SurroundDiv.html", PageGenerator, TagCollector, k_SurroundDivPage_4);
            new HtmlPage("IncludePage.html", PageGenerator, TagCollector, k_IncludePage_4);
            new HtmlPage("TestPage.html", PageGenerator, TagCollector, k_TestPage_4);
            var page = new HtmlPage("MainPage.html", PageGenerator, TagCollector, k_MainPage_4);

            Assert.That(k_TestResult_4 == page.RenderedHtml, "Html differ");
        }
    }
}
