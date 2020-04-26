using System;
using HtmlGenerator.Generator;

namespace HtmlGenerator.Tags
{
    public abstract class BaseTag : ITag
    {
        public string TagID => throw new NotImplementedException();

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
