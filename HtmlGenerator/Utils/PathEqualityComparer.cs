using System;
using System.Collections.Generic;

namespace HtmlGenerator.Utils
{
    public class PathEqualityComparer : StringComparer, IEqualityComparer<string>
    {
        public override int Compare(string x, string y)
        {
            return x.ToLower().CompareTo(y.ToLower());
        }

        public override bool Equals(string x, string y)
        {
            return x.ToLower() == y.ToLower();
        }

        public override int GetHashCode(string obj)
        {
            return obj.ToLower().GetHashCode();
        }
    }
}
