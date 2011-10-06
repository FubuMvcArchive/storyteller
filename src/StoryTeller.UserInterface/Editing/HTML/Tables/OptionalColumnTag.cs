using HtmlTags;
using StoryTeller.Engine;

namespace StoryTeller.UserInterface.Editing.HTML.Tables
{
    public class OptionalColumnTag : LinkTag
    {
        public OptionalColumnTag(Cell cell)
            : base(cell.Header + ";", "#", GrammarConstants.COLUMN_ADDER, cell.Key)
        {
            MetaData(GrammarConstants.KEY, cell.Key);
            AddClass("add" + cell.Key);
        }
    }
}