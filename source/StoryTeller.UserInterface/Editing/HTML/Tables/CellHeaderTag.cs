using HtmlTags;
using HtmlTags.Extended.TagBuilders;
using StoryTeller.Engine;

namespace StoryTeller.UserInterface.Editing.HTML.Tables
{
    public class CellHeaderTag : HtmlTag
    {
        public CellHeaderTag(Cell cell)
            : base("th")
        {
            Text(cell.Header);
            AddClass(cell.Key);
            MetaData("key", cell.Key);
            MetaData("mandatory", !cell.HasDefault());
            if (cell.HasDefault())
            {
                this.ActionLink("X", GrammarConstants.COLUMN_REMOVER).MetaData("key", cell.Key);
            }
        }
    }
}