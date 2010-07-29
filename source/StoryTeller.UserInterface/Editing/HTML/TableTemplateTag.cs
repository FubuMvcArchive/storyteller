using System.Collections.Generic;
using HtmlTags;
using StoryTeller.Engine;
using StoryTeller.Model;
using System.Linq;

namespace StoryTeller.UserInterface.Editing.HTML
{
    public class TableEditorTag : TableTag
    {
        public TableEditorTag()
        {
            Attr("cellpadding", "0").Attr("cellspacing", "0");
            AddClasses("grid", "editor");

            AddFooterRow(x =>
            {
                x.Cell().AddClass("table-add-row").Configure(td =>
                {
                    td.ActionLink("add").AddClass("adder");
                    td.ActionLink("clone").AddClass("cloner");
                });
            });
        }
    }

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
                x.Cell().AddClass("command").Child(new LinkTag("remove", "#", "remover"));
                table.Cells.Each(cell =>
                {
                    x.Cell().AddClass(cell.Key).Child(builders.BuildTag(cell));
                });
            });
        }
    }

    public class ColumnSelectionTag : HtmlTag
    {
        public ColumnSelectionTag(Table table) : base("p")
        {
            AddClass(GrammarConstants.COLUMN_SELECTOR);
            Add("span").Text(GrammarConstants.ADDITIONAL_COLUMNS_HEADER_TEXT);
            table.Cells
                .Where(x => x.HasDefault())
                .Each(cell => Child(new OptionalColumnTag(cell)));
        }
    }

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