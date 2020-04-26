using HtmlGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HtmlGenerator.Tags
{
    public class TagCollector
    {
        public IDictionary<string, ITag> Tags { get; protected set; }

        public TagCollector()
        {
            Tags = AppDomain.CurrentDomain.GetAllTypesWhichImplementInterface(typeof(ITag))
                .Select(t => (ITag)Activator.CreateInstance(t))
                .OrderBy(t => t.TagID)
                .ToDictionary(t => t.TagID, t => t, new PathEqualityComparer());
        }

        public IEnumerable<ITag> IterateTagsConstantOrder()
        {
            return Tags.Values.OrderBy(t => t.TagID);
        }
    }
}
