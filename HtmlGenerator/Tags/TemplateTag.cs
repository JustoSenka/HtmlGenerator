using HtmlGenerator.Generator;
using HtmlGenerator.Utils;
using System.Linq;
using System.Text.RegularExpressions;

namespace HtmlGenerator.Tags
{
    public class TemplateTag : BaseTag, ITag
    {
        public const string k_TagID = "Template";
        public string TagID => k_TagID;

        private readonly Regex k_TemplateClassTag = new Regex(@"<template class=""(.*)"">([\w\W\n]*?)</template>", PreferredRegexOptions);
        private readonly Regex k_TemplateArgumants = new Regex(@"<(@[\w\-\.]+)>([\w\W\n]*?)</[ ]?\1>", PreferredRegexOptions);
        private readonly Regex k_TemplateVariables = new Regex(@"(@[\w\-\.]+)" + k_MatchVariableEnd, PreferredRegexOptions);
        private const string k_MatchVariableEnd = @"(?=[^\w\-\.])"; // Matches end of arg name so when replacing @arg, should not match part of @argB 
        public string Modify(PageGenerator PageGenerator, string mainPageID, string html)
        {
            foreach (var tag in k_TemplateClassTag.Matches(html).OrderByDescending(t => t.Index))
            {
                var pageID = tag.Groups[1].Value.NormalizePath();

                if (!PageGenerator.Pages.ContainsKey(pageID))
                {
                    Logger.LogError($"'{mainPageID}': Class not found: {pageID}. Cannot Insert Template");
                    continue;
                }

                // Parse arguments from main html file (only template section)
                var templateArgArea = tag.Groups[2].Value.TrimNewLinesAndDirSeparators();
                var args = k_TemplateArgumants.Matches(templateArgArea).Select(match =>
                {
                    var argName = match.Groups[1].Value;
                    var argValue = match.Groups[2].Value.TrimNewLinesAndDirSeparators();
                    return (argName, argValue);
                }).ToArray();

                var templateHtml = PageGenerator.Pages[pageID].RenderedHtml;

                var variablesInTemplate = k_TemplateVariables.Matches(templateHtml).Select(m => m.Groups[1].Value).Distinct(new PathEqualityComparer()).ToArray();
                if (variablesInTemplate.Length != args.Length)
                {
                    Logger.LogWarning($"'{mainPageID}': Argument number to insert template '{pageID}' was not equal. " +
                        $"Template has {variablesInTemplate.Length} variables, provided {args.Length} arguments.");
                }

                // Replace arguments with real values
                foreach (var (argName, argValue) in args)
                    templateHtml = Regex.Replace(templateHtml, argName + k_MatchVariableEnd, argValue, PreferredRegexOptions); 

                // Replace total template code in original html with generated code
                html = html.Replace(tag.Index, tag.Length, templateHtml);
            }

            return html.FixLineEndings();
        }
    }
}
