using System;
using System.Collections.Generic;

namespace HtmlGenerator.Utils
{
    public class PathEqualityComparer : StringComparer, IEqualityComparer<string>
    {
        public override int Compare(string x, string y)
        {
            return x.NormalizePath().ToLower().CompareTo(y.NormalizePath().ToLower());
        }

        public override bool Equals(string x, string y)
        {
            return x.NormalizePath().ToLower() == y.NormalizePath().ToLower();
        }

        public override int GetHashCode(string obj)
        {
            return obj.NormalizePath().ToLower().GetHashCode();
        }
    }
}
