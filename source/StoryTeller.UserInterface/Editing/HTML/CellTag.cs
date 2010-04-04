using HtmlTags;
using StoryTeller.Engine;
using StoryTeller.Html;

namespace StoryTeller.UserInterface.Editing.HTML
{
    public class CellTag : HtmlTag
    {
        public CellTag(Cell cell, string tag)
            : base(tag)
        {
            AddClass(GrammarConstants.CELL);
            this.AddSafeClassName(cell.Key);
            MetaData(GrammarConstants.KEY, cell.Key);
        }
    }
}