using System.Collections.Generic;
using HtmlTags;
using StoryTeller.Model;

namespace StoryTeller.UserInterface.Editing.HTML.Tables
{
    public class TableTemplateTag : TableTag
    {
        public TableTemplateTag(Table table, ICellBuilderLibrary builders)
        {
            Style("display", "none");
            AddClass(GrammarConstants.TEMPLATES);

            AddHeaderRow(x =>
            {
                x.Header(" ").AddClass("command");
                table.Cells.Each(cell =>
                {
                    x.Child(new CellHeaderTag(cell));
                });
            });

            AddBodyRow(x =>
            {
                x.Cell().AddClass("command").Child<DeleteIconTag>().AddClass("remover");
                table.Cells.Each(cell =>
                {
                    x.Cell().AddClass(cell.Key).Child(builders.BuildTag(cell));
                });
            });
        }
    }
}