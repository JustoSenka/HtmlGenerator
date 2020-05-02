using HtmlGenerator.Generator;

namespace HtmlGenerator.Tags
{
    public interface ITag
    {
        string TagID { get; }
        string Modify(PageGenerator PageGenerator, string mainPageID, string html);
    }
}
