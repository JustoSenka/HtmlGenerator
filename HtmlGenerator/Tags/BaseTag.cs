using System;
using System.Text.RegularExpressions;
using HtmlGenerator.Generator;

namespace HtmlGenerator.Tags
{
    public abstract class BaseTag : ITag
    {
        protected const RegexOptions PreferredRegexOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

        public abstract string TagID { get; }
        public abstract string Modify(PageGenerator PageGenerator, string html);
    }

    public static class TagExtensions
    {
        public static string Replace(this string str, int index, int length, string replacement)
        {
            return str.Remove(index, length).Insert(index, replacement);
        }
    }
}
