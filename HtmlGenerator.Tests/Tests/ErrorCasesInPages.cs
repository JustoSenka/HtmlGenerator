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
    public class ErrorCasesInPages : BaseTest
    {
        public const string k_IncludePage_1 = @"<include src=""TestPage.html""/>";

        [Test]
        public void MissingInclude_ReportsError()
        {
            var page = PageGenerator.NewPage("IncludePage.html", k_IncludePage_1);
            page.Render();

            CheckErrorCount(1);
            CheckFirstErrorRegex($"Class not found.*TestPage.html");
        }

        public const string k_CorrectSurroundPage_2 = @"
<surround-begin src=""SurroundContent.html""/>
anything
<surround-end src=""SurroundContent.html""/>
";

        public const string k_CorrectSurroundPage_SpaceChangesAndMissingSlash_2 = @"
<surround-begin src=""SurroundContent.html"" >
anything
<surround-end src=""SurroundContent.html"" />
";

        public const string k_IncorrectSurroundPage_2 = @"
<surround-begin src=""SurroundContent.html""/>
anything
<surround-enedededd src=""SurroundContent.html""/>
";

        public const string k_IncorrectSurroundPageMissingClass_2 = @"
<surround-begin src=""SurroundContentMissing.html""/>
anything
<surround-end src=""SurroundContent.html""/>
";

        public const string k_CorrectSurroundContentPage_2 = @"
<body>
<surround-content/>
</body>
";

        public const string k_IncorrectSurroundContentPage_2 = @"
<body>
<surround-contentetet/>
</body>
";
        [TestCase(1, k_CorrectSurroundPage_2, k_CorrectSurroundContentPage_2, 0, "")]
        [TestCase(2, k_CorrectSurroundPage_SpaceChangesAndMissingSlash_2, k_CorrectSurroundContentPage_2, 0, "")]
        [TestCase(3, k_CorrectSurroundPage_2, k_IncorrectSurroundContentPage_2, 2, ".*SurroundContent.html.*surround-content")]
        [TestCase(4, k_IncorrectSurroundPage_2, k_CorrectSurroundContentPage_2, 1, "surround-begin count.* 1.*surround-end count.* 0")]
        [TestCase(5, k_IncorrectSurroundPageMissingClass_2, k_CorrectSurroundContentPage_2, 1, "Class not found.*SurroundContentMissing.html")]
        // id is used to easily distinguish different tests in test runner, since it becomes part of test name
        public void MissingSurrounContent_ReportsError(int id, string surroundPage, string contentPage, int errorCount, string errorRegex)
        {
            var content = PageGenerator.NewPage("SurroundContent.html", contentPage);
            var page = PageGenerator.NewPage("SurroundPage.html", surroundPage);
            page.Render();

            CheckErrorCount(errorCount);
            if (errorCount > 0)
                CheckFirstErrorRegex(errorRegex);
        }
    }
}
