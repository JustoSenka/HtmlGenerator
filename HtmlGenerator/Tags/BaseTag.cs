using System.Text.RegularExpressions;

namespace HtmlGenerator.Tags
{
    public abstract class BaseTag
    {
        protected const RegexOptions PreferredRegexOptions = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
        protected static string RegexForTagAndClass(string tagID) => $@"<{tagID} {RegexClassCalpture}>";
        protected const string RegexClassCalpture = @"class=""(.*)""[ ]?/?";
    }

    public static class TagExtensions
    {
        public static string Replace(this string str, int index, int length, string replacement)
        {
            return str.Remove(index, length).Insert(index, replacement);
        }
    }
}
