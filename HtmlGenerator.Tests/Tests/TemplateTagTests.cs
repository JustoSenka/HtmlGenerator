using HtmlGenerator.Generator;
using HtmlGenerator.Tags;
using NUnit.Framework;
using System.IO;

namespace Tests
{
    public class TemplateTagTests : BaseTest
    {
        public const string k_TemlpateCode = @"
<p>
    <h1>@title</h1>
    @description<br>
    <a href=""@link"">@link-text</a>
</p>";

        public const string k_PageCode = @"
<template src=""MyTemplate.html"">
    <@description>
        Some multiline 
        description here
    </@description>
    <@title>Some title</@title>
    <@link>Link URl</@link>
    <@link-text>Some Text</@link-text>
</template>";

        public const string k_ExpectedCode = @"

<p>
    <h1>Some title</h1>
    Some multiline 
        description here<br>
    <a href=""Link URl"">Some Text</a>
</p>";

        [Test]
        public void SimpleTemplate_GeneratesCorrectHtml()
        {
            new HtmlPage("MyTemplate.html", PageGenerator, TagCollector, k_TemlpateCode);
            var page = new HtmlPage("MainPage.html", PageGenerator, TagCollector, k_PageCode);

            Assert.AreEqual(k_ExpectedCode, page.RenderedHtml, "Html differ");
        }

        public const string k_TemlpateCode_2 = @"
@var @varA
@var2
<strong>@var2</strong>
<button src=""@var-dash"">@var.dot</button>
<@var/> <@var>SomeText@varA</@var>
";

        public const string k_PageCode_2 = @"
<template src=""MyTemplate.html"">
    <@var>var</@var>
    <@var2>var2</@var2>
    <@varA>varA</@varA>
    <@var-dash>var-dash</@var-dash>
    <@var.dot>var.dot</@var.dot>
</template>";

        public const string k_ExpectedCode_2 = @"

var varA
var2
<strong>var2</strong>
<button src=""var-dash"">var.dot</button>
<var/> <var>SomeTextvarA</var>
";

        [Test]
        public void Template_WithOverlappingVariableNames_GeneratesCorrectHtml()
        {
            new HtmlPage("MyTemplate.html", PageGenerator, TagCollector, k_TemlpateCode_2);
            var page = new HtmlPage("MainPage.html", PageGenerator, TagCollector, k_PageCode_2);

            Assert.AreEqual(k_ExpectedCode_2, page.RenderedHtml, "Html differ");
        }

        public const string k_TemlpateCode_3 = @"@var - @varA;";

        public const string k_PageCode_3 = @"
<template src=""MyTemplate.html"">
    <@var><a href=""~/some/path.html"">Link</a></@var>
    <@varA><a href=""..\some\path.html"">Link</a></@varA>
</template>";

        public const string k_ExpectedCode_3 = @"
<a href=""~/some/path.html"">Link</a> - <a href=""..\some\path.html"">Link</a>;";

        [Test]
        public void Template_WithAnotherPathElementInside_GeneratesCorrectHtml()
        {
            new HtmlPage("MyTemplate.html", PageGenerator, TagCollector, k_TemlpateCode_3);
            var page = new HtmlPage("MainPage.html", PageGenerator, TagCollector, k_PageCode_3);

            Assert.AreEqual(k_ExpectedCode_3, page.RenderedHtml, "Html differ");
        }

        public const string k_CorrectTemplate = @"@var - @varA;";

        public const string k_PageCode_4_NoVars = @"
<template src=""MyTemplate.html"">
</template>";

        public const string k_PageCode_4_TooManyVars = @"
<template src=""MyTemplate.html"">
<@var>var</@var>
<@varA>varA</@varA>
<@varB>varB</@varB>
</template>";

        public const string k_ExpectedCode_4 = @"
<a href=""~/some/path.html"">Link</a> - <a href=""..\some\path.html"">Link</a>;";

        [TestCase(k_CorrectTemplate, k_PageCode_4_NoVars, 1, "Argument number.*was not equal.*2 variables.*0 arguments")]
        [TestCase(k_CorrectTemplate, k_PageCode_4_TooManyVars, 1, "Argument number.*was not equal.*2 variables.*3 arguments")]
        public void Template_WithIncorrectCases_FailsGracefully(string templateCode, string mainPage, int errorCount, string errorRegex)
        {
            new HtmlPage("MyTemplate.html", PageGenerator, TagCollector, templateCode);
            var page = new HtmlPage("MainPage.html", PageGenerator, TagCollector, mainPage);
            page.Render();

            CheckWarningCount(errorCount);
            if (errorCount > 0)
                CheckFirstWarningRegex(errorRegex);
        }
    }
}
