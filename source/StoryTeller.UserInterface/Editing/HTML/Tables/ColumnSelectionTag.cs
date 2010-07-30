using System.Linq;
using HtmlTags;
using StoryTeller.Engine;
using StoryTeller.Model;
using GenericEnumerableExtensions = System.Collections.Generic.GenericEnumerableExtensions;

namespace StoryTeller.UserInterface.Editing.HTML.Tables
{
    public class ColumnSelectionTag : HtmlTag
    {
        public ColumnSelectionTag(Table table) : base("p")
        {
            AddClass(GrammarConstants.COLUMN_SELECTOR);
            Add("span").Text(GrammarConstants.ADDITIONAL_COLUMNS_HEADER_TEXT);
            GenericEnumerableExtensions.Each<Cell>(table.Cells
                                .Where(x => x.HasDefault()), cell => Child(new OptionalColumnTag(cell)));
        }
    }
}