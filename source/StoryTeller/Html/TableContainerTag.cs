using HtmlTags;

namespace StoryTeller.Html
{
    public class TableContainerTag : TableTag
    {
        public TableContainerTag()
        {
            AddClass("table-container");
        }

        public void Add(string title, HtmlTag bodyTag)
        {
            AddBodyRow(row =>
            {
                row.AddClass("table-container-row");
                row.Cell(title).AddClass("table-title");
                row.Cell().AddClass("table-cell").Child(bodyTag);
            
            });
        }
    }
}